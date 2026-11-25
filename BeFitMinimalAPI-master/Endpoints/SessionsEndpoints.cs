using BeFitMinimalAPI.Data;
using BeFitMinimalAPI.DTOs;
using BeFitMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BeFitMinimalAPI.Endpoints;

public static class SessionsEndpoints
{
    public static void MapSessionsEndpoints(this IEndpointRouteBuilder app)
    {
        var sessions = app.MapGroup("/api/Sessions").WithTags("Sessions");

        sessions.MapGet("/", async (ApplicationDbContext context) =>
            await context.Sessions
                .OrderByDescending(s => s.Start)
                .Select(s => new ShowSession(s.Id, s.Start, s.End, s.Exercises.Count))
                .ToListAsync());

        sessions.MapGet("/{id}", async (int id, ApplicationDbContext context) =>
            await context.Sessions.Include(s => s.Exercises).FirstOrDefaultAsync(s => s.Id == id) is Session session
                ? Results.Ok(new ShowSession(session))
                : Results.NotFound());

        sessions.MapPost("/", async (CreateSession input, ApplicationDbContext context) =>
        {
            var session = new Session { Start = input.Start!.Value, End = input.End!.Value };
            context.Sessions.Add(session);
            await context.SaveChangesAsync();

            return Results.Created($"/api/Sessions/{session.Id}", new ShowSession(session));
        });

        sessions.MapPut("/{id}", async (int id, CreateSession input, ApplicationDbContext context) =>
        {
            var session = await context.Sessions.FindAsync(id);
            if (session == null)
            {
                return Results.NotFound();
            }

            session.Start = input.Start!.Value;
            session.End = input.End!.Value;
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        // Together with its exercises (cascade delete).
        sessions.MapDelete("/{id}", async (int id, ApplicationDbContext context) =>
        {
            var session = await context.Sessions.FindAsync(id);
            if (session == null)
            {
                return Results.NotFound();
            }

            context.Sessions.Remove(session);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}