using BeFitAPI.Data;
using BeFitAPI.DTOs;
using BeFitAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController(ApplicationDbContext context) : ControllerBase
{
    // The sessions the controller works on - every query starts here.
    private IQueryable<Session> Sessions => context.Sessions;

    // GET: api/Sessions
    [HttpGet]
    public async Task<IEnumerable<ShowSession>> GetSessions()
    {
        return await Sessions
            .OrderByDescending(s => s.Start)
            .Select(s => new ShowSession(s.Id, s.Start, s.End, s.Exercises.Count))
            .ToListAsync();
    }

    // GET: api/Sessions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ShowSession>> GetSession(int id)
    {
        var session = await Sessions.Include(s => s.Exercises).FirstOrDefaultAsync(s => s.Id == id);
        return session == null ? NotFound() : new ShowSession(session);
    }

    // POST: api/Sessions
    [HttpPost]
    public async Task<ActionResult<ShowSession>> PostSession(CreateSession input)
    {
        var session = new Session { Start = input.Start!.Value, End = input.End!.Value };
        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSession), new { id = session.Id }, new ShowSession(session));
    }

    // PUT: api/Sessions/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSession(int id, CreateSession input)
    {
        var session = await Sessions.FirstOrDefaultAsync(s => s.Id == id);
        if (session == null)
        {
            return NotFound();
        }

        session.Start = input.Start!.Value;
        session.End = input.End!.Value;
        await context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Sessions/5 - together with its exercises (cascade delete).
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSession(int id)
    {
        var session = await Sessions.FirstOrDefaultAsync(s => s.Id == id);
        if (session == null)
        {
            return NotFound();
        }

        context.Sessions.Remove(session);
        await context.SaveChangesAsync();
        return NoContent();
    }
}