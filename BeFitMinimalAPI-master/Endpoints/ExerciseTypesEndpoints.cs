using BeFitMinimalAPI.Data;
using BeFitMinimalAPI.DTOs;
using BeFitMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BeFitMinimalAPI.Endpoints;

public static class ExerciseTypesEndpoints
{
    public static void MapExerciseTypesEndpoints(this IEndpointRouteBuilder app)
    {
        var types = app.MapGroup("/api/ExerciseTypes").WithTags("ExerciseTypes");

        types.MapGet("/", async (ApplicationDbContext context) =>
            await context.ExerciseTypes
                .OrderBy(t => t.Name)
                .Select(t => new ShowExerciseType(t.Id, t.Name))
                .ToListAsync());

        types.MapGet("/{id}", async (int id, ApplicationDbContext context) =>
            await context.ExerciseTypes.FindAsync(id) is ExerciseType type
                ? Results.Ok(new ShowExerciseType(type))
                : Results.NotFound());

        types.MapPost("/", async (CreateExerciseType input, ApplicationDbContext context) =>
        {
            var type = new ExerciseType { Name = input.Name };
            context.ExerciseTypes.Add(type);
            await context.SaveChangesAsync();

            return Results.Created($"/api/ExerciseTypes/{type.Id}", new ShowExerciseType(type));
        });

        types.MapPut("/{id}", async (int id, CreateExerciseType input, ApplicationDbContext context) =>
        {
            var type = await context.ExerciseTypes.FindAsync(id);
            if (type == null)
            {
                return Results.NotFound();
            }

            type.Name = input.Name;
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        types.MapDelete("/{id}", async (int id, ApplicationDbContext context) =>
        {
            var type = await context.ExerciseTypes.FindAsync(id);
            if (type == null)
            {
                return Results.NotFound();
            }

            // The database refuses to delete a type used by an exercise (DeleteBehavior.Restrict) - 409 instead of an error.
            if (await context.Exercises.AnyAsync(e => e.ExerciseTypeId == id))
            {
                return Results.Conflict("Tego typu nie można usunąć - są ćwiczenia, które go używają.");
            }

            context.ExerciseTypes.Remove(type);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}