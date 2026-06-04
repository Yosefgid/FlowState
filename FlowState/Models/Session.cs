namespace FlowState.Models
{
    public class Session
    {
        public int Id { get; set; }

        public int AdminId { get; set; }

        public string Name { get; set; }


        public Session(int id, int adminId, string name)
        {
            Id = id; 
            AdminId = adminId;
            Name = name;
        }

    }
}
