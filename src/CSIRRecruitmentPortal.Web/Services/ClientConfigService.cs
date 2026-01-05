using System.Net.Http.Json;
using CSIRRecruitmentPortal.Shared.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace CSIRRecruitmentPortal.Web.Services;

public class ClientConfigService : IConfigService
{
    private readonly HttpClient _httpClient;

    public ClientConfigService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SiteConfigDto> GetConfigAsync()
    {
        return await _httpClient.GetFromJsonAsync<SiteConfigDto>("api/config") 
               ?? new SiteConfigDto();
    }

    public async Task<bool> UpdateConfigAsync(SiteConfigDto config)
    {
        var response = await _httpClient.PutAsJsonAsync("api/config", config);
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> UploadAssetAsync(IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        
        // Limit to 5MB
        var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        
        content.Add(fileContent, "file", file.Name);

        var response = await _httpClient.PostAsync("api/config/upload", content);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        
        return null;
    }
}
