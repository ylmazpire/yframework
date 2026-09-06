using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using YFramework.Monitoring;

namespace SslWatchman.Services;

public record SertifikaKontrolSonucu(
    CheckOutcome Sonuc,
    string Mesaj,
    DateTime? BitisUtc,
    string? Issuer,
    IReadOnlyList<string> SanListesi)
{
    public string ToJson() => JsonSerializer.Serialize(new
    {
        bitisUtc = BitisUtc,
        issuer = Issuer,
        san = SanListesi
    });
}

/// <summary>
/// Bir host:port'a TLS bağlanır, sunucunun yaprak sertifikasını okur ve
/// <see cref="SertifikaDegerlendirici"/>'ye devrederek bir <see cref="CheckOutcome"/> üretir.
/// Veritabanına dokunmaz; kaydetme <see cref="DomainKontrolOrkestratoru"/>'nda.
/// </summary>
public class SertifikaKontrolServisi
{
    private static readonly TimeSpan BaglantiZamanAsimi = TimeSpan.FromSeconds(10);

    public async Task<SertifikaKontrolSonucu> KontrolEtAsync(string host, int port, int uyariEsigiGun, CancellationToken ct = default)
    {
        X509Certificate2? sertifika = null;
        SslPolicyErrors politikaHatalari = SslPolicyErrors.None;

        try
        {
            using var tcp = new TcpClient();
            using var zamanAsimiCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            zamanAsimiCts.CancelAfter(BaglantiZamanAsimi);

            await tcp.ConnectAsync(host, port, zamanAsimiCts.Token);

            await using var ssl = new SslStream(tcp.GetStream(), leaveInnerStreamOpen: false,
                userCertificateValidationCallback: (_, cert, _, errors) =>
                {
                    // Geçersiz sertifikayı da incelemek istiyoruz — her zaman true dön, kararı biz veririz.
                    if (cert is not null) sertifika = new X509Certificate2(cert);
                    politikaHatalari = errors;
                    return true;
                });

            await ssl.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
            {
                TargetHost = host // SNI — sanal host'larda doğru sertifikayı almak için şart
            }, zamanAsimiCts.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return Hata($"Bağlantı zaman aşımına uğradı ({BaglantiZamanAsimi.TotalSeconds:0} sn).");
        }
        catch (Exception ex)
        {
            return Hata($"Bağlantı/TLS hatası: {ex.Message}");
        }

        if (sertifika is null)
            return Hata("Sunucu sertifika sunmadı.");

        using (sertifika)
        {
            return SertifikaDegerlendirici.Degerlendir(sertifika, host, politikaHatalari, uyariEsigiGun, DateTime.UtcNow);
        }

        static SertifikaKontrolSonucu Hata(string mesaj)
            => new(CheckOutcome.Error, mesaj, null, null, Array.Empty<string>());
    }
}
