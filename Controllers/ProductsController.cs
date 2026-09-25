using c1Soft_Projesi.Data;
using c1Soft_Projesi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace c1Soft_Projesi.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext db;

    public ProductsController(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        var products = db.Products
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            products = products.Where(x =>
                x.ProductName.Contains(search) ||
                x.ProductCode.Contains(search) ||
                (x.Brand != null && x.Brand.Contains(search)) ||
                (x.Description != null && x.Description.Contains(search)) ||
                (x.ManufacturerCode != null && x.ManufacturerCode.Contains(search)) ||
                (x.CustomCode1 != null && x.CustomCode1.Contains(search)) ||
                (x.CustomCode2 != null && x.CustomCode2.Contains(search)));
        }

        if (categoryId.HasValue)
        {
            products = products.Where(x => x.CategoryId == categoryId.Value);
        }

        ViewBag.Categories = await db.Categories
            .Where(x => x.IsActive)
            .OrderBy(x => x.CategoryName)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;

        ViewBag.GridColumns = await db.ProductGridColumns
            .Where(x => x.IsVisible)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        return View(await products.OrderBy(x => x.ProductName).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await db.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.ProductId == id && x.IsActive);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}
