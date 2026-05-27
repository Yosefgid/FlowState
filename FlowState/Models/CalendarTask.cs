namespace FlowState.Models
{
    public class CalendarTask : ToDoTask
    {
        public CalendarTask(string name, string description, string googleId) : base(name, description, googleId)
        {
        }
    }
}
