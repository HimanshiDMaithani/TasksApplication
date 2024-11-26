using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public class TaskService : ITaskService
    {
        private readonly List<TaskItem> _tasks = new();
        public TaskItem AddTask(TaskItem task)
        {
            if (_tasks.Any(t => t.Name.Equals(task.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Task with the same name already exists.");
            _tasks.Add(task);
            return task;
        }

        public bool DeleteTask(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id && t.Status == "Completed");
            if (task == null) return false;
            _tasks.Remove(task);
            return true;
        }

        public List<TaskItem> GetAllTasks()
        {
            return _tasks ?? new List<TaskItem>();
        }

        public TaskItem UpdateTask(TaskItem task)
        {
            var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existingTask == null) throw new KeyNotFoundException("Task not found.");
            existingTask.Name = task.Name;
            existingTask.Priority = task.Priority;
            existingTask.Status = task.Status;
            return existingTask;
        }

        public TaskItem GetTaskById(Guid id) => _tasks.FirstOrDefault(t => t.Id == id);
    }
}
