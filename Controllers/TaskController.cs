using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IValidator<TaskItem> _taskValidator;

        public TaskController(ITaskService taskService, IValidator<TaskItem> taskValidator)
        {
            _taskService = taskService;
            _taskValidator = taskValidator;
        }

        [HttpGet]
        public IActionResult GetTasks() => Ok(_taskService.GetAllTasks());

        [HttpPost]
        public IActionResult AddTask([FromBody] TaskItem task)
        {
            var validationResult = _taskValidator.Validate(task);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var createdTask = _taskService.AddTask(task);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(Guid id, [FromBody] TaskItem task)
        {
            if (id != task.Id)
                return BadRequest("Task ID mismatch.");

            var validationResult = _taskValidator.Validate(task);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            return Ok(_taskService.UpdateTask(task));
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteTask(Guid id)
        {
            return _taskService.DeleteTask(id) ? Ok() : BadRequest("Task not found or not completed.");
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(Guid id)
        {
            var task = _taskService.GetTaskById(id);
            if (task == null)
                return NotFound("Task not found.");
            return Ok(task);
        }
    }
}
