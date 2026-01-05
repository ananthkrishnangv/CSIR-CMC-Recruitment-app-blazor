using System.Net.Http.Json;
using CSIRRecruitmentPortal.Shared.DTOs;

namespace CSIRRecruitmentPortal.Web.Services;

public class ClientApplicationService : IApplicationService
{
    private readonly HttpClient _httpClient;

    public ClientApplicationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return await _httpClient.GetFromJsonAsync<DashboardStatsDto>("api/applications/dashboard-stats") 
               ?? new DashboardStatsDto();
    }

    public async Task<List<ApplicationListItemDto>> GetMyApplicationsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ApplicationListItemDto>>("api/applications/my-applications") 
               ?? new List<ApplicationListItemDto>();
    }

    public async Task<ApplicationFormDto?> GetApplicationByIdAsync(Guid id)
    {
        try 
        {
            return await _httpClient.GetFromJsonAsync<ApplicationFormDto>($"api/applications/{id}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<ApplicationFormDto> CreateApplicationAsync(ApplicationFormDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/applications", request);
        return await response.Content.ReadFromJsonAsync<ApplicationFormDto>() 
               ?? throw new Exception("Failed to create application");
    }

    public async Task<ApplicationFormDto?> UpdateApplicationAsync(Guid id, ApplicationFormDto request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/applications/{id}", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApplicationFormDto>();
        }
        return null;
    }

    public async Task<bool> SubmitApplicationAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"api/applications/{id}/submit", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ApplicationListItemDto>> GetApplicationsByPostIdAsync(Guid postId)
    {
        return await _httpClient.GetFromJsonAsync<List<ApplicationListItemDto>>($"api/applications?postId={postId}") 
               ?? new List<ApplicationListItemDto>();
    }

    public async Task<bool> UpdateApplicationStatusAsync(Guid id, Shared.Enums.ApplicationStatus status, string? remarks = null)
    {
        var url = $"api/applications/{id}/status?status={status}";
        if (!string.IsNullOrEmpty(remarks)) url += $"&remarks={Uri.EscapeDataString(remarks)}";
        
        var response = await _httpClient.PatchAsync(url, null);
        return response.IsSuccessStatusCode;
    }
}
