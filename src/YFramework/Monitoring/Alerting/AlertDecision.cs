namespace YFramework.Monitoring.Alerting;

public enum AlertTransition
{
    /// <summary>Durum değişmedi — uyarı gönderme (spam önleme).</summary>
    None = 0,

    /// <summary>Durum kötüleşti — uyarı gönder.</summary>
    Escalation = 1,

    /// <summary>Durum düzeldi — "toparlandı" uyarısı gönder.</summary>
    Recovery = 2
}

/// <summary>
/// "Her kontrolde değil, sadece durum değiştiğinde uyar" kuralının saf (test edilebilir) hali.
/// </summary>
public static class AlertDecision
{
    /// <param name="onceki">Bir önceki kontrolün sonucu; ilk kontrolse <c>null</c>.</param>
    /// <param name="simdi">Bu kontrolün sonucu.</param>
    public static AlertTransition Evaluate(CheckOutcome? onceki, CheckOutcome simdi)
    {
        if (onceki is null)
            return simdi == CheckOutcome.Ok ? AlertTransition.None : AlertTransition.Escalation;

        if (simdi == onceki.Value) return AlertTransition.None;
        return simdi > onceki.Value ? AlertTransition.Escalation : AlertTransition.Recovery;
    }
}
