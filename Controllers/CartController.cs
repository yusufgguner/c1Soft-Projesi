using System.Security.Claims;
using c1Soft_Projesi.Data;
using c1Soft_Projesi.Models;
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
        int userId = GetUserId();

        var cart = await db.Carts
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        return View(cart ?? new Cart());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        int userId = GetUserId();

        var product = await db.Products
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.IsActive);

        if (product == null)
        {
            return NotFound();
        }

        var cart = await db.Carts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            db.Carts.Add(cart);
        }

        var cartItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        int requestedQuantity = (cartItem?.Quantity ?? 0) + quantity;

        if (quantity < 1 || requestedQuantity > product.StockQuantity)
        {
            TempData["CartError"] = $"Bu ürün için en fazla {product.StockQuantity} adet ekleyebilirsiniz.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        if (cartItem == null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity
            });
        }
        else
        {
            cartItem.Quantity = requestedQuantity;
        }

        cart.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int cartItemId, int quantity)
    {
        int userId = GetUserId();

        var item = await db.CartItems
            .Include(x => x.Product)
            .Include(x => x.Cart)
            .FirstOrDefaultAsync(x => x.CartItemId == cartItemId && x.Cart!.UserId == userId);

        if (item == null)
        {
            return NotFound();
        }

        var product = item.Product!;

        if (quantity < 1 || quantity > product.StockQuantity)
        {
            TempData["CartError"] = $"Bu ürün için en fazla {product.StockQuantity} adet seçebilirsiniz.";
            return RedirectToAction("Index");
        }

        item.Quantity = quantity;
        item.Cart!.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        int userId = GetUserId();

        var item = await db.CartItems
            .Include(x => x.Cart)
            .FirstOrDefaultAsync(x => x.CartItemId == cartItemId && x.Cart!.UserId == userId);

        if (item == null)
        {
            return NotFound();
        }

        db.CartItems.Remove(item);
        await db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
