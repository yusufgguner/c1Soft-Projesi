namespace c1Soft_Projesi.Models;

public class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }

    public List<CartItem> Items { get; set; } = new List<CartItem>();
}
