using System.Net.Http.Json;
using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Web.Services;

public class ClientPostService : IPostService
{
    private readonly HttpClient _httpClient;

    public ClientPostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<JobPostDto>> GetAllPostsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<JobPostDto>>("api/posts") 
               ?? new List<JobPostDto>();
    }

    public async Task<JobPostDto?> GetPostByIdAsync(Guid id)
    {
        try 
        {
            return await _httpClient.GetFromJsonAsync<JobPostDto>($"api/posts/{id}");
        }
        catch (HttpRequestException) 
        {
            return null;
        }
    }

    public async Task<JobPostDto> CreatePostAsync(CreatePostRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/posts", request);
        return await response.Content.ReadFromJsonAsync<JobPostDto>() 
               ?? throw new Exception("Failed to create post");
    }

    public async Task<bool> UpdatePostStatusAsync(Guid id, PostStatus status, string? resultPdfUrl = null)
    {
        var url = $"api/posts/{id}/status";
        if (!string.IsNullOrEmpty(resultPdfUrl))
            url += $"?resultPdfUrl={Uri.EscapeDataString(resultPdfUrl)}";
            
        var response = await _httpClient.PatchAsJsonAsync(url, status);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/posts/{id}");
        return response.IsSuccessStatusCode;
    }
}
