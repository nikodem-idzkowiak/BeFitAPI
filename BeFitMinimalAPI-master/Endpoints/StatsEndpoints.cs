using BeFitMinimalAPI.Data;
using BeFitMinimalAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BeFitMinimalAPI.Endpoints;

public static class StatsEndpoints
{
    // GET /api/Stats?weeks=4 - for every exercise type of the exercises from the last weeks.
    public static void MapStatsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Stats", async (ApplicationDbContext context, int weeks = 4) =>
        {
            if (weeks is < 1 or > 52)
            {
                return Results.BadRequest("Okres musi wynosić od 1 do 52 tygodni.");
            }

            var since = DateTime.Today.AddDays(-7 * weeks);
            return Results.Ok(await context.Exercises
                .Where(e => e.Session!.Start >= since)
                .GroupBy(e => e.ExerciseType!.Name)
                .Select(g => new ShowStats
                {
                    ExerciseTypeName = g.Key,
                    Count = g.Count(),
                    TotalReps = g.Sum(e => e.NumOfReps * e.NumOfSeries),
                    AverageWeight = g.Average(e => e.Weight),
                    MaxWeight = g.Max(e => e.Weight)
                })
                .OrderBy(s => s.ExerciseTypeName)
                .ToListAsync());
        }).WithTags("Stats");
    }
}
