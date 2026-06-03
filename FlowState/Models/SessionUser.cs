namespace FlowState.Models
{
    public class SessionUser
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int UserId { get; set; }

        public SessionUser(int sessionId, int userId)
        {
            SessionId = sessionId;
            UserId = userId;
        }

    }
}
