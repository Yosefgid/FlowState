using System.ComponentModel.DataAnnotations;

namespace FlowState.Models
{
    public class ToDoTask
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? TimeCompleted { get; set; }

        [Required]
        public DateTime TimeSet { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EisenCat Category { get; set; }

        public string? GoogleId { get; set; }

        public ToDoTask(string name, string description, string googleId)
        {
            Name = name;
            Description = description;
            IsCompleted = false;
            TimeSet = DateTime.Now;
            GoogleId = googleId;
            Category = EisenCat.Do;
        }

        public ToDoTask setStartDate(DateTime date)
        {
            StartDate = date;
            return this;
        }

        public ToDoTask setEndDate(DateTime date)
        {
            EndDate = date;
            return this;
        }
    }
}