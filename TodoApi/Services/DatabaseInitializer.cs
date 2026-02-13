using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace TodoApi.Services
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TodoDb")
                ?? throw new InvalidOperationException("Connection string 'TodoDb' not found.");
        }

        public void Initialize()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Todos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL
                )
            ";
            command.ExecuteNonQuery();

            Console.WriteLine("Database initialized successfully");
        }
    }
}