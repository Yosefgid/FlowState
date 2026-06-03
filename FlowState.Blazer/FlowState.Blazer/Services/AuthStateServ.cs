using System.Text.Json;
using FlowState.Blazer.Models.Auth;
namespace FlowState.Blazer.Services
{
    public class AuthStateServ
    {
        private readonly TokenService _tokenService;
        public event Action? OnAuthStateChanged;

        public string? Username { get; private set; }
        public string? Email { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Username);

        public AuthStateServ(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task InitialiseAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            var claims = ParseClaimsFromJwt(token);
            if (claims.TryGetValue("exp", out var expStr) &&
               long.TryParse(expStr, out var exp) &&
               DateTimeOffset.FromUnixTimeSeconds(exp) <= DateTimeOffset.UtcNow)
            {
                await _tokenService.RemoveTokenAsync();
                return;
            }
            Username = claims.GetValueOrDefault("username");
            Email = claims.GetValueOrDefault("email");
        }

        public void SetUser(string username, string email)
        {
            Username = username;
            Email = email;
            OnAuthStateChanged?.Invoke();
        }
        
        public async Task LogoutAsync()
        {
            await _tokenService.RemoveTokenAsync();
            Username = null;
            Email = null;
            OnAuthStateChanged?.Invoke();
        }

        private Dictionary<string, string> ParseClaimsFromJwt(string token)
        {
            var claims = new Dictionary<string, string>();
            var payload = token.Split('.')[1];
            var base64 = payload.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var bytes = Convert.FromBase64String(base64);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if(keyValuePairs != null)
            {
                foreach(var kv in keyValuePairs)
                {
                    claims[kv.Key] = kv.Value.ToString();
                }
            }
            return claims;
        }
       
    }
}
