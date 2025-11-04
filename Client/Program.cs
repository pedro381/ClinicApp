/*using Client;
using Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress)
    });
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();*/
using Client;
using Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- AJUSTE IMPORTANTE AQUI ---
// Pega a URL da API do arquivo de configuração
string apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl")
                    ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration.");

// Configura o HttpClient para usar a URL da API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
// --- FIM DO AJUSTE ---

builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();

