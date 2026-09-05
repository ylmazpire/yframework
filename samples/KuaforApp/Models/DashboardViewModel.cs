namespace KuaforApp.Models;

public class DashboardViewModel
{
    public int BugunkuRandevuSayisi { get; set; }
    public int BuHaftakiRandevuSayisi { get; set; }
    public int ToplamMusteriSayisi { get; set; }
    public int ToplamHizmetSayisi { get; set; }
    public List<Randevu> BugunkuRandevular { get; set; } = new();
}
