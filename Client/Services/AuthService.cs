using System.Net.Http.Json;
using Shared.DTOs.Auth;

namespace Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> Login(LoginRequest request)
        {
            var result = await _http.PostAsJsonAsync("api/auth/login", request);

            if (!result.IsSuccessStatusCode)
                return null;

            var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
            return response?.Token;
        }
    }
}
