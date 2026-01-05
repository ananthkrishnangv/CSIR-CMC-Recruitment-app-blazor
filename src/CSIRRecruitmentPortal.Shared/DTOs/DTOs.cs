using CSIRRecruitmentPortal.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace CSIRRecruitmentPortal.Shared.DTOs;

#region Authentication DTOs

/// <summary>
/// Request for applicant login using Aadhaar
/// </summary>
public class ApplicantLoginRequest
{
    [Required(ErrorMessage = "Aadhaar number is required")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhaar must be exactly 12 digits")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar must contain only digits")]
    public string Aadhaar { get; set; } = string.Empty;
}

/// <summary>
/// Request for admin/official login
/// </summary>
public class AdminLoginRequest
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Request for new user registration
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile must be exactly 10 digits")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile must contain only digits")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "Aadhaar number is required")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhaar must be exactly 12 digits")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar must contain only digits")]
    public string Aadhaar { get; set; } = string.Empty;
}

/// <summary>
/// Response after successful authentication
/// </summary>
public class AuthResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public UserDto? User { get; set; }
    public string? Token { get; set; }
}

/// <summary>
/// User data transfer object
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? Aadhaar { get; set; }
    public UserRole Role { get; set; }
    public string? AvatarUrl { get; set; }
}

#endregion

#region Job Post DTOs

/// <summary>
/// Job post data transfer object
/// </summary>
public class JobPostDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public PostType Type { get; set; }
    public string Department { get; set; } = string.Empty;
    public int Vacancies { get; set; }
    public DateTime LastDate { get; set; }
    public string? Description { get; set; }
    public PostStatus Status { get; set; }
    public string? FinalResultPdfUrl { get; set; }
    public List<CustomFieldDto> CustomFields { get; set; } = new();
}

/// <summary>
/// Custom field data transfer object
/// </summary>
public class CustomFieldDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public bool Required { get; set; }
    public string? Placeholder { get; set; }
    public List<string>? Options { get; set; }
    public FieldValidationDto? Validation { get; set; }
    public FieldLogicDto? Logic { get; set; }
}

public class FieldValidationDto
{
    public string? Pattern { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? ErrorMessage { get; set; }
}

public class FieldLogicDto
{
    public Guid DependsOnFieldId { get; set; }
    public string Condition { get; set; } = "EQUALS"; // EQUALS, NOT_EQUALS, CONTAINS
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Request to create a new job post
/// </summary>
public class CreatePostRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    public PostType Type { get; set; }

    [Required]
    public string Department { get; set; } = string.Empty;

    public int Vacancies { get; set; } = 1;

    [Required]
    public DateTime LastDate { get; set; }

    public string? Description { get; set; }

