namespace KuaforApp.Models;

public class GunSonuViewModel
{
    public DateTime Tarih { get; set; }
    public int ToplamRandevu { get; set; }
    public int TamamlananSayisi { get; set; }
    public int IptalSayisi { get; set; }
    public int PlanliKalanSayisi { get; set; }
    public decimal ToplamKazanc { get; set; }
    public List<Randevu> Randevular { get; set; } = new();
}
