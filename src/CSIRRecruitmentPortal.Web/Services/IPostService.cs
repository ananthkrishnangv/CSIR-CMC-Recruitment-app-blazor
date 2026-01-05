using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Web.Services;

public interface IPostService
{
    Task<List<JobPostDto>> GetAllPostsAsync();
    Task<JobPostDto?> GetPostByIdAsync(Guid id);
    Task<JobPostDto> CreatePostAsync(CreatePostRequest request);
    Task<bool> UpdatePostStatusAsync(Guid id, PostStatus status, string? resultPdfUrl = null);
    Task<bool> DeletePostAsync(Guid id);
}
