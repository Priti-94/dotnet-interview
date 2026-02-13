using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests
{
    public class DatabaseInitializerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly IConfiguration _config;

        public DatabaseInitializerTests()
        {
            // Use a shared in-memory SQLite DB for the test class lifetime
            _connection = new SqliteConnection("Data Source=TodoMemoryDbInit;Mode=Memory;Cache=Shared");
            _connection.Open();

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                        new KeyValuePair<string, string?>("ConnectionStrings:TodoDb", "Data Source=TodoMemoryDbInit;Mode=Memory;Cache=Shared")
                })
                .Build();
        }

        [Fact]
        public void Initialize_CreatesTableWithoutError()
        {
            var initializer = new DatabaseInitializer(_config);
            var ex = Record.Exception(() => initializer.Initialize());
            Assert.Null(ex);

            // Table should exist in the shared in-memory database
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Todos';";
            using var reader = command.ExecuteReader();
            Assert.True(reader.Read());
            Assert.Equal("Todos", reader.GetString(0));
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
