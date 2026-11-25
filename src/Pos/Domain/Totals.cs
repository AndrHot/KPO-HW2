namespace Pos.Domain;

public interface ITotalRule
{
    bool Include(Evaluation? grade);
}

public sealed class FinalRule : ITotalRule
{
    public bool Include(Evaluation? grade) => true;
}

public sealed class CurrentRule : ITotalRule
{
    public bool Include(Evaluation? grade) => grade != null;
}

public static class Totals
{
    public static decimal? Calculate(Gradebook book, int student, ITotalRule rule)
    {
        book.GetStudent(student);
        decimal sum = 0, weights = 0;
        foreach (var criterion in book.Criteria)
        {
            var grade = book.Find(student, criterion.Id);
            if (!rule.Include(grade)) continue;
            sum += (decimal)(grade?.Score ?? 0) / criterion.Maximum * criterion.Weight;
            weights += criterion.Weight;
        }
        return weights == 0 ? null : Math.Round(10 * sum / weights, 2, MidpointRounding.AwayFromZero);
    }
}
