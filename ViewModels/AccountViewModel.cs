using System.ComponentModel.DataAnnotations;

namespace c1Soft_Projesi.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [Display(Name = "Kullanıcı adı veya e-posta")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = "";
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta yazınız.")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = "";

    [Phone(ErrorMessage = "Geçerli bir telefon yazınız.")]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [Display(Name = "Kullanıcı adı")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = "";
}

public class AdminUserEditViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur.")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta yazınız.")]
    public string Email { get; set; } = "";

    [Phone(ErrorMessage = "Geçerli bir telefon yazınız.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string Username { get; set; } = "";

    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    public bool IsActive { get; set; } = true;
}
