using Microsoft.EntityFrameworkCore;

namespace ScratchConsole.Db;

public class ConsoleDbContext(DbContextOptions options) : DbContext(options)
{
    public required DbSet<Person> People { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsoleDbContext).Assembly);
    }
}