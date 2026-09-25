using c1Soft_Projesi.Data;
using c1Soft_Projesi.Models;
using c1Soft_Projesi.Services;
using c1Soft_Projesi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace c1Soft_Projesi.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly PasswordService passwordService;

    public AdminController(ApplicationDbContext db, PasswordService passwordService)
    {
        this.db = db;
        this.passwordService = passwordService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ProductCount = await db.Products.CountAsync();
        ViewBag.CategoryCount = await db.Categories.CountAsync();
        ViewBag.OrderCount = await db.Orders.CountAsync();
        ViewBag.UserCount = await db.Users.CountAsync();

        return View();
    }

    public async Task<IActionResult> Products()
    {
        var products = await db.Products
            .Include(x => x.Category)
            .OrderBy(x => x.ProductName)
            .ToListAsync();

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        await LoadCategories();
        return View(new Models.Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Models.Product model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View(model);
        }

        bool codeUsed = await db.Products.AnyAsync(x => x.ProductCode == model.ProductCode);

        if (codeUsed)
        {
            ModelState.AddModelError("ProductCode", "Bu ürün kodu zaten kayıtlı.");
            await LoadCategories();
            return View(model);
        }

        db.Products.Add(model);
        await db.SaveChangesAsync();

        return RedirectToAction("Products");
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await db.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        await LoadCategories();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(Models.Product model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View(model);
        }

        bool codeUsed = await db.Products.AnyAsync(x =>
            x.ProductCode == model.ProductCode && x.ProductId != model.ProductId);

        if (codeUsed)
        {
            ModelState.AddModelError("ProductCode", "Bu ürün kodu zaten kayıtlı.");
            await LoadCategories();
            return View(model);
        }

        var product = await db.Products.FindAsync(model.ProductId);

        if (product == null)
        {
            return NotFound();
        }

        product.CategoryId = model.CategoryId;
        product.ProductCode = model.ProductCode;
        product.ProductName = model.ProductName;
        product.Description = model.Description;
        product.Brand = model.Brand;
        product.ManufacturerCode = model.ManufacturerCode;
        product.CustomCode1 = model.CustomCode1;
        product.CustomCode2 = model.CustomCode2;
        product.ImageUrl = model.ImageUrl;
        product.StockQuantity = model.StockQuantity;
        product.CriticalStockLevel = model.CriticalStockLevel;
        product.Price = model.Price;

        await db.SaveChangesAsync();
        return RedirectToAction("Products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await db.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        product.IsActive = false;
        await db.SaveChangesAsync();

        return RedirectToAction("Products");
    }

    public async Task<IActionResult> Categories()
    {
        var categories = await db.Categories
            .OrderBy(x => x.CategoryName)
            .ToListAsync();

        return View(categories);
    }

    public async Task<IActionResult> Orders()
    {
        var orders = await db.Orders
            .Include(x => x.User)
            .Include(x => x.Items)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Users()
    {
        var users = await db.Users
            .Include(x => x.Role)
            .OrderBy(x => x.Username)
            .ToListAsync();

        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, string status)
    {
        string[] validStatuses = { "Pending", "Approved", "Rejected", "Shipped", "Delivered", "Cancelled" };

        if (!validStatuses.Contains(status))
        {
            return BadRequest();
        }

        var order = await db.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        order.OrderStatus = status;
        order.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();

        return RedirectToAction("Orders");
    }

    [HttpGet]
    public IActionResult CreateCategory()
    {
        return View(new Models.Category());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(Models.Category model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool nameUsed = await db.Categories.AnyAsync(x => x.CategoryName == model.CategoryName);

        if (nameUsed)
        {
            ModelState.AddModelError("CategoryName", "Bu kategori zaten kayıtlı.");
            return View(model);
        }

        db.Categories.Add(model);
        await db.SaveChangesAsync();

        return RedirectToAction("Categories");
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await db.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(Models.Category model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool nameUsed = await db.Categories.AnyAsync(x =>
            x.CategoryName == model.CategoryName && x.CategoryId != model.CategoryId);

        if (nameUsed)
        {
            ModelState.AddModelError("CategoryName", "Bu kategori zaten kayıtlı.");
            return View(model);
        }

        var category = await db.Categories.FindAsync(model.CategoryId);

        if (category == null)
        {
            return NotFound();
        }

        category.CategoryName = model.CategoryName;
        category.Description = model.Description;
        await db.SaveChangesAsync();

        return RedirectToAction("Categories");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await db.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        bool hasProducts = await db.Products.AnyAsync(x =>
            x.CategoryId == id && x.IsActive);

        if (hasProducts)
        {
            TempData["AdminError"] = "Ürünü bulunan kategori pasif yapılamaz.";
            return RedirectToAction("Categories");
        }

        category.IsActive = false;
        await db.SaveChangesAsync();

        return RedirectToAction("Categories");
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await db.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return View(new AdminUserEditViewModel
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Username = user.Username,
            IsActive = user.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(AdminUserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool duplicate = await db.Users.AnyAsync(x =>
            x.UserId != model.UserId &&
            (x.Username == model.Username || x.Email == model.Email));

        if (duplicate)
        {
            ModelState.AddModelError("", "Kullanıcı adı veya e-posta zaten kullanılıyor.");
            return View(model);
        }

        var user = await db.Users.FindAsync(model.UserId);

        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.Phone = model.Phone;
        user.Username = model.Username;
        user.IsActive = model.IsActive;

        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            user.PasswordHash = passwordService.Hash(model.NewPassword);
        }

        await db.SaveChangesAsync();
        return RedirectToAction("Users");
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await db.Orders
            .Include(x => x.User)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    private async Task LoadCategories()
    {
        ViewBag.Categories = await db.Categories
            .Where(x => x.IsActive)
            .OrderBy(x => x.CategoryName)
            .ToListAsync();
    }
}
