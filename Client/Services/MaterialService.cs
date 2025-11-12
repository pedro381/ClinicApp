using System.Net.Http.Json;
using Shared.DTOs;

public class MaterialService
{
    private readonly HttpClient _http;
    public MaterialService(HttpClient http) => _http = http;

    public Task<List<MaterialDto>> GetAll() => _http.GetFromJsonAsync<List<MaterialDto>>("materials")!;
    public Task<MaterialDto> Create(MaterialDto dto) => _http.PostAsJsonAsync("materials", dto).Result.Content.ReadFromJsonAsync<MaterialDto>()!;
    public Task Delete(Guid id) => _http.DeleteAsync($"materials/{id}");
}
