using Pos.Domain;

namespace Pos.Services;

public interface IJournalRepository
{
    Gradebook Open();
}

public interface IResultsWriter
{
    void Write(string path, IEnumerable<Result> rows);
}

public sealed record Result(int Id, string Name, decimal? Total);

public sealed class Journal(IJournalRepository repository, IResultsWriter writer)
{
    private readonly Gradebook _book = repository.Open();
    public IEnumerable<Student> Students => _book.Students;
    public IEnumerable<Criterion> Criteria => _book.Criteria;
    public Student AddStudent(string name) => _book.AddStudent(name);
    public Criterion AddCriterion(string name, int max, int weight) => _book.AddCriterion(name, max, weight);
    public void Grade(int student, int criterion, int score) => _book.Grade(student, criterion, score);
    public void Cancel(int student, int criterion) => _book.Cancel(student, criterion);

    public IEnumerable<(Criterion Criterion, Evaluation? Grade)> Grades(int student)
    {
        _book.GetStudent(student);
        return Criteria.Select(c => (c, _book.Find(student, c.Id)));
    }

    public decimal? Total(int student, ITotalRule rule) => Totals.Calculate(_book, student, rule);

    public void Export(string path) => writer.Write(path, Students.Select(s =>
        new Result(s.Id, s.Name, Total(s.Id, new FinalRule()))));
}
