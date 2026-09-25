using System.ComponentModel.DataAnnotations;

namespace c1Soft_Projesi.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } = "";

    [StringLength(250)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Product> Products { get; set; } = new List<Product>();
}
