using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Apolon.Core.ORM
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("ApolonDb");
            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("Connection string 'ApolonDb' not found in configuration.");
            }
            _connectionString = connStr;
        }

        public async Task<NpgsqlConnection> GetNewOpenConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task ExecuteDdlAsync(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return;

            Console.WriteLine($"\n--- Executing DDL ---\n{sql}\n---------------------");

            await using (var connection = await GetNewOpenConnectionAsync())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                try
                {
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine("DDL executed successfully.");
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine($"Error executing DDL: {ex.Message}");
                    Console.WriteLine($"SQL: {sql}");
                }
            }
        }

        public async Task EnsureSchemaCreatedAsync(params Type[] entityTypes)
        {
            var createTableSqlCommands = new List<string>();
            var alterTableSqlCommands = new List<string>();

            foreach (var type in entityTypes)
            {
                var metadata = ModelParser.GetMetadata(type);

                createTableSqlCommands.Add(PostgresDdlGenerator.GenerateCreateTableSql(metadata));

                var fkSql = PostgresDdlGenerator.GenerateForeignKeySql(metadata);
                if (!string.IsNullOrWhiteSpace(fkSql))
                    alterTableSqlCommands.Add(fkSql);
            }

            foreach (var sql in createTableSqlCommands)
            {
                await ExecuteDdlAsync(sql);
            }

            foreach (var sql in alterTableSqlCommands)
            {
                await ExecuteDdlAsync(sql);
            }

            Console.WriteLine("\n--- Database Schema Initialization Complete! ---");
        }
    }
}
