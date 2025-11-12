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
using Client.Auth;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Pega a URL da API do arquivo de configuração
string apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl")
                    ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration.");

// Configura o HttpClient para usar a URL da API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// --- Configuração de Autenticação ---

// Seu serviço de lógica de autenticação (login, logout, etc.)
builder.Services.AddScoped<AuthService>();

// Adiciona os serviços principais de autorização (ex: para usar [Authorize])
builder.Services.AddAuthorizationCore();

// Registra seu provedor de estado de autenticação personalizado
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// **LINHA FALTANTE ADICIONADA AQUI**
// Habilita o componente CascadingAuthenticationState a receber o estado de autenticação
// do provedor e distribuí-lo para o resto da aplicação.
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ClinicService>();
builder.Services.AddScoped<MaterialService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ClinicService>();
builder.Services.AddScoped<MaterialService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, Client.Auth.CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();


// --- Fim da Configuração de Autenticação ---

await builder.Build().RunAsync();


