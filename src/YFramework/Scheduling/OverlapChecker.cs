namespace YFramework.Scheduling;

/// <summary>
/// İki zaman aralığının çakışıp çakışmadığını kontrol eden genel amaçlı yardımcı sınıf.
/// Randevu sistemleri, oda/kaynak rezervasyonları gibi zamanlama gerektiren senaryolarda kullanılır.
/// </summary>
public static class OverlapChecker
{
    public static bool Overlaps(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        return start1 < end2 && start2 < end1;
    }
}
