using Xunit;
using TodoApi.Services;
using TodoApi.Models;
using TodoApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;

namespace TodoApi.Tests;

public class UnitTest1
{
    private readonly SqliteConnection _connection;
    private readonly TodoService _service;

    public UnitTest1()
    {
        // Use shared in-memory SQLite DB for the test class lifetime
        _connection = new SqliteConnection("Data Source=TodoMemoryDb;Mode=Memory;Cache=Shared");
        _connection.Open();

        // Initialize schema
        using var command = _connection.CreateCommand();
        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Todos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL
                )";
        command.ExecuteNonQuery();

        // Setup configuration to use the same shared in-memory DB
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                    new KeyValuePair<string, string?>("ConnectionStrings:TodoDb", "Data Source=TodoMemoryDb;Mode=Memory;Cache=Shared")
            })
            .Build();

        _service = new TodoService(config);
    }
    /*private TodoService GetServiceWithDb()
    {
        var config = TestConfigHelper.GetInMemoryConfig();
        var service = new TodoService(config);

        // Initialize the database schema
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(config.GetConnectionString("TodoDb"));
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
        return service;
    }*/

    [Fact]
    public async Task CreateTask()
    {
       
        var todo = new Todo { Title = "Test", Description = "Desc", IsCompleted = false };
        var result = await _service.CreateTodoAsync(todo);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }
    [Fact]
    public async Task GetAllTodosAsync()
    {
       
        await _service.CreateTodoAsync(new Todo { Title = "A", Description = "B", IsCompleted = false });
        var todos = await _service.GetAllTodosAsync();
        Assert.NotEmpty(todos);
    }

    [Fact]
    public async Task GetTodoByIdAsync()
    {
   
        var todo = await _service.CreateTodoAsync(new Todo { Title = "A", Description = "B", IsCompleted = false });
        var found = await _service.GetTodoByIdAsync(todo.Id);
        Assert.NotNull(found);
        Assert.Equal(todo.Title, found.Title);
    }
    [Fact]
    public async Task UpdateTodoAsync()
    {
      
        var todo = await _service.CreateTodoAsync(new Todo { Title = "A", Description = "B", IsCompleted = false });
        todo.Title = "Updated";
        var updated = await _service.UpdateTodoAsync(todo.Id, todo);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Title);
    }

    [Fact]
    public async Task DeleteTodoAsync()
    {
        
        var todo = await _service.CreateTodoAsync(new Todo { Title = "A", Description = "B", IsCompleted = false });
        var deleted = await _service.DeleteTodoAsync(todo.Id);
        Assert.True(deleted);
        var found = await _service.GetTodoByIdAsync(todo.Id);
        Assert.Null(found);




    }
}