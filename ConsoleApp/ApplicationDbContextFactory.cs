using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Contacts.Data;
internal class ApplicationDbContextFactory
: IDesignTimeDbContextFactory<AppDbContext>
{
    AppDbContext IDesignTimeDbContextFactory<AppDbContext>.CreateDbContext(
    string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration
        .GetConnectionString("DefaultConnection");
        builder.UseSqlite(connectionString);
        return new AppDbContext(builder.Options);
    }
}
