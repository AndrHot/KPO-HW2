using System.Globalization;
using System.Text;
using Pos.Domain;

namespace Pos.Services;

public sealed class MemoryRepository : IJournalRepository
{
    private readonly Gradebook _book = new();
    public Gradebook Open() => _book;
}

public sealed class CsvWriter : IResultsWriter
{
    public void Write(string path, IEnumerable<Result> rows)
    {
        using var file = new StreamWriter(path, false, new UTF8Encoding(true));
        file.WriteLine("ID;Студент;Итог из 10");
        foreach (var row in rows)
        {
            var name = row.Name.Replace("\"", "\"\"");
            var total = row.Total?.ToString("0.##", CultureInfo.GetCultureInfo("ru-RU"));
            file.WriteLine($"{row.Id};\"{name}\";{total}");
        }
    }
}
