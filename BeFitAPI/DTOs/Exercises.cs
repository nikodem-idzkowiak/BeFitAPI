using System.ComponentModel.DataAnnotations;
using BeFitAPI.Models;

namespace BeFitAPI.DTOs;

public class CreateExercise
{
    [Required(ErrorMessage = "Wybierz typ ćwiczenia.")]
    public int? ExerciseTypeId { get; set; }

    [Required(ErrorMessage = "Wybierz sesję.")]
    public int? SessionId { get; set; }

    [Required(ErrorMessage = "Podaj ciężar.")]
    [Range(0, 1000, ErrorMessage = "Ciężar musi wynosić od {1} do {2} kg.")]
    public int? Weight { get; set; }

    [Required(ErrorMessage = "Podaj liczbę powtórzeń.")]
    [Range(1, 1000, ErrorMessage = "Liczba powtórzeń musi wynosić od {1} do {2}.")]
    public int? NumOfReps { get; set; }

    [Required(ErrorMessage = "Podaj liczbę serii.")]
    [Range(1, 100, ErrorMessage = "Liczba serii musi wynosić od {1} do {2}.")]
    public int? NumOfSeries { get; set; }

    [MaxLength(200, ErrorMessage = "Komentarz może mieć najwyżej 200 znaków.")]
    public string? Comment { get; set; }
}

public record ShowExercise(int Id, int ExerciseTypeId, string ExerciseTypeName, int SessionId,
    int Weight, int NumOfReps, int NumOfSeries, string? Comment)
{
    public ShowExercise(Exercise e) : this(e.Id, e.ExerciseTypeId, e.ExerciseType?.Name ?? "", e.SessionId,
        e.Weight, e.NumOfReps, e.NumOfSeries, e.Comment) { }
}

// Properties instead of a constructor - EF Core translates an object initializer after GroupBy into SQL.
public record ShowStats
{
    public string ExerciseTypeName { get; init; } = "";
    public int Count { get; init; }
    public int TotalReps { get; init; }
    public double AverageWeight { get; init; }
    public int MaxWeight { get; init; }
}