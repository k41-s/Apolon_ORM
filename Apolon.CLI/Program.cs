using Apolon.Core.ORM.Database;
using Apolon.Core.ORM.Data;
using Apolon.Core.ORM.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("--- Apolon CLI Migration Tool ---");

        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton(config);

        services.AddSingleton<DatabaseService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        string migrationsPath = Path.Combine(Directory.GetCurrentDirectory(), "Migrations");

        services.AddScoped<IMigrationRunner>(provider =>
            new MigrationRunner(
                provider.GetRequiredService<IUnitOfWork>(),
                migrationsPath
            )
        );

        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        if (args.Length == 0)
        {
            await ShowHelp();
            return;
        }

        var command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "create":
                    if (args.Length < 2) throw new ArgumentException("Error: Missing migration name. Usage: create <Name>");
                    await runner.CreateMigrationAsync(args[1]);
                    break;
                case "up":
                case "apply":
                    await runner.ApplyPendingMigrationsAsync();
                    break;
                case "down":
                case "rollback":
                    await runner.RollbackLastMigrationAsync();
                    break;
                default:
                    await ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nCOMMAND FAILED: {ex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }
    }

    private static Task ShowHelp()
    {
        Console.WriteLine("\nUsage: dotnet run [command] [options]");
        Console.WriteLine("Commands:");
        Console.WriteLine("  create <Name>  : Creates a new timestamped SQL migration file.");
        Console.WriteLine("  up / apply     : Applies all pending migrations.");
        Console.WriteLine("  down / rollback: Rolls back the last applied migration.");
        return Task.CompletedTask;
    }
}