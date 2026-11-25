using BeFitMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BeFitMinimalAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ExerciseType> ExerciseTypes { get; set; } = default!;
    public DbSet<Session> Sessions { get; set; } = default!;
    public DbSet<Exercise> Exercises { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ExerciseType>().Property(t => t.Name).HasMaxLength(100);

        // An exercise type that is used by an exercise can't be deleted.
        builder.Entity<Exercise>()
            .HasOne(e => e.ExerciseType)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ExerciseType>().HasData(
            new ExerciseType { Id = 1, Name = "Przysiad" },
            new ExerciseType { Id = 2, Name = "Martwy ciąg" },
            new ExerciseType { Id = 3, Name = "Wyciskanie sztangi leżąc" },
            new ExerciseType { Id = 4, Name = "Wiosłowanie sztangą" },
            new ExerciseType { Id = 5, Name = "Podciąganie na drążku" },
            new ExerciseType { Id = 6, Name = "Wyciskanie żołnierskie" });
    }
}