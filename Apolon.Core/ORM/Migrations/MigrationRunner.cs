using Apolon.Core.Models;
using Apolon.Core.ORM.Data;
using System.Text.RegularExpressions;

namespace Apolon.Core.ORM.Migrations
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly IUnitOfWork _uow;
        private readonly string _migrationsDirectory;

        public MigrationRunner(IUnitOfWork uow, string migrationsDirectory)
        {
            _uow = uow;
            _migrationsDirectory = migrationsDirectory;
        }

        public async Task CreateMigrationAsync(string migrationName)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var safeName = Regex.Replace(migrationName.Trim(), @"\s+", "_");
            var fileName = $"{timestamp}_{safeName}.sql";
            var filePath = Path.Combine(_migrationsDirectory, fileName);

            var template =
$@"-- MIGRATION: {fileName}

-- UP
-- Replace this with your APPLY logic (e.g., CREATE TABLE, ALTER TABLE ADD COLUMN...)


-- DOWN
-- Replace this with your REVERT logic (e.g., DROP TABLE, ALTER TABLE DROP COLUMN...)

";

            if (!Directory.Exists(_migrationsDirectory))
            {
                Directory.CreateDirectory(_migrationsDirectory);
            }

            await File.WriteAllTextAsync(filePath, template);
            Console.WriteLine($"Created migration: {fileName}");
            Console.WriteLine($"Location: {filePath}");
        }

        public async Task ApplyPendingMigrationsAsync()
        {
            await EnsureSchemaVersionTableExistsAsync();

            var appliedMigrations = (await _uow.Repository<SchemaVersion>().GetAllAsync())
                                    .Select(x => x.Version)
                                    .ToHashSet();

            var files = Directory.GetFiles(_migrationsDirectory, "*.sql")
                                 .OrderBy(f => f)
                                 .ToList();

            int count = 0;
            foreach (var file in files)
            {
                var version = Path.GetFileNameWithoutExtension(file);

                if (appliedMigrations.Contains(version)) continue;

                Console.WriteLine($"Applying: {version}");

                var sqlContent = await File.ReadAllTextAsync(file);
                var upSql = ParseSection(sqlContent, "UP");

                if (string.IsNullOrWhiteSpace(upSql))
                {
                    Console.WriteLine($"Skipping {version}: No UP section found. File must contain '-- UP'.");
                    continue;
                }

                try
                {
                    await _uow.BeginTransactionAsync();

                    await _uow.ExecuteRawSqlAsync(upSql);

                    var schemaVersion = new SchemaVersion
                    {
                        Version = version,
                        AppliedOn = DateTime.UtcNow
                    };
                    await _uow.Repository<SchemaVersion>().AddAsync(schemaVersion);

                    await _uow.CommitAsync();
                    Console.WriteLine($"Applied and tracked: {version}");
                    count++;
                }
                catch (Exception ex)
                {
                    await _uow.RollbackAsync();
                    Console.WriteLine($"FAILED: Migration {version} failed. Transaction rolled back.");
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }

            if (count == 0) 
                Console.WriteLine("Database schema is up to date.");
        }

        public async Task RollbackLastMigrationAsync()
        {
            await EnsureSchemaVersionTableExistsAsync();

            var lastMigration = (await _uow.Repository<SchemaVersion>().GetAllAsync())
                                .OrderByDescending(x => x.AppliedOn)
                                .FirstOrDefault();

            if (lastMigration == null)
            {
                Console.WriteLine("No migrations found in the database to rollback.");
                return;
            }

            Console.WriteLine($"Rolling back: {lastMigration.Version}");

            var filePath = Directory.GetFiles(_migrationsDirectory, $"{lastMigration.Version}.sql").FirstOrDefault();
            if (filePath == null)
            {
                throw new FileNotFoundException($"Could not find file for version {lastMigration.Version} on disk.");
            }

            var sqlContent = await File.ReadAllTextAsync(filePath);
            var downSql = ParseSection(sqlContent, "DOWN");

            if (string.IsNullOrWhiteSpace(downSql))
            {
                throw new InvalidOperationException($"Migration {lastMigration.Version} has no '-- DOWN' section. Rollback aborted.");
            }

            try
            {
                await _uow.BeginTransactionAsync();

                await _uow.ExecuteRawSqlAsync(downSql);

                await _uow.Repository<SchemaVersion>().DeleteAsync(lastMigration.Id);

                await _uow.CommitAsync();
                Console.WriteLine($"Successfully rolled back and untracked: {lastMigration.Version}");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                Console.WriteLine($"FAILED: Rollback of {lastMigration.Version} failed. Transaction rolled back.");
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private async Task EnsureSchemaVersionTableExistsAsync()
        {
            var checkSql =
                @"CREATE TABLE IF NOT EXISTS ""__schema_version"" (
                    id SERIAL PRIMARY KEY,
                    version VARCHAR(255) NOT NULL UNIQUE,
                    applied_on TIMESTAMP NOT NULL
                );";

            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.ExecuteRawSqlAsync(checkSql);
                await _uow.CommitAsync();
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                Console.WriteLine($"Warning: Failed to ensure tracking table exists: {ex.Message}");
            }
        }

        private string ParseSection(string sql, string sectionName)
        {
            var pattern = $@"--\s*{sectionName}\s+(.*?)(?=--\s*(UP|DOWN|\s*)|$)";
            var match = Regex.Match(sql, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }
    }
}
