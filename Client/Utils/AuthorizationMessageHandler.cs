using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace Client.Utils
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public AuthorizationMessageHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch
            {
                // Se não conseguir obter o token, continua sem ele
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
