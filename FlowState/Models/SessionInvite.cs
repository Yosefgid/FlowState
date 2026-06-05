namespace FlowState.Models
{
    public class SessionInvite
    {

        public int Id { get; set; }

        public string Token { get; set; } = Guid.NewGuid().ToString("N");

        public int SessionId { get; set; }

        public DateTime ExpiresAt { get; set; }


        public SessionInvite(int sessionId)
        {
            SessionId = sessionId;
            ExpiresAt = DateTime.Now.AddDays(1);
        }
     
    }
}
