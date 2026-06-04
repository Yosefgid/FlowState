using FlowState.Blazer.Models.Auth;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FlowState.Blazer.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly TokenService _tokenService;
        private readonly AuthStateServ _authStateService;

        public AuthService(HttpClient http, TokenService tokenService, AuthStateServ authStateService)
        {
            _http = http;
            _tokenService = tokenService;
            _authStateService = authStateService;
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync("/api/auth/register", request);

            if (!response.IsSuccessStatusCode)
            {
                
                return (false, await ReadErrorAsync(response));
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            await _tokenService.SetTokenAsync(result!.Token);
            SetAuthHeader(result.Token);
            _authStateService.SetUser(result.Username, result.Email);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> LoginAsync(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync("/api/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                return (false, await ReadErrorAsync(response));
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            await _tokenService.SetTokenAsync(result!.Token);
            SetAuthHeader(result.Token);
            _authStateService.SetUser(result.Username, result.Email);
            return (true, null);
        }

        public async Task LogoutAsync()
        {
            await _authStateService.LogoutAsync();
            SetAuthHeader(null);
        }
        private void SetAuthHeader(string? token)
        {
            _http.DefaultRequestHeaders.Authorization =
                string.IsNullOrWhiteSpace(token)
                    ? null
                    : new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> ReadErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var body = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (body.TryGetProperty("message", out var msg))
                {
                    return msg.GetString() ?? "Something went wrong.";
                }
            }
            catch { }
            return "Something went wrong.";
        }
    }
}
