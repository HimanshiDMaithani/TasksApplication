using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace TaskManagementSystem.Tests
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IValidator<TaskItem>> _mockTaskValidator;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockTaskValidator = new Mock<IValidator<TaskItem>>();
            _controller = new TaskController(_mockTaskService.Object, _mockTaskValidator.Object);
        }

        #region GetTasks

        [Fact]
        public void GetTasks_ReturnsOkResult_WithListOfTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), Name = "Task 1", Priority = 1, Status = "NotStarted" },
                new TaskItem { Id = Guid.NewGuid(), Name = "Task 2", Priority = 2, Status = "InProgress" }
            };

            _mockTaskService.Setup(service => service.GetAllTasks()).Returns(tasks);

            // Act
            var result = _controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<TaskItem>>(okResult.Value);
            Assert.Equal(tasks.Count, returnValue.Count);
        }

        #endregion

        #region AddTask

        [Fact]
        public void AddTask_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Name = "New Task", Priority = 1, Status = "NotStarted" };
            var validationResult = new FluentValidation.Results.ValidationResult(
                new[] { new FluentValidation.Results.ValidationFailure("Name", "Name is required.") }
            );
            _mockTaskValidator.Setup(v => v.Validate(It.IsAny<TaskItem>())).Returns(validationResult);

            // Act
            var result = _controller.AddTask(task);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<List<FluentValidation.Results.ValidationFailure>>(badRequestResult.Value);
            Assert.Single(errors);
            Assert.Equal("Name", errors[0].PropertyName);
        }

        [Fact]
        public void AddTask_ReturnsCreatedAtAction_WhenValidationSucceeds()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Name = "New Task", Priority = 1, Status = "NotStarted" };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _mockTaskValidator.Setup(v => v.Validate(It.IsAny<TaskItem>())).Returns(validationResult);
            _mockTaskService.Setup(service => service.AddTask(It.IsAny<TaskItem>())).Returns(task);

            // Act
            var result = _controller.AddTask(task);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(TaskController.GetTaskById), createdAtActionResult.ActionName);
            Assert.Equal(task.Id, ((TaskItem)createdAtActionResult.Value).Id);
        }

        #endregion

        #region UpdateTask

        [Fact]
        public void UpdateTask_ReturnsBadRequest_WhenTaskIdMismatch()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = Guid.NewGuid(), Name = "Updated Task", Priority = 1, Status = "InProgress" };

            // Act
            var result = _controller.UpdateTask(taskId, task);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Task ID mismatch.", badRequestResult.Value);
        }

        [Fact]
        public void UpdateTask_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Name = "Updated Task", Priority = 1, Status = "InProgress" };
            var validationResult = new FluentValidation.Results.ValidationResult(
                new[] { new FluentValidation.Results.ValidationFailure("Priority", "Priority must be greater than 0.") }
            );
            _mockTaskValidator.Setup(v => v.Validate(It.IsAny<TaskItem>())).Returns(validationResult);

            // Act
            var result = _controller.UpdateTask(taskId, task);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<List<FluentValidation.Results.ValidationFailure>>(badRequestResult.Value);
            Assert.Single(errors);
            Assert.Equal("Priority", errors[0].PropertyName);
        }

        [Fact]
        public void UpdateTask_ReturnsOkResult_WhenValidationSucceeds()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Name = "New Vlaid Task", Priority = 1, Status = "InProgress" };
            var validationResult = new FluentValidation.Results.ValidationResult();
            _mockTaskValidator.Setup(v => v.Validate(It.IsAny<TaskItem>())).Returns(validationResult);
            _mockTaskService.Setup(service => service.UpdateTask(It.IsAny<TaskItem>())).Returns(task);

            // Act
            var result = _controller.UpdateTask(taskId, task);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #region DeleteTask

        [Fact]
        public void DeleteTask_ReturnsOkResult_WhenTaskIsDeleted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(service => service.DeleteTask(taskId)).Returns(true);

            // Act
            var result = _controller.DeleteTask(taskId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteTask_ReturnsBadRequest_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(service => service.DeleteTask(taskId)).Returns(false);

            // Act
            var result = _controller.DeleteTask(taskId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Task not found or not completed.", badRequestResult.Value);
        }

        #endregion

        #region GetTaskById

        [Fact]
        public void GetTaskById_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(service => service.GetTaskById(taskId)).Returns((TaskItem)null);

            // Act
            var result = _controller.GetTaskById(taskId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetTaskById_ReturnsOkResult_WhenTaskExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Name = "Existing Task", Priority = 1, Status = "Completed" };
            _mockTaskService.Setup(service => service.GetTaskById(taskId)).Returns(task);

            // Act
            var result = _controller.GetTaskById(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TaskItem>(okResult.Value);
            Assert.Equal(taskId, returnValue.Id);
        }

        #endregion
    }
}
