using System.ComponentModel.DataAnnotations;

namespace FlowState.Models
{
    public class ToDoTask
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool IsCompleted { get; set; }
        
        public DateTime? TimeCompleted { get; set; }
        [Required]
        public DateTime TimeSet { get; set; }
        
        public string? GoogleId { get; set; }

        public ToDoTask(int id, string name, string description, string googleId)
        {
            Id = id;
            Name = name;
            Description = description;
            IsCompleted = false;
            TimeSet = DateTime.Now;
            GoogleId = googleId;
        }

    }
}
