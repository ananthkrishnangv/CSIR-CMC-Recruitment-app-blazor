using CSIRRecruitmentPortal.Shared.DTOs;
using Microsoft.AspNetCore.Components.Forms;

namespace CSIRRecruitmentPortal.Web.Services;

public interface IConfigService
{
    Task<SiteConfigDto> GetConfigAsync();
    Task<bool> UpdateConfigAsync(SiteConfigDto config);
    Task<string?> UploadAssetAsync(IBrowserFile file);
}
