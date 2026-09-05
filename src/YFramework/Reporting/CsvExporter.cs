using System.Text;

namespace YFramework.Reporting;

/// <summary>
/// Basit, bağımlılıksız CSV üretici. Excel'de sorunsuz Türkçe karakter görünmesi için
/// UTF-8 BOM ekler. Herhangi bir liste/rapor verisini dışa aktarmak için kullanılabilir.
/// </summary>
public static class CsvExporter
{
    public static byte[] ToCsv(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(';', headers.Select(Escape)));

        foreach (var row in rows)
        {
            sb.AppendLine(string.Join(';', row.Select(Escape)));
        }

        var preamble = Encoding.UTF8.GetPreamble();
        var body = Encoding.UTF8.GetBytes(sb.ToString());
        var result = new byte[preamble.Length + body.Length];
        preamble.CopyTo(result, 0);
        body.CopyTo(result, preamble.Length);
        return result;
    }

    private static readonly char[] FormulaTetikleyiciler = { '=', '+', '-', '@' };

    private static string Escape(string value)
    {
        // Excel/LibreOffice CSV formül enjeksiyonu (CSV injection): bir hücre "=", "+", "-" veya
        // "@" ile başlıyorsa, dosya açıldığında formül olarak çalıştırılabilir (örn. "=cmd|'/c calc'!A1").
        // Başına tek tırnak koymak bu yorumlamayı engeller, veriyi bozmaz.
        if (value.Length > 0 && FormulaTetikleyiciler.Contains(value[0]))
        {
            value = "'" + value;
        }

        if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        return value;
    }
}
