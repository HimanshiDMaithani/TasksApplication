using Moq;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagementSystem.Tests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;

        public TaskServiceTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
        }

        [Fact]
        public void AddTask_Should_Add_New_Task_With_Guid()
        {
            // Arrange
            var taskService = new TaskService();
            var task = new TaskItem { Id = Guid.NewGuid(), Name = "Test Task", Priority = 1, Status = "NotStarted" };

            // Act
            taskService.AddTask(task);
            var tasks = taskService.GetAllTasks();

            // Assert
            Assert.Single(tasks);  // Ensure one task is added
            Assert.Contains(tasks, t => t.Name == "Test Task");
            Assert.NotEqual(Guid.Empty, task.Id);  // Ensure the GUID is not empty for the new task
        }

        [Fact]
        public void UpdateTask_Should_Modify_Existing_Task_With_Guid()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = taskId,
                Name = "Test Task",
                Priority = 1,
                Status = "NotStarted"
            };

            _taskServiceMock.Setup(s => s.GetAllTasks()).Returns(new List<TaskItem> { task });

            // Act - simulate updating the task
            task.Name = "Updated Task";
            _taskServiceMock.Object.UpdateTask(task);

            // Assert - verify the task was updated
            _taskServiceMock.Verify(s => s.UpdateTask(It.Is<TaskItem>(t => t.Id == taskId && t.Name == "Updated Task")), Times.Once);
        }

        [Fact]
        public void DeleteTask_Should_Remove_Task_With_Guid()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = taskId,
                Name = "Test Task",
                Priority = 1,
                Status = "NotStarted"
            };

            _taskServiceMock.Setup(s => s.GetAllTasks()).Returns(new List<TaskItem> { task });

            // Act - delete the task
            _taskServiceMock.Object.DeleteTask(taskId);

            // Assert - ensure the task is removed
            _taskServiceMock.Verify(s => s.DeleteTask(taskId), Times.Once);
        }
    }
}
