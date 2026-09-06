using YFramework.Monitoring;
using YFramework.Monitoring.Alerting;

namespace YFramework.Tests.Monitoring;

public class AlertDecisionTests
{
    [Theory]
    [InlineData(null, CheckOutcome.Ok, AlertTransition.None)]            // ilk kontrol, sorun yok
    [InlineData(null, CheckOutcome.Warning, AlertTransition.Escalation)] // ilk kontrol, zaten sorunlu
    [InlineData(null, CheckOutcome.Critical, AlertTransition.Escalation)]
    [InlineData(CheckOutcome.Ok, CheckOutcome.Ok, AlertTransition.None)]           // sabit iyi
    [InlineData(CheckOutcome.Critical, CheckOutcome.Critical, AlertTransition.None)] // sabit kötü — spam yok
    [InlineData(CheckOutcome.Ok, CheckOutcome.Warning, AlertTransition.Escalation)]
    [InlineData(CheckOutcome.Warning, CheckOutcome.Critical, AlertTransition.Escalation)]
    [InlineData(CheckOutcome.Ok, CheckOutcome.Error, AlertTransition.Escalation)]
    [InlineData(CheckOutcome.Critical, CheckOutcome.Warning, AlertTransition.Recovery)]
    [InlineData(CheckOutcome.Warning, CheckOutcome.Ok, AlertTransition.Recovery)]
    [InlineData(CheckOutcome.Error, CheckOutcome.Ok, AlertTransition.Recovery)]
    public void Evaluate(CheckOutcome? onceki, CheckOutcome simdi, AlertTransition beklenen)
        => Assert.Equal(beklenen, AlertDecision.Evaluate(onceki, simdi));
}
