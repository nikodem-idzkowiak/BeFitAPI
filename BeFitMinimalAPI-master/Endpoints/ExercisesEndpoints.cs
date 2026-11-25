using BeFitMinimalAPI.Data;
using BeFitMinimalAPI.DTOs;
using BeFitMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BeFitMinimalAPI.Endpoints;

public static class ExercisesEndpoints
{
    public static void MapExercisesEndpoints(this IEndpointRouteBuilder app)
    {
        var exercises = app.MapGroup("/api/Exercises").WithTags("Exercises");

        // GET /api/Exercises?sessionId=5
        exercises.MapGet("/", async (int? sessionId, ApplicationDbContext context) =>
        {
            var query = context.Exercises.Include(e => e.ExerciseType).AsQueryable();
            if (sessionId != null)
            {
                query = query.Where(e => e.SessionId == sessionId);
            }
            return await query.OrderBy(e => e.Id).Select(e => new ShowExercise(e)).ToListAsync();
        });

        exercises.MapGet("/{id}", async (int id, ApplicationDbContext context) =>
            await context.Exercises.Include(e => e.ExerciseType).FirstOrDefaultAsync(e => e.Id == id) is Exercise exercise
                ? Results.Ok(new ShowExercise(exercise))
                : Results.NotFound());

        exercises.MapPost("/", async (CreateExercise input, ApplicationDbContext context) =>
        {
            if (await CheckReferencesAsync(input, context) is { } problem)
            {
                return problem;
            }

            var exercise = new Exercise();
            Copy(input, exercise);
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();
            await context.Entry(exercise).Reference(e => e.ExerciseType).LoadAsync();

            return Results.Created($"/api/Exercises/{exercise.Id}", new ShowExercise(exercise));
        });

        exercises.MapPut("/{id}", async (int id, CreateExercise input, ApplicationDbContext context) =>
        {
            var exercise = await context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                return Results.NotFound();
            }
            if (await CheckReferencesAsync(input, context) is { } problem)
            {
                return problem;
            }

            Copy(input, exercise);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        exercises.MapDelete("/{id}", async (int id, ApplicationDbContext context) =>
        {
            var exercise = await context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                return Results.NotFound();
            }

            context.Exercises.Remove(exercise);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    // The session and the exercise type must exist - otherwise 400 in the same format as the validation errors.
    private static async Task<IResult?> CheckReferencesAsync(CreateExercise input, ApplicationDbContext context)
    {
        var errors = new Dictionary<string, string[]>();
        if (!await context.Sessions.AnyAsync(s => s.Id == input.SessionId))
        {
            errors[nameof(input.SessionId)] = ["Nie ma takiej sesji."];
        }
        if (!await context.ExerciseTypes.AnyAsync(t => t.Id == input.ExerciseTypeId))
        {
            errors[nameof(input.ExerciseTypeId)] = ["Nie ma takiego typu ćwiczenia."];
        }
        return errors.Count == 0 ? null : Results.ValidationProblem(errors);
    }

    private static void Copy(CreateExercise input, Exercise exercise)
    {
        exercise.ExerciseTypeId = input.ExerciseTypeId!.Value;
        exercise.SessionId = input.SessionId!.Value;
        exercise.Weight = input.Weight!.Value;
        exercise.NumOfReps = input.NumOfReps!.Value;
        exercise.NumOfSeries = input.NumOfSeries!.Value;
        exercise.Comment = input.Comment;
    }
}
