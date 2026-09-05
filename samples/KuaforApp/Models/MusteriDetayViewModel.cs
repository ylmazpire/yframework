namespace KuaforApp.Models;

public class MusteriDetayViewModel
{
    public Musteri Musteri { get; set; } = null!;
    public List<Randevu> Randevular { get; set; } = new();
    public decimal ToplamHarcama { get; set; }
    public int TamamlananRandevuSayisi { get; set; }
}
