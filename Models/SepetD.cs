using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c1Soft_Projesi.Models;

public class SepetD
{
    [Key]
    public int Sayac { get; set; }

    public int SepetId { get; set; }

    public int UrunId { get; set; }

    [Required]
    [StringLength(50)]
    public string UrunKodu { get; set; } = "";

    [Required]
    [StringLength(150)]
    public string UrunAdi { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int Miktar { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BirimFiyat { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BirimTutar { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal KDVOrani { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal KDVTutari { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GenelToplam { get; set; }

    public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

    public SepetR? Sepet { get; set; }

    public Product? Urun { get; set; }
}
