using System.Security.Claims;
using c1Soft_Projesi.Data;
using c1Soft_Projesi.Models;
using c1Soft_Projesi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace c1Soft_Projesi.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext db;

    public OrdersController(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<IActionResult> Index()
    {
        int kullaniciId = GetUserId();

        var siparisler = await db.SiparisR
            .Include(x => x.Kalemler)
            .Where(x => x.KullaniciId == kullaniciId)
            .OrderByDescending(x => x.Tarih)
            .ToListAsync();

        return View(siparisler);
    }

    public async Task<IActionResult> Details(int id)
    {
        int kullaniciId = GetUserId();

        var siparis = await db.SiparisR
            .Include(x => x.Kalemler)
            .FirstOrDefaultAsync(x => x.SiparisId == id && x.KullaniciId == kullaniciId);

        if (siparis == null)
        {
            return NotFound();
        }

        return View(siparis);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        int kullaniciId = GetUserId();

        var sepet = await AcikSepetiGetir(kullaniciId);

        if (sepet == null || sepet.Kalemler.Count == 0)
        {
            TempData["OrderError"] = "Sipariş oluşturmak için sepetinizde ürün olmalıdır.";
            return RedirectToAction("Index", "Cart");
        }

        return View(sepet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string shippingAddress, string? notu)
    {
        int kullaniciId = GetUserId();

        var sepet = await AcikSepetiGetir(kullaniciId);

        if (sepet == null || sepet.Kalemler.Count == 0)
        {
            TempData["OrderError"] = "Sepetiniz boş olduğu için sipariş oluşturulamadı.";
            return RedirectToAction("Index", "Cart");
        }

        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            ModelState.AddModelError("", "Teslimat adresi zorunludur.");
            return View(sepet);
        }

        foreach (var kalem in sepet.Kalemler)
        {
            if (kalem.Urun == null || kalem.Miktar > kalem.Urun.StockQuantity)
            {
                TempData["OrderError"] = "Sepetteki ürünlerden birinin stoğu yeterli değil.";
                return RedirectToAction("Index", "Cart");
            }
        }

        // sepette beklerken fiyat değişmiş olabilir, güncel fiyatla tekrar hesaplıyorum
        foreach (var kalem in sepet.Kalemler)
        {
            kalem.BirimFiyat = kalem.Urun!.Price;
            SepetHesaplama.KalemHesapla(kalem);
        }

        SepetHesaplama.ToplamlariHesapla(sepet);
        sepet.Notu = notu;

        using var transaction = await db.Database.BeginTransactionAsync();

        var siparis = new SiparisR
        {
            SiparisNo = "SIP-" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
            SepetId = sepet.SepetId,
            KullaniciId = kullaniciId,
            Tarih = DateTime.Now,
            BrutTutar = sepet.BrutTutar,
            VergiTutar = sepet.VergiTutar,
            GenelTutar = sepet.GenelTutar,
            Notu = sepet.Notu,
            TeslimatAdresi = shippingAddress,
            SiparisDurumu = "Pending"
        };

        foreach (var kalem in sepet.Kalemler)
        {
            siparis.Kalemler.Add(new SiparisD
            {
                UrunId = kalem.UrunId,
                UrunKodu = kalem.UrunKodu,
                UrunAdi = kalem.UrunAdi,
                Miktar = kalem.Miktar,
                BirimFiyat = kalem.BirimFiyat,
                BirimTutar = kalem.BirimTutar,
                KDVOrani = kalem.KDVOrani,
                KDVTutari = kalem.KDVTutari,
                GenelToplam = kalem.GenelToplam
            });

            var urun = kalem.Urun!;

            db.StockMovements.Add(new StockMovement
            {
                ProductId = urun.ProductId,
                UserId = kullaniciId,
                MovementType = "Order",
                QuantityChange = -kalem.Miktar,
                OldQuantity = urun.StockQuantity,
                NewQuantity = urun.StockQuantity - kalem.Miktar,
                Note = "Sipariş: " + siparis.SiparisNo
            });

            urun.StockQuantity -= kalem.Miktar;
        }

        db.SiparisR.Add(siparis);

        // sepeti silmiyorum, dönüştü diye işaretliyorum. yeni ürün eklenince yeni SepetR açılıyor
        sepet.Donustumu = true;

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return RedirectToAction("Details", new { id = siparis.SiparisId });
    }

    private async Task<SepetR?> AcikSepetiGetir(int kullaniciId)
    {
        return await db.SepetR
            .Include(x => x.Kalemler)
            .ThenInclude(x => x.Urun)
            .FirstOrDefaultAsync(x => x.KullaniciId == kullaniciId && x.Donustumu == false);
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
