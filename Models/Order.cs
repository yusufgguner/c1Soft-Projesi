using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c1Soft_Projesi.Models;

public class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    [Required]
    [StringLength(30)]
    public string OrderNumber { get; set; } = "";

    [Required]
    [StringLength(30)]
    public string OrderStatus { get; set; } = "Pending";

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(300)]
    public string? ShippingAddress { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}
