using Pos.Domain;
using Pos.Services;

namespace Pos.Ui;

public sealed class ConsoleUi(Journal journal, TextReader input, TextWriter output)
{
    public void Run()
    {
        while (true)
        {
            output.WriteLine("""

                1. Добавить студента
                2. Добавить критерий
                3. Студенты и критерии
                4. Выставить или исправить оценку
                5. Отменить оценку
                6. Оценки и итог студента
                7. Экспорт итогов в CSV
                0. Выход
                """);
            try
            {
                switch (Read("Пункт: "))
                {
                    case "0": output.WriteLine("До встречи!"); return;
                    case "1": AddStudent(); break;
                    case "2": AddCriterion(); break;
                    case "3": PrintLists(); break;
                    case "4":
                        journal.Grade(ReadInt("ID студента: "), ReadInt("ID критерия: "), ReadInt("Балл: "));
                        output.WriteLine("Оценка записана.");
                        break;
                    case "5":
                        journal.Cancel(ReadInt("ID студента: "), ReadInt("ID критерия: "));
                        output.WriteLine("Оценка отменена.");
                        break;
                    case "6": PrintResults(ReadInt("ID студента: ")); break;
                    case "7": Export(Read("Путь к CSV: ")); break;
                    default: output.WriteLine("Нет такого пункта."); break;
                }
            }
            catch (EndOfStreamException)
            {
                output.WriteLine("Ввод завершён.");
                return;
            }
            catch (ArgumentException e)
            {
                output.WriteLine($"Ошибка: {e.Message}");
            }
        }
    }

    public void RunDemo()
    {
        output.WriteLine("===== Демо =====");
        var student = journal.AddStudent("Анна Иванова");
        var practice = journal.AddCriterion("Практика", 20, 3);
        var defence = journal.AddCriterion("Защита", 10, 1);
        journal.Grade(student.Id, practice.Id, 10);
        journal.Grade(student.Id, defence.Id, 10);
        PrintLists();
        PrintResults(student.Id);
        Export("results.csv");
    }

    private void AddStudent()
    {
        var student = journal.AddStudent(Read("Имя студента: "));
        output.WriteLine($"Добавлен студент №{student.Id}: {student.Name}");
    }

    private void AddCriterion()
    {
        var criterion = journal.AddCriterion(Read("Название: "), ReadInt("Максимум: "), ReadInt("Вес: "));
        output.WriteLine($"Добавлен критерий №{criterion.Id}: {criterion.Name}");
    }

    private void PrintLists()
    {
        output.WriteLine("Студенты:");
        foreach (var student in journal.Students)
            output.WriteLine($"  {student.Id}. {student.Name}");
        output.WriteLine("Критерии:");
        foreach (var criterion in journal.Criteria)
            output.WriteLine($"  {criterion.Id}. {criterion.Name}, максимум {criterion.Maximum}, вес {criterion.Weight}");
    }

    private void PrintResults(int id)
    {
        foreach (var (criterion, grade) in journal.Grades(id))
            output.WriteLine($"{criterion.Name}: {grade?.Score.ToString() ?? "—"} из {criterion.Maximum}");
        output.WriteLine($"Текущий балл из 10: {journal.Total(id, new CurrentRule())?.ToString() ?? "—"}");
        output.WriteLine($"Итог из 10: {journal.Total(id, new FinalRule())?.ToString() ?? "—"}");
    }

    private void Export(string path)
    {
        try
        {
            journal.Export(path);
            output.WriteLine($"Итоги сохранены в {path}");
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            output.WriteLine($"Ошибка экспорта: {e.Message}");
        }
    }

    private string Read(string prompt)
    {
        output.Write(prompt);
        return input.ReadLine()?.Trim() ?? throw new EndOfStreamException();
    }

    private int ReadInt(string prompt)
    {
        while (true)
        {
            if (int.TryParse(Read(prompt), out var value)) return value;
            output.WriteLine("Введите целое число.");
        }
    }
}
