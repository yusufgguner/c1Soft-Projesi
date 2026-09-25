using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c1Soft_Projesi.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    [Required]
    [StringLength(50)]
    public string ProductCode { get; set; } = "";

    [Required]
    [StringLength(150)]
    public string ProductName { get; set; } = "";

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; }

    public Order? Order { get; set; }

    public Product? Product { get; set; }
}
