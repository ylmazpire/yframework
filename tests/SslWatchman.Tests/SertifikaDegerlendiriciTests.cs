using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SslWatchman.Services;
using YFramework.Monitoring;

namespace SslWatchman.Tests;

public class SertifikaDegerlendiriciTests
{
    private static readonly DateTime Simdi = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private static X509Certificate2 Sertifika(string cn, DateTime baslangicUtc, DateTime bitisUtc, string? san = null, string issuerO = "Test CA")
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest($"CN={cn}, O={issuerO}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        if (san is not null)
        {
            var sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddDnsName(san);
            req.CertificateExtensions.Add(sanBuilder.Build());
        }
        return req.CreateSelfSigned(new DateTimeOffset(baslangicUtc), new DateTimeOffset(bitisUtc));
    }

    private static SertifikaKontrolSonucu Degerlendir(X509Certificate2 cert, string host, int esik = 21, SslPolicyErrors errors = SslPolicyErrors.None)
        => SertifikaDegerlendirici.Degerlendir(cert, host, errors, esik, Simdi);

    [Fact]
    public void GecerliVeUzakBitis_Ok()
    {
        using var c = Sertifika("ornek.com", Simdi.AddDays(-10), Simdi.AddDays(60), san: "ornek.com");
        var s = Degerlendir(c, "ornek.com");
        Assert.Equal(CheckOutcome.Ok, s.Sonuc);
        Assert.Contains("60 gün", s.Mesaj);
    }

    [Fact]
    public void EsikIcinde_Warning()
    {
        using var c = Sertifika("ornek.com", Simdi.AddDays(-10), Simdi.AddDays(15), san: "ornek.com");
        Assert.Equal(CheckOutcome.Warning, Degerlendir(c, "ornek.com", esik: 21).Sonuc);
    }

    [Fact]
    public void YediGundenAz_Critical()
    {
        using var c = Sertifika("ornek.com", Simdi.AddDays(-10), Simdi.AddDays(5), san: "ornek.com");
        var s = Degerlendir(c, "ornek.com");
        Assert.Equal(CheckOutcome.Critical, s.Sonuc);
        Assert.Contains("5 gün", s.Mesaj);
    }

    [Fact]
    public void SuresiDolmus_Critical()
    {
        using var c = Sertifika("ornek.com", Simdi.AddDays(-90), Simdi.AddDays(-3), san: "ornek.com");
        var s = Degerlendir(c, "ornek.com");
        Assert.Equal(CheckOutcome.Critical, s.Sonuc);
        Assert.Contains("3 gün önce doldu", s.Mesaj);
    }

    [Fact]
    public void HenuzGecerliDegil_Critical()
    {
        using var c = Sertifika("ornek.com", Simdi.AddDays(5), Simdi.AddDays(90), san: "ornek.com");
        Assert.Equal(CheckOutcome.Critical, Degerlendir(c, "ornek.com").Sonuc);
    }

    [Fact]
    public void AlanAdiEslesmiyor_Critical()
    {
        using var c = Sertifika("baska.com", Simdi.AddDays(-10), Simdi.AddDays(90), san: "baska.com");
        var s = Degerlendir(c, "ornek.com");
        Assert.Equal(CheckOutcome.Critical, s.Sonuc);
        Assert.Contains("kapsamıyor", s.Mesaj);
    }

    [Fact]
    public void ZincirHatasi_Critical()
    {
        using var c = Sertifika("ornek.com", Simdi.AddDays(-10), Simdi.AddDays(90), san: "ornek.com");
        var s = Degerlendir(c, "ornek.com", errors: SslPolicyErrors.RemoteCertificateChainErrors);
        Assert.Equal(CheckOutcome.Critical, s.Sonuc);
        Assert.Contains("zinciri doğrulanamadı", s.Mesaj);
    }

    [Theory]
    [InlineData("CN=R11, O=Let's Encrypt, C=US", "Let's Encrypt")]
    [InlineData("CN=E1, O=Let's Encrypt", "Let's Encrypt")]
    [InlineData("CN=Sectigo RSA DV, C=GB", "Sectigo RSA DV")]
    public void KisaIssuer_OVarsaOnuYoksaCn(string dn, string beklenen)
        => Assert.Equal(beklenen, SertifikaDegerlendirici.KisaIssuer(dn));
}
