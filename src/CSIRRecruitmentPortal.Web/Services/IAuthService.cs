using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Entities;

namespace CSIRRecruitmentPortal.Web.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginApplicantAsync(ApplicantLoginRequest request);
    Task<AuthResponse> LoginAdminAsync(AdminLoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task Logout();
}
