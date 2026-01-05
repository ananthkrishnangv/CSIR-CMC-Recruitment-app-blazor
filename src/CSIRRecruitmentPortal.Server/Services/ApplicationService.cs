using Microsoft.EntityFrameworkCore;
using CSIRRecruitmentPortal.Server.Data;
using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Entities;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Server.Services;

/// <summary>
/// Application service implementation
/// </summary>
public class ApplicationService : IApplicationService
{
    private readonly AppDbContext _context;

    public ApplicationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApplicationListItemDto>> GetApplicationsByUserAsync(string aadhaar)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Aadhaar == aadhaar);
        if (user == null) return new List<ApplicationListItemDto>();

        var applications = await _context.Applications
            .Include(a => a.Post)
            .Include(a => a.PersonalDetails)
            .Include(a => a.Education)
            .Where(a => a.UserId == user.Id)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return applications.Select(MapToListItem).ToList();
    }

    public async Task<List<ApplicationListItemDto>> GetApplicationsByPostAsync(Guid postId)
    {
        var applications = await _context.Applications
            .Include(a => a.Post)
            .Include(a => a.PersonalDetails)
            .Include(a => a.Education)
            .Include(a => a.User)
            .Where(a => a.PostId == postId)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();

        return applications.Select(MapToListItem).ToList();
    }

    public async Task<ApplicationFormDto?> GetApplicationByIdAsync(Guid id)
    {
        var app = await _context.Applications
            .Include(a => a.Post)
            .Include(a => a.PersonalDetails)
            .Include(a => a.Education)
            .Include(a => a.Experience)
            .Include(a => a.Documents)
            .Include(a => a.CustomValues)
            .FirstOrDefaultAsync(a => a.Id == id);

        return app != null ? MapToFormDto(app) : null;
    }

    public async Task<ApplicationFormDto> CreateApplicationAsync(Guid userId, ApplicationFormDto request)
    {
        // Generate application number
        var year = DateTime.UtcNow.Year;
        var count = await _context.Applications.CountAsync(a => a.CreatedAt.Year == year) + 1;
        var applicationNumber = $"SERC-{year}-{count:D5}";

        var application = new Application
        {
            Id = Guid.NewGuid(),
            ApplicationNumber = applicationNumber,
            UserId = userId,
            PostId = request.PostId,
            Status = ApplicationStatus.Draft,
            StatementOfPurpose = request.StatementOfPurpose,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add personal details
        if (request.PersonalDetails != null)
        {
            application.PersonalDetails = new ApplicationPersonalDetails
            {
                ApplicationId = application.Id,
                FullName = request.PersonalDetails.FullName,
                DateOfBirth = request.PersonalDetails.DateOfBirth,
                Gender = Enum.TryParse<Gender>(request.PersonalDetails.Gender, out var gender) ? gender : null,
                Category = request.PersonalDetails.Category,
                FatherName = request.PersonalDetails.FatherName,
                Address = request.PersonalDetails.Address,
                Nationality = request.PersonalDetails.Nationality
            };
        }

        // Add education
        foreach (var edu in request.Education)
        {
            application.Education.Add(new ApplicationEducation
            {
                Id = Guid.NewGuid(),
                ApplicationId = application.Id,
                Level = edu.Level,
                Institution = edu.Institution,
                BoardUniversity = edu.Board,
                YearPassing = int.TryParse(edu.Year, out var yr) ? yr : null,
                Percentage = decimal.TryParse(edu.Percentage, out var pct) ? pct : null
            });
        }

        // Add experience
        foreach (var exp in request.Experience)
        {
            application.Experience.Add(new ApplicationExperience
            {
                Id = Guid.NewGuid(),
                ApplicationId = application.Id,
                Organization = exp.Organization,
                Designation = exp.Designation,
                FromDate = exp.FromDate,
                ToDate = exp.ToDate,
                Responsibilities = exp.Responsibilities
            });
        }

        // Add custom values
        foreach (var cv in request.CustomValues)
        {
            application.CustomValues.Add(new ApplicationCustomValue
            {
                Id = Guid.NewGuid(),
                ApplicationId = application.Id,
                FieldId = cv.Key,
                FieldValue = cv.Value
            });
        }

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return MapToFormDto(application);
    }

    public async Task<ApplicationFormDto?> UpdateApplicationAsync(Guid id, ApplicationFormDto request)
    {
        var app = await _context.Applications
            .Include(a => a.PersonalDetails)
            .Include(a => a.Education)
            .Include(a => a.Experience)
            .Include(a => a.CustomValues)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return null;

        app.StatementOfPurpose = request.StatementOfPurpose;
        app.UpdatedAt = DateTime.UtcNow;

        // Update personal details
        if (app.PersonalDetails != null && request.PersonalDetails != null)
        {
            app.PersonalDetails.FullName = request.PersonalDetails.FullName;
            app.PersonalDetails.DateOfBirth = request.PersonalDetails.DateOfBirth;
            app.PersonalDetails.Gender = Enum.TryParse<Gender>(request.PersonalDetails.Gender, out var gender) ? gender : null;
            app.PersonalDetails.Category = request.PersonalDetails.Category;
            app.PersonalDetails.FatherName = request.PersonalDetails.FatherName;
            app.PersonalDetails.Address = request.PersonalDetails.Address;
            app.PersonalDetails.Nationality = request.PersonalDetails.Nationality;
        }

        // Update education (replace all)
        _context.ApplicationEducation.RemoveRange(app.Education);
        foreach (var edu in request.Education)
        {
            app.Education.Add(new ApplicationEducation
            {
                Id = Guid.NewGuid(),
                ApplicationId = app.Id,
                Level = edu.Level,
                Institution = edu.Institution,
                BoardUniversity = edu.Board,
                YearPassing = int.TryParse(edu.Year, out var yr) ? yr : null,
                Percentage = decimal.TryParse(edu.Percentage, out var pct) ? pct : null
            });
        }

        // Update experience (replace all)
        _context.ApplicationExperience.RemoveRange(app.Experience);
        foreach (var exp in request.Experience)
        {
            app.Experience.Add(new ApplicationExperience
            {
                Id = Guid.NewGuid(),
                ApplicationId = app.Id,
                Organization = exp.Organization,
                Designation = exp.Designation,
                FromDate = exp.FromDate,
                ToDate = exp.ToDate,
                Responsibilities = exp.Responsibilities
            });
        }

        // Update custom values (replace all)
        _context.ApplicationCustomValues.RemoveRange(app.CustomValues);
        foreach (var cv in request.CustomValues)
        {
            app.CustomValues.Add(new ApplicationCustomValue
            {
                Id = Guid.NewGuid(),
                ApplicationId = app.Id,
                FieldId = cv.Key,
                FieldValue = cv.Value
            });
        }

        await _context.SaveChangesAsync();

        return MapToFormDto(app);
    }

    public async Task<bool> SubmitApplicationAsync(Guid id)
    {
        var app = await _context.Applications.FindAsync(id);
        if (app == null) return false;

        app.Status = ApplicationStatus.Submitted;
        app.SubmittedAt = DateTime.UtcNow;
        app.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkUpdateStatusAsync(BulkStatusUpdateRequest request)
    {
        var applications = await _context.Applications
            .Where(a => request.ApplicationIds.Contains(a.Id))
            .ToListAsync();

        foreach (var app in applications)
        {
            app.Status = request.NewStatus;
            app.ScrutinyRemarks = request.Remarks;
            app.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalApplications = await _context.Applications.CountAsync(),
            PendingScrutiny = await _context.Applications.CountAsync(a => a.Status == ApplicationStatus.Submitted),
            Eligible = await _context.Applications.CountAsync(a => a.Status == ApplicationStatus.EligibleWritten || 
                                                                   a.Status == ApplicationStatus.EligiblePractical || 
                                                                   a.Status == ApplicationStatus.EligibleInterview),
            Selected = await _context.Applications.CountAsync(a => a.Status == ApplicationStatus.Selected),
            ActivePosts = await _context.JobPosts.CountAsync(p => p.Status == PostStatus.PUBLISHED),
            OpenTickets = await _context.SupportTickets.CountAsync(t => t.Status == TicketStatus.OPEN)
        };
    }

    public async Task<List<ApplicationTrendDto>> GetApplicationTrendsAsync()
    {
        var startDate = DateTime.UtcNow.AddDays(-7);
        var applications = await _context.Applications
            .Where(a => a.CreatedAt >= startDate)
            .GroupBy(a => a.CreatedAt.Date)
            .Select(g => new ApplicationTrendDto
            {
                Day = g.Key.DayOfWeek.ToString(),
                Count = g.Count()
            })
            .ToListAsync();

        return applications;
    }

    public async Task<List<CategoryDistributionDto>> GetCategoryDistributionAsync()
    {
        var distribution = await _context.ApplicationPersonalDetails
            .Where(p => p.Category != null)
            .GroupBy(p => p.Category)
            .Select(g => new CategoryDistributionDto
            {
                Category = g.Key.ToString()!,
                Count = g.Count()
            })
            .ToListAsync();

        return distribution;
    }

    private static ApplicationListItemDto MapToListItem(Application app) => new()
    {
        Id = app.Id,
        ApplicationNumber = app.ApplicationNumber,
        PostCode = app.Post?.Code ?? "",
        PostTitle = app.Post?.Title ?? "",
        Status = app.Status,
        SubmittedDate = app.SubmittedAt,
        ApplicantName = app.PersonalDetails?.FullName ?? app.User?.FullName ?? "",
        Category = app.PersonalDetails?.Category,
        Gender = app.PersonalDetails?.Gender?.ToString(),
        HighestQualification = app.Education.OrderByDescending(e => e.YearPassing).FirstOrDefault()?.Level,
        Percentage = app.Education.OrderByDescending(e => e.YearPassing).FirstOrDefault()?.Percentage
    };

    private static ApplicationFormDto MapToFormDto(Application app) => new()
    {
        Id = app.Id,
        ApplicationNumber = app.ApplicationNumber,
        PostId = app.PostId,
        PostTitle = app.Post?.Title,
        Status = app.Status,
        SubmittedDate = app.SubmittedAt,
        PersonalDetails = app.PersonalDetails != null ? new PersonalDetailsDto
        {
            FullName = app.PersonalDetails.FullName ?? "",
            DateOfBirth = app.PersonalDetails.DateOfBirth,
            Gender = app.PersonalDetails.Gender?.ToString(),
            Category = app.PersonalDetails.Category ?? Category.GEN,
            FatherName = app.PersonalDetails.FatherName,
            Mobile = "",
            Aadhaar = "",
            Address = app.PersonalDetails.Address,
            Nationality = app.PersonalDetails.Nationality
        } : new PersonalDetailsDto(),
        Education = app.Education.Select(e => new EducationEntryDto
        {
            Id = e.Id,
            Level = e.Level ?? "",
            Institution = e.Institution ?? "",
            Board = e.BoardUniversity ?? "",
            Year = e.YearPassing?.ToString() ?? "",
            Percentage = e.Percentage?.ToString() ?? ""
        }).ToList(),
        Experience = app.Experience.Select(e => new ExperienceEntryDto
        {
            Id = e.Id,
            Organization = e.Organization ?? "",
            Designation = e.Designation ?? "",
            FromDate = e.FromDate,
            ToDate = e.ToDate,
            Responsibilities = e.Responsibilities ?? ""
        }).ToList(),
        Documents = new DocumentsDto
        {
            PhotoUrl = app.Documents.FirstOrDefault(d => d.DocType == DocumentType.PHOTO)?.FilePath,
            SignatureUrl = app.Documents.FirstOrDefault(d => d.DocType == DocumentType.SIGNATURE)?.FilePath,
            ResumeUrl = app.Documents.FirstOrDefault(d => d.DocType == DocumentType.RESUME)?.FilePath,
            CasteCertificateUrl = app.Documents.FirstOrDefault(d => d.DocType == DocumentType.CASTE_CERT)?.FilePath
        },
        StatementOfPurpose = app.StatementOfPurpose,
        CustomValues = app.CustomValues.ToDictionary(cv => cv.FieldId, cv => cv.FieldValue ?? ""),
        Remarks = app.ScrutinyRemarks
    };
}
