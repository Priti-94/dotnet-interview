# Solution Documentation

**Candidate Name:** [Priti Tiwari]  
**Completion Date:** [13-02-2026]

---

# Refactoring Plan for TodoApi

Refactoring plan for the TodoApi project with explanations and actionable steps for each recommendation.

---

## 1. Use Parameterized Queries

All SQL commands in the codebase should use parameterized queries instead of string interpolation or concatenation. This prevents SQL injection attacks, improves security, and ensures correct handling of special characters in user input.  
**Solution:**  
- Replace all SQL command text that uses string interpolation (e.g., `$"SELECT * FROM Todos WHERE Id = {id}"`) with parameterized queries (e.g., `command.CommandText = "SELECT * FROM Todos WHERE Id = @Id"; command.Parameters.AddWithValue("@Id", id);`).  
- Review all data access code for compliance.

---

## 2. Implement Dependency Injection for Database Connection

The database connection string should not be hardcoded. Instead, it should be stored in `appsettings.json` and injected into services via the .NET Core dependency injection (DI) system. This improves flexibility, testability, and maintainability.  
**Solution:**  
- Store the connection string in `appsettings.json` under the `ConnectionStrings` section.
- Inject `IConfiguration` into services that require the connection string.
- Retrieve the connection string using `configuration.GetConnectionString("TodoDb")`.

---

## 3. Make Service Methods Asynchronous
 
All database operations should be implemented using async methods (`async`/`await` and ADO.NET async APIs). This allows the application to handle more concurrent requests efficiently and improves scalability, especially under load.  
**Solution:**  
- Change all service methods that interact with the database to their async equivalents (e.g., `OpenAsync`, `ExecuteReaderAsync`, etc.).
- Update method signatures to return `Task<T>` or `Task`.
- Update controller actions to call these async methods and use `await`.

---

## 4. Move Database Initialization to a Dedicated Service
 
Database initialization logic should be encapsulated in a dedicated singleton service (e.g., `DatabaseInitializer`). This separates concerns, improves maintainability, and allows for easier testing and extension (e.g., migrations).  
**Solution:**  
- Create a `DatabaseInitializer` class that handles schema creation.
- Register it as a singleton in the DI container.
- Call its `Initialize()` method at application startup.

---

## 5. Add Error Handling and Logging
 
Robust error handling and logging are essential for diagnosing issues and ensuring application stability. All service and controller methods should catch exceptions, log errors, and return appropriate HTTP responses.  
**Solution:**  
- Add try-catch blocks where appropriate.
- Use ASP.NET Core’s built-in logging (`ILogger<T>`) to log exceptions and important events.
- Return meaningful error messages and HTTP status codes to clients.

---

## 6. Use DTOs for API Communication

Data Transfer Objects (DTOs) should be used for all API request and response payloads. This decouples the API contract from the internal data model, prevents over-posting, and allows for more flexible API evolution.  
**Solution:**  
- Define DTO classes for create, update, and response operations.
- Map between DTOs and domain models in controller actions.
- Update controller signatures to use DTOs instead of domain models.

---

## 7. Validate Input Data

All incoming data should be validated using data annotations and model validation. This ensures data integrity and provides immediate feedback to API consumers when invalid data is submitted.  
**Solution:**  
- Add `[Required]`, `[StringLength]`, and other relevant data annotations to DTO properties.
- Rely on `[ApiController]` for automatic model validation and 400 Bad Request responses.
- Optionally, add custom validation logic as needed.

---

## 8. Register Services with Scoped Lifetime

Services that interact with the database (such as `TodoService`) should be registered with a scoped lifetime in the DI container. This ensures a new instance per HTTP request, which is the recommended practice for services that use database connections.  
**Solution:**  
- Register `TodoService` with `builder.Services.AddScoped<TodoService>();` in `Program.cs`.
- Register singleton services (like `DatabaseInitializer`) with `AddSingleton`.

---

## 9. Improve Separation of Concerns

The codebase should follow the principle of separation of concerns: controllers handle HTTP and API logic, services handle business logic, and data access is abstracted. This makes the code easier to maintain, test, and extend.  
**Solution:**  
- Ensure controllers do not contain business or data access logic.
- Move all business logic to services.
- Use DTOs for API boundaries.
- Keep data access logic encapsulated within services.

