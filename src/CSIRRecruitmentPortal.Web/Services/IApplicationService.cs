using CSIRRecruitmentPortal.Shared.DTOs;

namespace CSIRRecruitmentPortal.Web.Services;

public interface IApplicationService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<List<ApplicationListItemDto>> GetMyApplicationsAsync();
    Task<ApplicationFormDto?> GetApplicationByIdAsync(Guid id);
    Task<ApplicationFormDto> CreateApplicationAsync(ApplicationFormDto request);
    Task<ApplicationFormDto?> UpdateApplicationAsync(Guid id, ApplicationFormDto request);
    Task<bool> SubmitApplicationAsync(Guid id);
    
    // Admin methods
    Task<List<ApplicationListItemDto>> GetApplicationsByPostIdAsync(Guid postId);
    Task<bool> UpdateApplicationStatusAsync(Guid id, ApplicationStatus status, string? remarks = null);
}
