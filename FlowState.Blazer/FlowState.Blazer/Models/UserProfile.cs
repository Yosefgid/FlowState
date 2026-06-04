namespace FlowState.Blazer.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateEmailRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ChangeUsernameRequest
    {
        public string NewUsername { get; set; } = string.Empty;
    }
}
