using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using CSIRRecruitmentPortal.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSIRRecruitmentPortal.Web.Services;

public class ClientAuthService : AuthenticationStateProvider, IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationState _anonymous;

    public ClientAuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task<AuthResponse> LoginApplicantAsync(ApplicantLoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login/applicant", request);
        return await HandleAuthResponse(response);
    }

    public async Task<AuthResponse> LoginAdminAsync(AdminLoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login/admin", request);
        return await HandleAuthResponse(response);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        return await response.Content.ReadFromJsonAsync<AuthResponse>() 
               ?? new AuthResponse { Success = false, Message = "Unknown error" };
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
            return _anonymous;

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
    }

    private async Task<AuthResponse> HandleAuthResponse(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (response.IsSuccessStatusCode && result != null && result.Success)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        return result ?? new AuthResponse { Success = false, Message = "Unknown error" };
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
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

    // Client-side implementation of interface methods that are server-only
    public Task<UserDto?> GetUserByIdAsync(Guid userId) => throw new NotImplementedException("Client side cannot verify user by ID directly");
    public string GenerateJwtToken(Shared.Entities.User user) => throw new NotImplementedException("Client side cannot generate tokens");
}
