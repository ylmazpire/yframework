namespace YFramework.Validation;

/// <summary>
/// Vergi kimlik numarası (VKN, 10 hane) doğrulaması — Maliye Bakanlığı kontrol algoritması.
/// </summary>
public static class TurkishTaxNumberValidator
{
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        var s = value.Trim();
        if (s.Length != 10) return false;

        var d = new int[10];
        for (var i = 0; i < 10; i++)
        {
            if (!char.IsDigit(s[i])) return false;
            d[i] = s[i] - '0';
        }

        var toplam = 0;
        for (var i = 0; i < 9; i++)
        {
            var tmp = (d[i] + (9 - i)) % 10;
            toplam += tmp == 9 ? 9 : (tmp * (1 << (9 - i))) % 9;
        }

        var kontrol = toplam % 10 == 0 ? 0 : 10 - (toplam % 10);
        return kontrol == d[9];
    }
}
