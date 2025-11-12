using System.Net.Http.Json;
using Shared.DTOs;

public class DashboardService
{
    private readonly HttpClient _http;
    public DashboardService(HttpClient http) => _http = http;

    public Task<DashboardSummaryDto> GetSummary() =>
        _http.GetFromJsonAsync<DashboardSummaryDto>("dashboard/summary")!;
    public Task<object> GetClinicDetails(Guid id) =>
        _http.GetFromJsonAsync<object>($"dashboard/clinic/{id}")!;
}
