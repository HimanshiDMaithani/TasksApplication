using FluentValidation.TestHelper;
using Moq;
using System;
using System.Collections.Generic;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using TaskManagementSystem.Validation;
using Xunit;

namespace TaskManagementSystem.Tests
{
    public class TaskValidatorTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly TaskValidator _validator;

        public TaskValidatorTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _validator = new TaskValidator(_taskServiceMock.Object);
        }

        // Arrange the data and validate priority greater than 0
        [Fact]
        public void Should_Fail_When_Priority_Is_Less_Than_1()
        {
            // Arrange
            var task = new TaskItem { Name = "Task1", Priority = 0, Status = "NotStarted" };

            // Act
            var result = _validator.TestValidate(task);

            // Assert
            result.ShouldHaveValidationErrorFor(t => t.Priority);
        }

        // Assert when priority is greater than 0
        [Fact]
        public void Should_Pass_When_Priority_Is_Greater_Than_0()
        {
            // Arrange
            var task = new TaskItem { Name = "Task1", Priority = 1, Status = "NotStarted" };

            // Act
            var result = _validator.TestValidate(task);

            // Assert
            result.ShouldNotHaveValidationErrorFor(t => t.Priority);
        }


        [Fact]
        public void Should_Fail_When_Task_Name_Is_Not_Unique()
        {
            // Arrange
            var task = new TaskItem { Name = "Existing Task", Priority = 1, Status = "NotStarted" };

            // Mock the service to return an existing task with the same name
            _taskServiceMock.Setup(x => x.GetAllTasks()).Returns(new List<TaskItem>
            {
                new TaskItem { Name = "Existing Task", Priority = 1, Status = "InProgress", Id = Guid.NewGuid() }
            });

            // Act
            var result = _validator.TestValidate(task);

            // Assert
            result.ShouldHaveValidationErrorFor(t => t.Name);
        }

        // Assert when task name is unique
        [Fact]
        public void Should_Pass_When_Task_Name_Is_Unique()
        {
            // Arrange
            var task = new TaskItem { Name = "Unique Task", Priority = 1, Status = "NotStarted" };

            // Mock the service to return a different task
            _taskServiceMock.Setup(x => x.GetAllTasks()).Returns(new List<TaskItem>
            {
                new TaskItem { Name = "Different Task", Priority = 1, Status = "InProgress", Id = Guid.NewGuid() }
            });

            // Act
            var result = _validator.TestValidate(task);

            // Assert
            result.ShouldNotHaveValidationErrorFor(t => t.Name);
        }

        // Assert when status is invalid
        [Fact]
        public void Should_Fail_When_Status_Is_Invalid()
        {
            // Arrange
            var task = new TaskItem { Name = "Task1", Priority = 1, Status = "InvalidStatus" };

            // Act
            var result = _validator.TestValidate(task);

            // Assert
            result.ShouldHaveValidationErrorFor(t => t.Status);
        }

        // Assert when status is valid
        [Fact]
        public void Should_Pass_When_Status_Is_Valid()
        {
            // Arrange
            var task = new TaskItem { Name = "Task1", Priority = 1, Status = "InProgress" };

            // Act
            var result = _validator.TestValidate(task);

            // Assert
            result.ShouldNotHaveValidationErrorFor(t => t.Status);
        }

        // Assert that task name is required
        [Fact]
        public void Should_Fail_When_Task_Name_Is_Empty()
        {
            // Arrange
            var task = new TaskItem { Name = "", Priority = 1, Status = "NotStarted" };

            // Act
            var result = _validator.TestValidate(task);

            // Assert
            result.ShouldHaveValidationErrorFor(t => t.Name);
        }

        // Assert that task name is not empty
        [Fact]
        public void Should_Pass_When_Task_Name_Is_Provided()
        {
            // Arrange
            var task = new TaskItem { Name = "Valid Task", Priority = 1, Status = "InProgress" };

            // Act
            var result = _validator.TestValidate(task);

            // Assert
            result.ShouldNotHaveValidationErrorFor(t => t.Name);
        }
    }
}
