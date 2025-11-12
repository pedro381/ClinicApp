using System.Net.Http.Json;
using Shared.DTOs;

public class ClinicService
{
    private readonly HttpClient _http;
    public ClinicService(HttpClient http) => _http = http;

    public Task<List<ClinicDto>> GetAll() => _http.GetFromJsonAsync<List<ClinicDto>>("clinics")!;
    public Task<object> Get(Guid id) => _http.GetFromJsonAsync<object>($"clinics/{id}")!;
    public Task<ClinicDto> Create(ClinicDto dto) => _http.PostAsJsonAsync("clinics", dto).Result.Content.ReadFromJsonAsync<ClinicDto>()!;
}
