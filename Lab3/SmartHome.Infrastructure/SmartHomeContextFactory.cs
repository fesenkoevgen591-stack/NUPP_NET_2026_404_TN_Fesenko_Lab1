
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartHome.Infrastructure;

public class SmartHomeContextFactory
    : IDesignTimeDbContextFactory<SmartHomeContext>
{
    public SmartHomeContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SmartHomeContext>()
            .UseSqlite("Data Source=SmartHome.db")
            .Options;

        return new SmartHomeContext(options);
    }
}
