using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScratchConsole.Db;
using ScratchConsole.Infrastructure;

namespace ScratchConsole.Services;

internal class ConsoleRunner(
    ILogger<ConsoleRunner> logger,
    ConsoleDbContext dbContext,
    Settings settings)
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        logger.LogInformation("Settings - '{StringSetting}': {NumberSetting}", settings.StringSetting,
            settings.NumberSetting);

        // Console work goes here...
        // await dbContext.Database.EnsureCreatedAsync();
        var existingPeople = await dbContext.People.AsNoTracking().ToArrayAsync(cancellationToken);

        if (existingPeople.Length > 0) dbContext.People.RemoveRange(existingPeople);

        await dbContext.People.AddRangeAsync(
            new[]
            {
                new Person(Guid.Parse("f59b2474-7c3e-4610-8310-d44eb01b8ff8"), "Joe", "Blogs"),
                new Person(Guid.Parse("25d8fcf7-b361-45ef-a502-5685c04d0166"), "Jane", "Doe"),
            }, cancellationToken);

        var p = await dbContext.FindAsync<Person>(
            Guid.Parse("f59b2474-7c3e-4610-8310-d44eb01b8ff8"),
            cancellationToken);
        p?.SetFirstName("Joseph");

        logger.LogInformation("{@Person}", p);

        var savedCount = await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{PeopleCount} records affected", savedCount);
    }
}