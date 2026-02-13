using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService
    {
        private readonly string _connectionString;

        public TodoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TodoDb")
                ?? throw new InvalidOperationException("Connection string 'TodoDb' not found.");
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

           await using var command = connection.CreateCommand();
           

            command.CommandText = @"
                INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                VALUES (@Title, @Description, @IsCompleted, @CreatedAt);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@Title", todo.Title);
            command.Parameters.AddWithValue("@Description", todo.Description ?? string.Empty);
            command.Parameters.AddWithValue("@IsCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow.ToString("o"));


            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            todo.Id = id;
            todo.CreatedAt = DateTime.UtcNow;
            return todo;
        }

        public  async Task<List<Todo>> GetAllTodosAsync()
        {
            var todos = new List<Todo>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Todos";

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                todos.Add(new Todo
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    IsCompleted = reader.GetInt32(3) == 1,
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                });
            }

            return todos;
        }

        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Todos WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Todo
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    IsCompleted = reader.GetInt32(3) == 1,
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                };
            }

            return null;
        }

        public  async Task<Todo?> UpdateTodoAsync(int id, Todo todo)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Todos
                SET Title = @Title, Description = @Description, IsCompleted = @IsCompleted
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Title", todo.Title);
            command.Parameters.AddWithValue("@Description", todo.Description ?? string.Empty);
            command.Parameters.AddWithValue("@IsCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected=await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                todo.Id = id;
                return todo;
            }
            return null;
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Todos WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
