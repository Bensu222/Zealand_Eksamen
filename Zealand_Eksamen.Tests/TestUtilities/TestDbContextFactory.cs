using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;

namespace Zealand_Eksamen.Tests.TestUtilities;

public static class TestDbContextFactory
{
    public static SqliteConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        return connection;
    }

    public static ApplicationDbContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}