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
                var localSettingsPath = Path.Combine(current.FullName, "src", "AIOutfitStylist.Api", "appsettings.Local.json");
                var connectionString = ReadDefaultConnection(localSettingsPath) ?? ReadDefaultConnection(appSettingsPath);

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    return connectionString;
                }
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("DefaultConnection was not found in src/AIOutfitStylist.Api/appsettings.json.");
    }

    private static string? ReadDefaultConnection(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);
        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) ||
            !connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection))
        {
            return null;
        }

        return defaultConnection.GetString();
    }
}
