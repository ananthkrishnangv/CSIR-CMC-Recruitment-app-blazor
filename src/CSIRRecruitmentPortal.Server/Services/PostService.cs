using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using CSIRRecruitmentPortal.Server.Data;
using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Entities;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Server.Services;

/// <summary>
/// Job post service implementation
/// </summary>
public class PostService : IPostService
{
    private readonly AppDbContext _context;

    public PostService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobPostDto>> GetAllPostsAsync()
    {
        var posts = await _context.JobPosts
            .Include(p => p.CustomFields.OrderBy(f => f.SortOrder))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(MapToDto).ToList();
    }

    public async Task<List<JobPostDto>> GetPublishedPostsAsync()
    {
        var posts = await _context.JobPosts
            .Include(p => p.CustomFields.OrderBy(f => f.SortOrder))
            .Where(p => p.Status != PostStatus.DRAFT)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(MapToDto).ToList();
    }

    public async Task<JobPostDto?> GetPostByIdAsync(Guid id)
    {
        var post = await _context.JobPosts
            .Include(p => p.CustomFields.OrderBy(f => f.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == id);

        return post != null ? MapToDto(post) : null;
    }

    public async Task<JobPostDto> CreatePostAsync(CreatePostRequest request)
    {
        var post = new JobPost
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Title = request.Title,
            Type = request.Type,
            Department = request.Department,
            Vacancies = request.Vacancies,
            LastDate = request.LastDate,
            Description = request.Description,
            Status = PostStatus.DRAFT,
            CreatedAt = DateTime.UtcNow
        };

        // Add custom fields
        foreach (var fieldDto in request.CustomFields)
        {
            var field = new PostCustomField
            {
                Id = Guid.NewGuid(),
                PostId = post.Id,
                Label = fieldDto.Label,
                FieldType = fieldDto.Type,
                IsRequired = fieldDto.Required,
                Placeholder = fieldDto.Placeholder,
                OptionsJson = fieldDto.Options != null ? JsonSerializer.Serialize(fieldDto.Options) : null,
                ValidationJson = fieldDto.Validation != null ? JsonSerializer.Serialize(fieldDto.Validation) : null,
                LogicJson = fieldDto.Logic != null ? JsonSerializer.Serialize(fieldDto.Logic) : null,
                SortOrder = request.CustomFields.IndexOf(fieldDto)
            };
            post.CustomFields.Add(field);
        }

        _context.JobPosts.Add(post);
        await _context.SaveChangesAsync();

        return MapToDto(post);
    }

    public async Task<JobPostDto?> UpdatePostAsync(Guid id, CreatePostRequest request)
    {
        var post = await _context.JobPosts
            .Include(p => p.CustomFields)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return null;

        post.Code = request.Code;
        post.Title = request.Title;
        post.Type = request.Type;
        post.Department = request.Department;
        post.Vacancies = request.Vacancies;
        post.LastDate = request.LastDate;
        post.Description = request.Description;

        // Remove existing custom fields and add new ones
        _context.PostCustomFields.RemoveRange(post.CustomFields);
        post.CustomFields.Clear();

        foreach (var fieldDto in request.CustomFields)
        {
            var field = new PostCustomField
            {
                Id = Guid.NewGuid(),
                PostId = post.Id,
                Label = fieldDto.Label,
                FieldType = fieldDto.Type,
                IsRequired = fieldDto.Required,
                Placeholder = fieldDto.Placeholder,
                OptionsJson = fieldDto.Options != null ? JsonSerializer.Serialize(fieldDto.Options) : null,
                ValidationJson = fieldDto.Validation != null ? JsonSerializer.Serialize(fieldDto.Validation) : null,
                LogicJson = fieldDto.Logic != null ? JsonSerializer.Serialize(fieldDto.Logic) : null,
                SortOrder = request.CustomFields.IndexOf(fieldDto)
            };
            post.CustomFields.Add(field);
        }

        await _context.SaveChangesAsync();

        return MapToDto(post);
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var post = await _context.JobPosts.FindAsync(id);
        if (post == null) return false;

        _context.JobPosts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePostStatusAsync(Guid id, PostStatus status, string? resultPdfUrl = null)
    {
        var post = await _context.JobPosts.FindAsync(id);
        if (post == null) return false;

        post.Status = status;
        if (!string.IsNullOrEmpty(resultPdfUrl))
        {
            post.FinalResultPdfUrl = resultPdfUrl;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private static JobPostDto MapToDto(JobPost post) => new()
    {
        Id = post.Id,
        Code = post.Code,
        Title = post.Title,
        Type = post.Type,
        Department = post.Department,
        Vacancies = post.Vacancies,
        LastDate = post.LastDate,
        Description = post.Description,
        Status = post.Status,
        FinalResultPdfUrl = post.FinalResultPdfUrl,
        CustomFields = post.CustomFields.Select(f => new CustomFieldDto
        {
            Id = f.Id,
            Label = f.Label,
            Type = f.FieldType,
            Required = f.IsRequired,
            Placeholder = f.Placeholder,
            Options = !string.IsNullOrEmpty(f.OptionsJson) 
                ? JsonSerializer.Deserialize<List<string>>(f.OptionsJson) 
                : null,
            Validation = !string.IsNullOrEmpty(f.ValidationJson)
                ? JsonSerializer.Deserialize<FieldValidationDto>(f.ValidationJson)
                : null,
            Logic = !string.IsNullOrEmpty(f.LogicJson)
                ? JsonSerializer.Deserialize<FieldLogicDto>(f.LogicJson)
                : null
        }).ToList()
    };
}
