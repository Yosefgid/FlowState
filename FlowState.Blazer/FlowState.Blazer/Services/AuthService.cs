using FlowState.Blazer.Models.Auth;

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
                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            await _tokenService.SetTokenAsync(result!.Token);
            _authStateService.SetUser(result.Username, result.Email);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> LoginAsync(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync("/api/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            await _tokenService.SetTokenAsync(result!.Token);
            _authStateService.SetUser(result.Username, result.Email);
            return (true, null);
        }

        public async Task LogoutAsync()
        {
            await _authStateService.LogoutAsync();
        }
    }
}
