using System.ComponentModel.DataAnnotations;

namespace c1Soft_Projesi.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Kategori seçiniz.")]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(50)]
    public string ProductCode { get; set; } = "";

    [Required]
    [StringLength(150)]
    public string ProductName { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Brand { get; set; }

    [StringLength(50)]
    public string? ManufacturerCode { get; set; }

    [StringLength(50)]
    public string? CustomCode1 { get; set; }

    [StringLength(50)]
    public string? CustomCode2 { get; set; }

    [StringLength(300)]
    public string? ImageUrl { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stok negatif olamaz.")]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Kritik stok negatif olamaz.")]
    public int CriticalStockLevel { get; set; } = 5;

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Fiyat geçerli olmalıdır.")]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Category? Category { get; set; }
}
