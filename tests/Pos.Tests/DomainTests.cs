using Pos.Domain;

namespace Pos.Tests;

public class DomainTests
{
    private readonly Gradebook _book = new();

    public DomainTests()
    {
        _book.AddStudent("Анна");
        _book.AddCriterion("Практика", 20, 3);
        _book.AddCriterion("Защита", 10, 1);
    }

    [Theory]
    [InlineData(1, 1, -1)]
    [InlineData(1, 1, 21)]
    [InlineData(1, 99, 5)]
    [InlineData(99, 1, 5)]
    public void Grade_RejectsInvalidValueOrId(int student, int criterion, int score)
    {
        _book.Grade(1, 1, 10);
        Assert.Throws<ArgumentException>(() => _book.Grade(student, criterion, score));
        Assert.Equal(10, _book.Find(1, 1)!.Score);
    }

    [Fact]
    public void Total_RecalculatesAfterCorrectionAndCancellation()
    {
        _book.Grade(1, 1, 10);
        _book.Grade(1, 2, 10);
        Assert.Equal(6.25m, Totals.Calculate(_book, 1, new FinalRule()));
        _book.Grade(1, 1, 20);
        Assert.Equal(10m, Totals.Calculate(_book, 1, new FinalRule()));
        _book.Cancel(1, 2);
        Assert.Equal(7.5m, Totals.Calculate(_book, 1, new FinalRule()));
        Assert.Equal(10m, Totals.Calculate(_book, 1, new CurrentRule()));
    }

    [Fact]
    public void CurrentTotal_DistinguishesMissingGradeFromZero()
    {
        Assert.Null(Totals.Calculate(_book, 1, new CurrentRule()));
        _book.Grade(1, 1, 0);
        Assert.Equal(0m, Totals.Calculate(_book, 1, new CurrentRule()));
    }
}
