namespace YFramework.Scheduling;

/// <summary>
/// Aynı anda birden fazla PARALEL kaynağı (personel, oda, lift/servis kanalı, araç…) olan
/// planlama senaryoları için çakışma kontrolü.
///
/// <see cref="OverlapChecker"/> tek bir aralık çiftini karşılaştırır; burada "N kaynaktan
/// hangileri şu slot için boş?" sorusu yanıtlanır. Kaynak kimliği olarak <see cref="int"/>
/// kullanılır (EF int anahtarlarıyla ve tipik domain modelleriyle uyumlu).
/// </summary>
public static class ResourceScheduler
{
    /// <summary>
    /// Belirli bir kaynağa ait rezervasyonlar içinde, istenen aralıkla çakışan var mı?
    /// (Kaynağı zaten seçilmiş bir plan için kullan.)
    /// </summary>
    public static bool HasConflict<TReservation>(
        IEnumerable<TReservation> reservationsOnResource,
        Func<TReservation, TimeRange> rangeSelector,
        TimeRange wanted)
    {
        ArgumentNullException.ThrowIfNull(reservationsOnResource);
        ArgumentNullException.ThrowIfNull(rangeSelector);

        return reservationsOnResource.Any(r => rangeSelector(r).Overlaps(wanted));
    }

    /// <summary>
    /// <paramref name="resources"/> içinden, <paramref name="wanted"/> aralığında hiçbir
    /// rezervasyonla çakışmayanları (giriş sırasını koruyarak) döndürür.
    /// </summary>
    public static IReadOnlyList<TResource> AvailableResources<TResource, TReservation>(
        IEnumerable<TResource> resources,
        Func<TResource, int> resourceIdSelector,
        IEnumerable<TReservation> reservations,
        Func<TReservation, int> reservationResourceIdSelector,
        Func<TReservation, TimeRange> reservationRangeSelector,
        TimeRange wanted)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(resourceIdSelector);
        ArgumentNullException.ThrowIfNull(reservations);
        ArgumentNullException.ThrowIfNull(reservationResourceIdSelector);
        ArgumentNullException.ThrowIfNull(reservationRangeSelector);

        var mesgulKaynakIdleri = reservations
            .Where(r => reservationRangeSelector(r).Overlaps(wanted))
            .Select(reservationResourceIdSelector)
            .ToHashSet();

        return resources
            .Where(res => !mesgulKaynakIdleri.Contains(resourceIdSelector(res)))
            .ToList();
    }

    /// <summary>
    /// İstenen aralıkta boş olan ilk kaynağı döndürür, yoksa <c>null</c>.
    /// Otomatik atama (ör. "boş ilk lifte al") için.
    /// </summary>
    public static TResource? FirstAvailable<TResource, TReservation>(
        IEnumerable<TResource> resources,
        Func<TResource, int> resourceIdSelector,
        IEnumerable<TReservation> reservations,
        Func<TReservation, int> reservationResourceIdSelector,
        Func<TReservation, TimeRange> reservationRangeSelector,
        TimeRange wanted)
        where TResource : class
    {
        return AvailableResources(
            resources, resourceIdSelector,
            reservations, reservationResourceIdSelector, reservationRangeSelector,
            wanted).FirstOrDefault();
    }
}
