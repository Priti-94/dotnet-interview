using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;
        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpPost("createTodo")]
        public IActionResult CreateTodo([FromBody] Todo todo)
        {
            try
            {
                var result = _todoService.CreateTodoAsync(todo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("getTodo")]
        public IActionResult GetTodo([FromBody] GetTodoRequest request)
        {
            try
            {
               
                if (request.Id.HasValue)
                {
                    var todo = _todoService.GetTodoByIdAsync(request.Id.Value);
                    if (todo == null)
                    {
                        return NotFound();
                    }
                    return Ok(todo);
                }
                else
                {
                    var todos = _todoService.GetAllTodosAsync();
                    return Ok(todos);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("updateTodo")]
        public IActionResult UpdateTodo([FromBody] UpdateTodoRequest request)
        {
            try
            {
              
                var existingTodo = _todoService.GetTodoByIdAsync(request.Id);
                if (existingTodo == null)
                {
                    return NotFound();
                }

                var todo = new Todo
                {
                    Title = request.Title,
                    Description = request.Description,
                    IsCompleted = request.IsCompleted
                };

                var result = _todoService.UpdateTodoAsync(request.Id, todo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deleteTodo")]
        public IActionResult DeleteTodo([FromBody] DeleteTodoRequest request)
        {
            try
            {
               
                var result = _todoService.DeleteTodoAsync(request.Id);
                if (result.Result)
                {
                    return Ok(new { message = "Todo deleted successfully" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class GetTodoRequest
    {
        public int? Id { get; set; }
    }

    public class UpdateTodoRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public bool IsCompleted { get; set; }
    }

    public class DeleteTodoRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
