using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Apolon.Core.ORM
{
    public class DatabaseService : IDisposable
    {
        private readonly string _connectionString;
        private NpgsqlConnection? _connection;

        public DatabaseService(IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("ApolonDb");
            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("Connection string 'ApolonDb' not found in configuration.");
            }
            _connectionString = connStr;
        }

        private async Task<NpgsqlConnection> GetOpenConnectionAsync()
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
            }

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
            return _connection;
        }

        public async Task ExecuteDdlAsync(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return;

            Console.WriteLine($"\n--- Executing DDL ---\n{sql}\n---------------------");

            var connection = await GetOpenConnectionAsync();

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

            _connection?.Close();

            Console.WriteLine("\n--- Database Schema Initialization Complete! ---");
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
