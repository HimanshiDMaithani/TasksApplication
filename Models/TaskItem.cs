using System;

namespace TaskManagementSystem.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
    }
}
