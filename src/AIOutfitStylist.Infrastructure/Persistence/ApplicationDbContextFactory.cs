using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace AIOutfitStylist.Infrastructure.Persistence;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = ReadConnectionString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ApplicationDbContext(options);
    }

    private static string ReadConnectionString()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            var appSettingsPath = Path.Combine(current.FullName, "src", "AIOutfitStylist.Api", "appsettings.json");
            if (File.Exists(appSettingsPath))
            {
                using var stream = File.OpenRead(appSettingsPath);
                using var document = JsonDocument.Parse(stream);
                var connectionString = document.RootElement
                    .GetProperty("ConnectionStrings")
                    .GetProperty("DefaultConnection")
                    .GetString();

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    return connectionString;
                }
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("DefaultConnection was not found in src/AIOutfitStylist.Api/appsettings.json.");
    }
}
