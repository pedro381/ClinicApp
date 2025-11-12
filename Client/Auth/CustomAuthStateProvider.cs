using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Client.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;
        private AuthenticationState _cachedState;
        private bool _isInitialized = false;

        public CustomAuthStateProvider(IJSRuntime js)
        {
            _js = js;
            // Inicializa com estado anônimo imediatamente
            _cachedState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Sempre verifica o localStorage para garantir estado atualizado
                var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

                if (string.IsNullOrWhiteSpace(token))
                {
                    // Se não há token, retorna estado anônimo
                    _cachedState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    return _cachedState;
                }

                // Se já temos um estado autenticado no cache, retorna ele (evita re-parsing desnecessário)
                if (_cachedState.User?.Identity?.IsAuthenticated == true)
                {
                    return _cachedState;
                }

                // Parse do token e atualiza o cache
                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
                var user = new ClaimsPrincipal(identity);
                _cachedState = new AuthenticationState(user);
                return _cachedState;
            }
            catch
            {
                // Em caso de erro, retorna o estado em cache ou anônimo
                return _cachedState ?? new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        // 🔹 Notifica autenticação (login)
        public void NotifyUserAuthentication(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            var authenticatedUser = new ClaimsPrincipal(identity);
            _cachedState = new AuthenticationState(authenticatedUser);
            var authState = Task.FromResult(_cachedState);
            NotifyAuthenticationStateChanged(authState);
        }

        // 🔹 Notifica logout
        public async Task LogoutAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            _cachedState = new AuthenticationState(anonymousUser);
            var authState = Task.FromResult(_cachedState);
            NotifyAuthenticationStateChanged(authState);
        }

        // 🔹 Decodifica o JWT sem depender de libs externas
        private IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var payload = token.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = new List<Claim>();
            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    string claimType = kvp.Key;
                    
                    // Mapeia chaves comuns do JWT para ClaimTypes padrão
                    if (kvp.Key == "nameid" || kvp.Key == ClaimTypes.NameIdentifier)
                        claimType = ClaimTypes.NameIdentifier;
                    else if (kvp.Key == "unique_name" || kvp.Key == "name" || kvp.Key == ClaimTypes.Name)
                        claimType = ClaimTypes.Name;
                    else if (kvp.Key == "role" || kvp.Key == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                        claimType = ClaimTypes.Role;

                    if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in element.EnumerateArray())
                            claims.Add(new Claim(claimType, item.GetString() ?? ""));
                    }
                    else
                    {
                        claims.Add(new Claim(claimType, kvp.Value?.ToString() ?? ""));
                    }
                }
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}