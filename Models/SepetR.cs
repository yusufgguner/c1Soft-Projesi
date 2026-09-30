using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c1Soft_Projesi.Models;

public class SepetR
{
    [Key]
    public int SepetId { get; set; }

    public int KullaniciId { get; set; }

    public DateTime Tarih { get; set; } = DateTime.Now;

    public DateTime? GuncellemeTarihi { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BrutTutar { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal VergiTutar { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GenelTutar { get; set; }

    [StringLength(500)]
    public string? Notu { get; set; }

    // siparişe dönünce true. kullanıcının açık sepeti = Donustumu false olan
    public bool Donustumu { get; set; } = false;

    public User? Kullanici { get; set; }

    public List<SepetD> Kalemler { get; set; } = new List<SepetD>();
}