    public List<CustomFieldDto> CustomFields { get; set; } = new();
}

#endregion

#region Application DTOs

/// <summary>
/// Application form data transfer object
/// </summary>
public class ApplicationFormDto
{
    public Guid? Id { get; set; }
    public string? ApplicationNumber { get; set; }
    public Guid PostId { get; set; }
    public string? PostTitle { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime? SubmittedDate { get; set; }

    public PersonalDetailsDto PersonalDetails { get; set; } = new();
    public List<EducationEntryDto> Education { get; set; } = new();
    public List<ExperienceEntryDto> Experience { get; set; } = new();
    public List<string> Publications { get; set; } = new();
    public DocumentsDto Documents { get; set; } = new();
    public string? StatementOfPurpose { get; set; }
    public Dictionary<Guid, string> CustomValues { get; set; } = new();
    public string? Remarks { get; set; }
}

public class PersonalDetailsDto
{
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Category Category { get; set; }
    public string? FatherName { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string Aadhaar { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Nationality { get; set; } = "Indian";
}

public class EducationEntryDto
{
    public Guid Id { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public string Board { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Percentage { get; set; } = string.Empty;
}

public class ExperienceEntryDto
{
    public Guid Id { get; set; }
    public string Organization { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string Responsibilities { get; set; } = string.Empty;
}

public class DocumentsDto
{
    public string? PhotoUrl { get; set; }
    public string? SignatureUrl { get; set; }
    public string? ResumeUrl { get; set; }
    public string? CasteCertificateUrl { get; set; }
}

/// <summary>
/// Application list item for dashboard
/// </summary>
public class ApplicationListItemDto
{
    public Guid Id { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public string PostTitle { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public Category? Category { get; set; }
    public string? Gender { get; set; }
    public string? HighestQualification { get; set; }
    public decimal? Percentage { get; set; }
}

/// <summary>
/// Bulk status update request
/// </summary>
public class BulkStatusUpdateRequest
{
    public List<Guid> ApplicationIds { get; set; } = new();
    public ApplicationStatus NewStatus { get; set; }
    public string? Remarks { get; set; }
}

#endregion

#region Helpdesk DTOs

/// <summary>
/// Support ticket data transfer object
/// </summary>
public class SupportTicketDto
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ApplicationNumber { get; set; }
    public TicketCategory Category { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TicketReplyDto> Replies { get; set; } = new();
}

public class TicketReplyDto
{
    public Guid Id { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public UserRole SenderRole { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Request to create a new support ticket
/// </summary>
public class CreateTicketRequest
{
    public string? ApplicationNumber { get; set; }
    public Guid? PostId { get; set; }
    public TicketCategory Category { get; set; }

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Request to reply to a ticket
/// </summary>
public class TicketReplyRequest
{
    [Required]
    public string Message { get; set; } = string.Empty;
}

#endregion

#region Site Configuration DTOs

/// <summary>
/// Site configuration data transfer object
/// </summary>
public class SiteConfigDto
{
    public HeaderConfigDto Header { get; set; } = new();
    public FooterConfigDto Footer { get; set; } = new();
    public AssistanceConfigDto Assistance { get; set; } = new();
    public LandingConfigDto Landing { get; set; } = new();
    public SmtpConfigDto Smtp { get; set; } = new();
    public NotificationConfigDto Notifications { get; set; } = new();
    public List<NewsItemDto> News { get; set; } = new();
}

public class HeaderConfigDto
{
    public string MinistryText { get; set; } = "MINISTRY OF SCIENCE & TECHNOLOGY";
    public string OrganizationName { get; set; } = "CSIR-SERC";
    public string OrganizationSubtitle { get; set; } = "Structural Engineering Research Centre";
    public string ParentOrganization { get; set; } = "Council of Scientific & Industrial Research";
    public string LogoUrl { get; set; } = "https://upload.wikimedia.org/wikipedia/en/2/22/CSIR_Logo.svg";
}

public class FooterConfigDto
{
    public string AboutText { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CopyrightText { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public List<LinkItemDto> QuickLinks { get; set; } = new();
    public List<LinkItemDto> SupportLinks { get; set; } = new();
}

public class AssistanceConfigDto
{
    public string Title { get; set; } = "Need Assistance?";
    public string Description { get; set; } = string.Empty;
}

public class LandingConfigDto
{
    public string HeroImageUrl { get; set; } = string.Empty;
}

public class SmtpConfigDto
{
    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string User { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
}

public class NotificationConfigDto
{
    public bool SmsEnabled { get; set; }
    public bool WhatsappEnabled { get; set; }
}

public class LinkItemDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class NewsItemDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsNew { get; set; }
    public string? Link { get; set; }
}

#endregion

#region Dashboard DTOs

/// <summary>
/// Dashboard statistics
/// </summary>
public class DashboardStatsDto
{
    public int TotalApplications { get; set; }
    public int PendingScrutiny { get; set; }
    public int Eligible { get; set; }
    public int Selected { get; set; }
    public int ActivePosts { get; set; }
    public int OpenTickets { get; set; }
}

/// <summary>
/// Application trend data for charts
/// </summary>
public class ApplicationTrendDto
{
    public string Day { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// Category distribution for charts
/// </summary>
public class CategoryDistributionDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}

#endregion
