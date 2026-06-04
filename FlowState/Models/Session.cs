namespace FlowState.Models
{
    public class Session
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public Session(int id, string name)
        {
            Id = id; 
            Name = name;
        }

    }
}
