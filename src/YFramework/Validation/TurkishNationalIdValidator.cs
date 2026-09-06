namespace YFramework.Validation;

/// <summary>
/// T.C. kimlik numarası (11 hane) doğrulaması — resmi kontrol algoritması.
/// DataAnnotations'tan bağımsız kullanılabilmesi için mantık burada; attribute bunu çağırır.
/// </summary>
public static class TurkishNationalIdValidator
{
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        var s = value.Trim();
        if (s.Length != 11) return false;

        var d = new int[11];
        for (var i = 0; i < 11; i++)
        {
            if (!char.IsDigit(s[i])) return false;
            d[i] = s[i] - '0';
        }

        // İlk hane 0 olamaz.
        if (d[0] == 0) return false;

        // 10. hane: (tek konumların toplamı * 7 - çift konumların toplamı) mod 10
        var tekToplam = d[0] + d[2] + d[4] + d[6] + d[8];
        var ciftToplam = d[1] + d[3] + d[5] + d[7];
        var onuncu = ((tekToplam * 7) - ciftToplam) % 10;
        if (onuncu < 0) onuncu += 10;
        if (onuncu != d[9]) return false;

        // 11. hane: ilk 10 hanenin toplamı mod 10
        var ilkOnToplam = 0;
        for (var i = 0; i < 10; i++) ilkOnToplam += d[i];
        return ilkOnToplam % 10 == d[10];
    }
}
