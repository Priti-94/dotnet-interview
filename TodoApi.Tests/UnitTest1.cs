using Xunit;
using TodoApi.Services;
using TodoApi.Models;
using TodoApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TodoApi.Tests;

public class UnitTest1
{

    private TodoService GetServiceWithDb()
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
    }

    [Fact]
    public async Task CreateTask()
    {
        var service = GetServiceWithDb();
        var todo = new Todo { Title = "Test", Description = "Desc", IsCompleted = false };
        var result = await service.CreateTodoAsync(todo);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }
    [Fact]
    public async Task GetAllTodosAsync()
    {
        var service = GetServiceWithDb();
        await service.CreateTodoAsync(new Todo { Title = "A", Description = "B", IsCompleted = false });
        var todos = await service.GetAllTodosAsync();
        Assert.NotEmpty(todos);
    }

    [Fact]
    public async Task GetTodoByIdAsync()
    {
        var service = GetServiceWithDb();
        var todo = await service.CreateTodoAsync(new Todo { Title = "A", Description = "B", IsCompleted = false });
        var found = await service.GetTodoByIdAsync(todo.Id);
        Assert.NotNull(found);
        Assert.Equal(todo.Title, found.Title);
    }
    [Fact]
    public async Task UpdateTodoAsync()
    {
        var service = GetServiceWithDb();
        var todo = await service.CreateTodoAsync(new Todo { Title = "A", Description = "B", IsCompleted = false });
        todo.Title = "Updated";
        var updated = await service.UpdateTodoAsync(todo.Id, todo);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Title);
    }

    [Fact]
    public async Task DeleteTodoAsync()
    {
        var service = GetServiceWithDb();
        var todo = await service.CreateTodoAsync(new Todo { Title = "A", Description = "B", IsCompleted = false });
        var deleted = await service.DeleteTodoAsync(todo.Id);
        Assert.True(deleted);
        var found = await service.GetTodoByIdAsync(todo.Id);
        Assert.Null(found);




    }
}