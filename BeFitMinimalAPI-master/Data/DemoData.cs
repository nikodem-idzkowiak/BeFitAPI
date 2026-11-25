using Microsoft.EntityFrameworkCore.Migrations;

namespace BeFit.Data;

// Six weeks of training sessions - something to work on, especially in the statistics.
// The dates are counted back from the moment the migration runs (SQLite's datetime('now')), so the data is always recent.
// Two people train: sessions 1000-1999 every other day, sessions 2000-2999 every third day with 20% heavier weights.
// (Lesson 06 gives them owners - the accounts anna@befit.pl and jan@befit.pl - in DemoOwners.cs.)
public static class DemoData
{
    // Exercise types from the HasData seed: 1 squat, 2 deadlift, 3 bench press, 4 barbell row, 5 pull-up, 6 overhead press.
    private static readonly int[][] Plans = [[1, 3, 4], [2, 6, 5]];
    private static readonly int[] BaseWeight = [0, 60, 80, 50, 40, 0, 30];

    public static void Up(MigrationBuilder migrationBuilder)
    {
        AddSessions(migrationBuilder, firstSessionId: 1000, everyDays: 2, strength: 1.0);
        AddSessions(migrationBuilder, firstSessionId: 2000, everyDays: 3, strength: 1.2);
    }

    public static void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM Exercises WHERE SessionId BETWEEN 1000 AND 2999;");
        migrationBuilder.Sql("DELETE FROM Sessions WHERE Id BETWEEN 1000 AND 2999;");
    }

    private static void AddSessions(MigrationBuilder migrationBuilder, int firstSessionId, int everyDays, double strength)
    {
        var sessionId = firstSessionId;
        for (var daysAgo = 41; daysAgo >= 1; daysAgo -= everyDays, sessionId++)
        {
            var start = 17 * 60 + daysAgo % 3 * 30;        // 17:00, 17:30 or 18:00
            var end = start + 60 + daysAgo % 4 * 10;       // 60-90 minutes
            migrationBuilder.Sql($"""
                INSERT INTO Sessions (Id, Start, End) VALUES ({sessionId},
                    datetime('now', 'localtime', 'start of day', '-{daysAgo} days', '+{start} minutes'),
                    datetime('now', 'localtime', 'start of day', '-{daysAgo} days', '+{end} minutes'));
                """);

            foreach (var typeId in Plans[sessionId % 2])
            {
                // A little heavier every week: the oldest session is 41 days ago.
                var weight = (int)Math.Round((BaseWeight[typeId] + (41 - daysAgo) / 3.0) * strength);
                var heavy = typeId <= 2;
                var comment = daysAgo % 7 == 0 ? "'Dobra forma'" : "NULL";
                migrationBuilder.Sql($"""
                    INSERT INTO Exercises (Weight, NumOfReps, NumOfSeries, Comment, ExerciseTypeId, SessionId)
                    VALUES ({(typeId == 5 ? 0 : weight)}, {(heavy ? 5 : 8)}, {(heavy ? 5 : 3 + daysAgo % 2)}, {comment}, {typeId}, {sessionId});
                    """);
            }
        }
    }
}
