using System.ComponentModel.DataAnnotations;

namespace c1Soft_Projesi.Models;

public class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.Now;

    public Cart? Cart { get; set; }

    public Product? Product { get; set; }
}
