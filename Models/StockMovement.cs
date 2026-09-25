using System.ComponentModel.DataAnnotations;

namespace c1Soft_Projesi.Models;

public class StockMovement
{
    public int StockMovementId { get; set; }

    public int ProductId { get; set; }

    public int? UserId { get; set; }

    [Required]
    [StringLength(30)]
    public string MovementType { get; set; } = "";

    public int QuantityChange { get; set; }

    public int OldQuantity { get; set; }

    public int NewQuantity { get; set; }

    [StringLength(250)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Product? Product { get; set; }

    public User? User { get; set; }
}
