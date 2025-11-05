using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace Client.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;

        public CustomAuthStateProvider(IJSRuntime js)
        {
            _js = js;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (string.IsNullOrEmpty(token))
            {
                // Usuário NÃO autenticado
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Decodifica o JWT (simples)
            var claims = ParseClaimsFromJwt(token);

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var payload = token.Split('.')[1];
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(payload)));

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return keyValuePairs.Select(k => new Claim(k.Key, k.Value.ToString()));
        }

        private string PadBase64(string base64)
        {
            if (base64.Length % 4 == 2) return base64 + "==";
            if (base64.Length % 4 == 3) return base64 + "=";
            return base64;
        }

        public void NotifyUserAuthentication()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

    }
}
