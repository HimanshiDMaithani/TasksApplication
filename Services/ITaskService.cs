using System;
using System.Collections.Generic;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public interface ITaskService
    {
        List<TaskItem> GetAllTasks();
        TaskItem AddTask(TaskItem task);
        TaskItem UpdateTask(TaskItem task);
        bool DeleteTask(Guid id);
        TaskItem GetTaskById(Guid id);
    }
}
