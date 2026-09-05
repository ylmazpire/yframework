using YFramework.Reporting;

namespace KuaforApp.Models;

public class DashboardViewModel
{
    public int BugunkuRandevuSayisi { get; set; }
    public int BuHaftakiRandevuSayisi { get; set; }
    public int ToplamMusteriSayisi { get; set; }
    public int ToplamHizmetSayisi { get; set; }
    public FinancialSummary BuAyOzeti { get; set; } = new(0, 0);
    public List<Randevu> BugunkuRandevular { get; set; } = new();
    public List<AylikTrendNoktasi> AylikTrend { get; set; } = new();
}
