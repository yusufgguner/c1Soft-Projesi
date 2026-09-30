using System.Security.Claims;
using c1Soft_Projesi.Data;
using c1Soft_Projesi.Models;
using c1Soft_Projesi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace c1Soft_Projesi.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext db;

    public CartController(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<IActionResult> Index()
    {
        int kullaniciId = GetUserId();

        var sepet = await AcikSepetiGetir(kullaniciId);

        return View(sepet ?? new SepetR());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        int kullaniciId = GetUserId();

        var urun = await db.Products
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.IsActive);

        if (urun == null)
        {
            return NotFound();
        }

        var sepet = await AcikSepetiGetir(kullaniciId);

        if (sepet == null)
        {
            sepet = new SepetR
            {
                KullaniciId = kullaniciId,
                Tarih = DateTime.Now,
                Donustumu = false
            };
            db.SepetR.Add(sepet);
        }

        var kalem = sepet.Kalemler.FirstOrDefault(x => x.UrunId == productId);
        int istenenMiktar = (kalem?.Miktar ?? 0) + quantity;

        if (quantity < 1 || istenenMiktar > urun.StockQuantity)
        {
            TempData["CartError"] = $"Bu ürün için en fazla {urun.StockQuantity} adet ekleyebilirsiniz.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        if (kalem == null)
        {
            kalem = new SepetD
            {
                UrunId = urun.ProductId,
                UrunKodu = urun.ProductCode,
                UrunAdi = urun.ProductName,
                EklenmeTarihi = DateTime.Now
            };
            sepet.Kalemler.Add(kalem);
        }

        kalem.Miktar = istenenMiktar;
        kalem.BirimFiyat = urun.Price;
        SepetHesaplama.KalemHesapla(kalem);
        SepetHesaplama.ToplamlariHesapla(sepet);

        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int sayac, int quantity)
    {
        int kullaniciId = GetUserId();

        var sepet = await AcikSepetiGetir(kullaniciId);
        var kalem = sepet?.Kalemler.FirstOrDefault(x => x.Sayac == sayac);

        if (sepet == null || kalem == null)
        {
            return NotFound();
        }

        var urun = kalem.Urun!;

        if (quantity < 1 || quantity > urun.StockQuantity)
        {
            TempData["CartError"] = $"Bu ürün için en fazla {urun.StockQuantity} adet seçebilirsiniz.";
            return RedirectToAction("Index");
        }

        kalem.Miktar = quantity;
        kalem.BirimFiyat = urun.Price;
        SepetHesaplama.KalemHesapla(kalem);
        SepetHesaplama.ToplamlariHesapla(sepet);

        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int sayac)
    {
        int kullaniciId = GetUserId();

        var sepet = await AcikSepetiGetir(kullaniciId);
        var kalem = sepet?.Kalemler.FirstOrDefault(x => x.Sayac == sayac);

        if (sepet == null || kalem == null)
        {
            return NotFound();
        }

        sepet.Kalemler.Remove(kalem);
        db.SepetD.Remove(kalem);
        SepetHesaplama.ToplamlariHesapla(sepet);

        await db.SaveChangesAsync();

        return RedirectToAction("Index");
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
