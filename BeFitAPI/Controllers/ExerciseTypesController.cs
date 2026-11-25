using BeFitAPI.Data;
using BeFitAPI.DTOs;
using BeFitAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExerciseTypesController(ApplicationDbContext context) : ControllerBase
{
    // GET: api/ExerciseTypes
    [HttpGet]
    public async Task<IEnumerable<ShowExerciseType>> GetExerciseTypes()
    {
        return await context.ExerciseTypes
            .OrderBy(t => t.Name)
            .Select(t => new ShowExerciseType(t.Id, t.Name))
            .ToListAsync();
    }

    // GET: api/ExerciseTypes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ShowExerciseType>> GetExerciseType(int id)
    {
        var type = await context.ExerciseTypes.FindAsync(id);
        return type == null ? NotFound() : new ShowExerciseType(type);
    }

    // POST: api/ExerciseTypes
    [HttpPost]
    public async Task<ActionResult<ShowExerciseType>> PostExerciseType(CreateExerciseType input)
    {
        var type = new ExerciseType { Name = input.Name };
        context.ExerciseTypes.Add(type);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExerciseType), new { id = type.Id }, new ShowExerciseType(type));
    }

    // PUT: api/ExerciseTypes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutExerciseType(int id, CreateExerciseType input)
    {
        var type = await context.ExerciseTypes.FindAsync(id);
        if (type == null)
        {
            return NotFound();
        }

        type.Name = input.Name;
        await context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/ExerciseTypes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExerciseType(int id)
    {
        var type = await context.ExerciseTypes.FindAsync(id);
        if (type == null)
        {
            return NotFound();
        }

        // The database refuses to delete a type used by an exercise (DeleteBehavior.Restrict) - 409 instead of an error.
        if (await context.Exercises.AnyAsync(e => e.ExerciseTypeId == id))
        {
            return Conflict("Tego typu nie można usunąć - są ćwiczenia, które go używają.");
        }

        context.ExerciseTypes.Remove(type);
        await context.SaveChangesAsync();
        return NoContent();
    }
}