using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Entities;

namespace CSIRRecruitmentPortal.Server.Services;

#region Service Interfaces

public interface IAuthService
{
    Task<AuthResponse> LoginApplicantAsync(ApplicantLoginRequest request);
    Task<AuthResponse> LoginAdminAsync(AdminLoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    string GenerateJwtToken(User user);
}

public interface IPostService
{
    Task<List<JobPostDto>> GetAllPostsAsync();
    Task<List<JobPostDto>> GetPublishedPostsAsync();
    Task<JobPostDto?> GetPostByIdAsync(Guid id);
    Task<JobPostDto> CreatePostAsync(CreatePostRequest request);
    Task<JobPostDto?> UpdatePostAsync(Guid id, CreatePostRequest request);
    Task<bool> DeletePostAsync(Guid id);
    Task<bool> UpdatePostStatusAsync(Guid id, Shared.Enums.PostStatus status, string? resultPdfUrl = null);
}

public interface IApplicationService
{
    Task<List<ApplicationListItemDto>> GetApplicationsByUserAsync(string aadhaar);
    Task<List<ApplicationListItemDto>> GetApplicationsByPostAsync(Guid postId);
    Task<ApplicationFormDto?> GetApplicationByIdAsync(Guid id);
    Task<ApplicationFormDto> CreateApplicationAsync(Guid userId, ApplicationFormDto request);
    Task<ApplicationFormDto?> UpdateApplicationAsync(Guid id, ApplicationFormDto request);
    Task<bool> SubmitApplicationAsync(Guid id);
    Task<bool> BulkUpdateStatusAsync(BulkStatusUpdateRequest request);
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<List<ApplicationTrendDto>> GetApplicationTrendsAsync();
    Task<List<CategoryDistributionDto>> GetCategoryDistributionAsync();
}

public interface IHelpdeskService
{
    Task<List<SupportTicketDto>> GetTicketsByUserAsync(Guid userId);
    Task<List<SupportTicketDto>> GetAllTicketsAsync();
    Task<SupportTicketDto?> GetTicketByIdAsync(Guid id);
    Task<SupportTicketDto> CreateTicketAsync(Guid userId, CreateTicketRequest request);
    Task<SupportTicketDto?> ReplyToTicketAsync(Guid ticketId, Guid senderId, TicketReplyRequest request);
    Task<bool> ResolveTicketAsync(Guid ticketId);
}

public interface IConfigService
{
    Task<SiteConfigDto> GetSiteConfigAsync();
    Task<bool> UpdateSiteConfigAsync(SiteConfigDto config);
    Task<List<NewsItemDto>> GetNewsItemsAsync();
    Task<NewsItemDto> AddNewsItemAsync(NewsItemDto item);
    Task<bool> DeleteNewsItemAsync(Guid id);
}

#endregion
