namespace BeFitMinimalAPI.Models;

public class Session
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public List<Exercise> Exercises { get; set; } = [];
}