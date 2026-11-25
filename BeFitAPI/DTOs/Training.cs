using System.ComponentModel.DataAnnotations;
using BeFitAPI.Models;

namespace BeFitAPI.DTOs;

public class CreateExerciseType
{
    [Required(ErrorMessage = "Podaj nazwę.")]
    [MaxLength(100, ErrorMessage = "Nazwa może mieć najwyżej 100 znaków.")]
    public string Name { get; set; } = "";
}

public record ShowExerciseType(int Id, string Name)
{
    public ShowExerciseType(ExerciseType type) : this(type.Id, type.Name) { }
}

public class CreateSession : IValidatableObject
{
    [Required(ErrorMessage = "Podaj początek sesji.")]
    public DateTime? Start { get; set; }

    [Required(ErrorMessage = "Podaj koniec sesji.")]
    public DateTime? End { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start is null || End is null)
        {
            yield break;
        }
        if (End <= Start)
        {
            yield return new ValidationResult("Koniec sesji musi być późniejszy niż jej początek.", [nameof(End)]);
        }
        else if (End - Start > TimeSpan.FromHours(4))
        {
            yield return new ValidationResult("Sesja może trwać najwyżej 4 godziny.", [nameof(End)]);
        }
    }
}

public record ShowSession(int Id, DateTime Start, DateTime End, int ExerciseCount)
{
    public ShowSession(Session session) : this(session.Id, session.Start, session.End, session.Exercises.Count) { }
}