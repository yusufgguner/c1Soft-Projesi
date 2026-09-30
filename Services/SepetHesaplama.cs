using c1Soft_Projesi.Models;

namespace c1Soft_Projesi.Services;

public static class SepetHesaplama
{
    public const decimal KdvOrani = 20;

    public static void KalemHesapla(SepetD kalem)
    {
        kalem.KDVOrani = KdvOrani;
        kalem.BirimTutar = kalem.BirimFiyat * kalem.Miktar;
        kalem.KDVTutari = Math.Round(kalem.BirimTutar * kalem.KDVOrani / 100, 2);
        kalem.GenelToplam = kalem.BirimTutar + kalem.KDVTutari;
    }

    public static void ToplamlariHesapla(SepetR sepet)
    {
        decimal brutTutar = 0;
        decimal vergiTutar = 0;
        decimal genelTutar = 0;

        foreach (var kalem in sepet.Kalemler)
        {
            brutTutar += kalem.BirimTutar;
            vergiTutar += kalem.KDVTutari;
            genelTutar += kalem.GenelToplam;
        }

        sepet.BrutTutar = brutTutar;
        sepet.VergiTutar = vergiTutar;
        sepet.GenelTutar = genelTutar;
        sepet.GuncellemeTarihi = DateTime.Now;
    }
}
