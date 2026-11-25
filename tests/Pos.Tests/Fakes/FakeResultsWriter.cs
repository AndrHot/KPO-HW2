using Pos.Services;

namespace Pos.Tests.Fakes;

public sealed class FakeResultsWriter : IResultsWriter
{
    public IReadOnlyList<Result> Rows { get; private set; } = [];

    public void Write(string path, IEnumerable<Result> rows) => Rows = rows.ToList();
}
