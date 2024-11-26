using FluentValidation;
using System;
using System.Linq;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Validation
{
    public class TaskValidator : AbstractValidator<TaskItem>
    {
        private readonly ITaskService _taskService;

        public TaskValidator(ITaskService taskService)
        {
            _taskService = taskService;

            RuleFor(task => task.Name)
            .NotEmpty().WithMessage("Task name is required.")
            .Must((task, name) => BeUnique(name, task))
            .WithMessage("Task name must be unique.");

            RuleFor(task => task.Priority)
                .GreaterThan(0).WithMessage("Priority must be greater than 0.");

            RuleFor(task => task.Status)
            .NotEmpty().WithMessage("Task status is required.")
            .Must(BeValidStatus).WithMessage("Invalid task status. Allowed values are 'NotStarted', 'InProgress', or 'Completed'.");
        }

        private bool BeUnique(string taskName, TaskItem task)
        {
            var list = _taskService.GetAllTasks();
            if (list == null)
            {
                return true;
            }

            var existingTask = list.FirstOrDefault(t => t.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase));

            if (existingTask != null && existingTask.Id != task.Id)
            {
                return false;
            }

            return true;
        }


        private bool BeValidStatus(string status)
        {
            return status == "NotStarted" || status == "InProgress" || status == "Completed";
        }
    }

}
