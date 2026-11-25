using Microsoft.Extensions.DependencyInjection;
using Pos.Services;
using Pos.Tests.Fakes;

namespace Pos.Tests;

public class ServicesTests
{
    [Fact]
    public void Export_WritesWeightedTotal()
    {
        var fake = new FakeResultsWriter();
        using var provider = new ServiceCollection().AddPos().AddSingleton<IResultsWriter>(fake).BuildServiceProvider();
        var journal = provider.GetRequiredService<Journal>();
        var student = journal.AddStudent("Анна");
        var practice = journal.AddCriterion("Практика", 20, 3);
        journal.AddCriterion("Защита", 10, 1);
        journal.Grade(student.Id, practice.Id, 10);

        journal.Export("results.csv");

        Assert.Equal(new Result(student.Id, "Анна", 3.75m), Assert.Single(fake.Rows));
    }
}
