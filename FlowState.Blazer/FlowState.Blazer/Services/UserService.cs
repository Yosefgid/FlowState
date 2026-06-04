using FlowState.Blazer.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
namespace FlowState.Blazer.Services
{
    public class UserService
    {
        private readonly HttpClient _http;
        private readonly TokenService _tokenService;
   

        public UserService(HttpClient http, TokenService tokenService)
        {
            _http = http;
            _tokenService = tokenService;
        }
        

        public async Task<UserProfile?> GetMyProfileAsync()
        {
            var id = await GetUserIdAsync();
            if (id == null) return null;
            var response = await _http.GetAsync($"/api/users/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<UserProfile>();
            //var response = await _http.GetAsync("api/users/me"); // your actual route
            //Console.WriteLine($"[UserService] GetMyProfile status: {response.StatusCode}");
            //if (!response.IsSuccessStatusCode) return null;
            //return await response.Content.ReadFromJsonAsync<UserProfile>();

        }

        public async Task<(bool Success, string? Error)> UpdateEmailAsync(string email)
        {
            var id = await GetUserIdAsync();
            if (id == null) return (false, "Not logged in.");

            var response = await _http.PutAsJsonAsync($"/api/users/{id}", new UpdateEmailRequest { Email = email });
            return response.IsSuccessStatusCode ? (true, null) : (false, await ReadErrorAsync(response));


        }
        public async Task<(bool Success, string? Error)> ChangeUsernameAsync(string newUsername)
        {
            var id = await GetUserIdAsync();
            if (id == null) return (false, "Not logged in.");

            var response = await _http.PatchAsJsonAsync($"/api/users/{id}/username", new ChangeUsernameRequest { NewUsername = newUsername });
            return response.IsSuccessStatusCode ? (true, null) : (false, await ReadErrorAsync(response));


        }
        public async Task<(bool Success, string? Error)> DeleteAccountAsync()
        {
            var id = await GetUserIdAsync();
            if (id == null) return (false, "Not logged in.");

            var response = await _http.DeleteAsync($"/api/users/{id}");
            return response.IsSuccessStatusCode ? (true, null) : (false, await ReadErrorAsync(response));


        }

        private async Task<int?> GetUserIdAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return null;
            var payload = token.Split('.')[1];
            var base64 = payload.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            } 

            try
            {
                var bytes = Convert.FromBase64String(base64);
                var json = System.Text.Encoding.UTF8.GetString(bytes);

                using var doc = JsonDocument.Parse(json);
                //sub==userid in the token header
                if (doc.RootElement.TryGetProperty("sub", out var sub) && int.TryParse(sub.GetString(), out var id)) return id;
            }
            catch { }
            return null;
        }
        

        private async Task<string> ReadErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var text = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(text)) return $"Request failed ({(int)response.StatusCode}).";
                {
                    using var doc = JsonDocument.Parse(text);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("message", out var msg))
                    {
                        return msg.GetString() ?? text;
                    }
                    if(doc.RootElement.ValueKind == JsonValueKind.String){
                        return doc.RootElement.GetString() ?? text;
                    }
                    return text;
                }
            }
            catch { return $"Something went wrong.. The Request has failed ({(int)response.StatusCode})."; }
        }



    }
}
