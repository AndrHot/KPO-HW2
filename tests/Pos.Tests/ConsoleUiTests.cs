using Pos.Services;
using Pos.Tests.Fakes;
using Pos.Ui;

namespace Pos.Tests;

public class ConsoleUiTests
{
    [Fact]
    public void RunDemo_PrintsGradesAndExportsTotal()
    {
        var fake = new FakeResultsWriter();
        var journal = new Journal(new MemoryRepository(), fake);
        using var output = new StringWriter();

        new ConsoleUi(journal, TextReader.Null, output).RunDemo();

        Assert.Contains("Практика: 10 из 20", output.ToString());
        Assert.Equal(6.25m, Assert.Single(fake.Rows).Total);
    }
}
