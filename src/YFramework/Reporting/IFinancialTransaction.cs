namespace YFramework.Reporting;

/// <summary>
/// Gelir/gider raporlaması yapabilmek için bir işlemin sağlaması gereken minimum bilgi.
/// Uygulamalar kendi muhasebe kayıt sınıflarında bu arayüzü implemente eder.
/// </summary>
public interface IFinancialTransaction
{
    DateTime Tarih { get; }
    decimal Tutar { get; }
    bool GelirMi { get; }
}
