using System.ComponentModel.DataAnnotations;

namespace c1Soft_Projesi.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    public int RoleId { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = "";

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = "";

    [Required]
    [StringLength(150)]
    [EmailAddress]
    public string Email { get; set; } = "";

    [StringLength(30)]
    public string? Phone { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Role? Role { get; set; }
}
