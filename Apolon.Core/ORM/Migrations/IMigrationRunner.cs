
namespace Apolon.Core.ORM.Migrations
{
    public interface IMigrationRunner
    {
        Task CreateMigrationAsync(string migrationName);
        Task ApplyPendingMigrationsAsync();
        Task RollbackLastMigrationAsync();
    }
}
