using System.Security.Claims;
using c1Soft_Projesi.Data;
using c1Soft_Projesi.Models;
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
        int userId = GetUserId();

        var orders = await db.Orders
            .Include(x => x.Items)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        int userId = GetUserId();

        var order = await db.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.OrderId == id && x.UserId == userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        int userId = GetUserId();

        var cart = await db.Carts
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (cart == null || cart.Items.Count == 0)
        {
            TempData["OrderError"] = "Sipariş oluşturmak için sepetinizde ürün olmalıdır.";
            return RedirectToAction("Index", "Cart");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string shippingAddress)
    {
        int userId = GetUserId();

        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            ModelState.AddModelError("", "Teslimat adresi zorunludur.");
            return View();
        }

        var cart = await db.Carts
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (cart == null || cart.Items.Count == 0)
        {
            TempData["OrderError"] = "Sepetiniz boş olduğu için sipariş oluşturulamadı.";
            return RedirectToAction("Index", "Cart");
        }

        foreach (var item in cart.Items)
        {
            if (item.Product == null || item.Quantity > item.Product.StockQuantity)
            {
                TempData["OrderError"] = "Sepetteki ürünlerden birinin stoğu yeterli değil.";
                return RedirectToAction("Index", "Cart");
            }
        }

        using var transaction = await db.Database.BeginTransactionAsync();

        var order = new Order
        {
            UserId = userId,
            OrderNumber = "ORD-" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
            OrderStatus = "Pending",
            ShippingAddress = shippingAddress,
            CreatedAt = DateTime.Now
        };

        foreach (var item in cart.Items)
        {
            var product = item.Product!;
            decimal lineTotal = product.Price * item.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                LineTotal = lineTotal
            });

            db.StockMovements.Add(new StockMovement
            {
                ProductId = product.ProductId,
                UserId = userId,
                MovementType = "Order",
                QuantityChange = -item.Quantity,
                OldQuantity = product.StockQuantity,
                NewQuantity = product.StockQuantity - item.Quantity,
                Note = "Sipariş: " + order.OrderNumber
            });

            product.StockQuantity -= item.Quantity;
            order.TotalAmount += lineTotal;
        }

        db.Orders.Add(order);
        db.CartItems.RemoveRange(cart.Items);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return RedirectToAction("Index");
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
