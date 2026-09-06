namespace YFramework.Scheduling;

/// <summary>
/// Yarı açık bir zaman aralığı: [Start, End). Randevu, rezervasyon, servis planı gibi
/// senaryolarda çakışma kontrolünü tek bir tipte toplar.
/// </summary>
public readonly record struct TimeRange(DateTime Start, DateTime End)
{
    public TimeSpan Duration => End - Start;

    public bool IsValid => End > Start;

    /// <summary>Bu aralık verilen aralıkla kesişiyor mu? (uçların değmesi çakışma sayılmaz)</summary>
    public bool Overlaps(TimeRange other) => OverlapChecker.Overlaps(Start, End, other.Start, other.End);

    public static TimeRange FromDuration(DateTime start, TimeSpan duration) => new(start, start + duration);
}
