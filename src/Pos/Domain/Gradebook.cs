namespace Pos.Domain;

public sealed record Student(int Id, string Name);
public sealed record Criterion(int Id, string Name, int Maximum, int Weight);

public sealed record Evaluation
{
    public int StudentId { get; }
    public int CriterionId { get; }
    public int Score { get; }

    private Evaluation(int studentId, int criterionId, int score) =>
        (StudentId, CriterionId, Score) = (studentId, criterionId, score);

    public static Evaluation Create(Gradebook book, int studentId, int criterionId, int score)
    {
        book.GetStudent(studentId);
        var criterion = book.GetCriterion(criterionId);
        if (score < 0 || score > criterion.Maximum)
            throw new ArgumentException($"Балл должен быть от 0 до {criterion.Maximum}.");
        return new Evaluation(studentId, criterionId, score);
    }
}

public sealed class Gradebook
{
    private readonly Dictionary<int, Student> _students = new();
    private readonly Dictionary<int, Criterion> _criteria = new();
    private readonly Dictionary<(int, int), Evaluation> _grades = new();

    public IEnumerable<Student> Students => _students.Values;
    public IEnumerable<Criterion> Criteria => _criteria.Values;

    private static string Name(string value) => string.IsNullOrWhiteSpace(value)
        ? throw new ArgumentException("Название или имя не может быть пустым.") : value.Trim();

    public Student AddStudent(string name)
    {
        var student = new Student(_students.Count + 1, Name(name));
        _students.Add(student.Id, student);
        return student;
    }

    public Criterion AddCriterion(string name, int maximum, int weight)
    {
        if (maximum <= 0 || weight <= 0)
            throw new ArgumentException("Максимум и вес должны быть положительными.");
        var criterion = new Criterion(_criteria.Count + 1, Name(name), maximum, weight);
        _criteria.Add(criterion.Id, criterion);
        return criterion;
    }

    public Student GetStudent(int id) => _students.GetValueOrDefault(id)
        ?? throw new ArgumentException("Студент не найден.");

    public Criterion GetCriterion(int id) => _criteria.GetValueOrDefault(id)
        ?? throw new ArgumentException("Критерий не найден.");

    public void Grade(int student, int criterion, int score) =>
        _grades[(student, criterion)] = Evaluation.Create(this, student, criterion, score);

    public void Cancel(int student, int criterion)
    {
        if (!_grades.Remove((student, criterion)))
            throw new ArgumentException("Такой оценки нет.");
    }

    public Evaluation? Find(int student, int criterion) => _grades.GetValueOrDefault((student, criterion));
}
