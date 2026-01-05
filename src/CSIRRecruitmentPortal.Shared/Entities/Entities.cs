using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Shared.Entities;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100), EmailAddress]
    public string? Email { get; set; }

    [Required, MaxLength(15)]
    public string Mobile { get; set; } = string.Empty;

    [MaxLength(12)]
    public string? Aadhaar { get; set; }

    [MaxLength(255)]
    public string? PasswordHash { get; set; }

    public UserRole Role { get; set; } = UserRole.APPLICANT;

    [MaxLength(255)]
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}

/// <summary>
/// Job post entity
/// </summary>
public class JobPost
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty; // e.g., SCI-01-2024

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public PostType Type { get; set; }

    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    public int Vacancies { get; set; }

    [Required]
    public DateTime LastDate { get; set; }

    public string? Description { get; set; }

    public PostStatus Status { get; set; } = PostStatus.DRAFT;

    [MaxLength(512)]
    public string? FinalResultPdfUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<PostCustomField> CustomFields { get; set; } = new List<PostCustomField>();
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}

/// <summary>
/// Custom field configuration for dynamic form builder
/// </summary>
public class PostCustomField
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PostId { get; set; }

    [Required, MaxLength(255)]
    public string Label { get; set; } = string.Empty;

    public FieldType FieldType { get; set; }

    public bool IsRequired { get; set; }

    [MaxLength(255)]
    public string? Placeholder { get; set; }

    /// <summary>
    /// JSON array of options for dropdown/radio fields
    /// </summary>
    public string? OptionsJson { get; set; }

    /// <summary>
    /// JSON object for validation rules: { "pattern": "regex", "minLength": 10, "errorMessage": "..." }
    /// </summary>
    public string? ValidationJson { get; set; }

    /// <summary>
    /// JSON object for conditional logic: { "dependsOnFieldId": "...", "condition": "EQUALS", "value": "..." }
    /// </summary>
    public string? LogicJson { get; set; }

    public int SortOrder { get; set; }

    // Navigation
    [ForeignKey(nameof(PostId))]
    public virtual JobPost Post { get; set; } = null!;

    public virtual ICollection<ApplicationCustomValue> CustomValues { get; set; } = new List<ApplicationCustomValue>();
}

/// <summary>
/// Application entity - links users to job posts
/// </summary>
public class Application
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(50)]
    public string ApplicationNumber { get; set; } = string.Empty; // e.g., SERC-2024-00001

    public Guid UserId { get; set; }
    public Guid PostId { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;

    public string? StatementOfPurpose { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string? ScrutinyRemarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    [ForeignKey(nameof(PostId))]
    public virtual JobPost Post { get; set; } = null!;

    public virtual ApplicationPersonalDetails? PersonalDetails { get; set; }
    public virtual ICollection<ApplicationEducation> Education { get; set; } = new List<ApplicationEducation>();
    public virtual ICollection<ApplicationExperience> Experience { get; set; } = new List<ApplicationExperience>();
    public virtual ICollection<ApplicationDocument> Documents { get; set; } = new List<ApplicationDocument>();
    public virtual ICollection<ApplicationCustomValue> CustomValues { get; set; } = new List<ApplicationCustomValue>();
}

/// <summary>
/// Personal details for an application
/// </summary>
public class ApplicationPersonalDetails
{
    [Key]
    public Guid ApplicationId { get; set; }

    [MaxLength(100)]
    public string? FullName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public Category? Category { get; set; }

    [MaxLength(100)]
    public string? FatherName { get; set; }

    public string? Address { get; set; }

    [MaxLength(50)]
    public string Nationality { get; set; } = "Indian";

    // Navigation
    [ForeignKey(nameof(ApplicationId))]
    public virtual Application Application { get; set; } = null!;
}

/// <summary>
/// Education history for an application
/// </summary>
public class ApplicationEducation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationId { get; set; }

    [MaxLength(50)]
    public string? Level { get; set; } // 10th, 12th, B.Tech, etc.

    [MaxLength(255)]
    public string? Institution { get; set; }

    [MaxLength(100)]
    public string? BoardUniversity { get; set; }

    public int? YearPassing { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Percentage { get; set; }

    // Navigation
    [ForeignKey(nameof(ApplicationId))]
    public virtual Application Application { get; set; } = null!;
}

/// <summary>
/// Experience history for an application
/// </summary>
public class ApplicationExperience
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationId { get; set; }

    [MaxLength(255)]
    public string? Organization { get; set; }

    [MaxLength(100)]
    public string? Designation { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? Responsibilities { get; set; }

    // Navigation
    [ForeignKey(nameof(ApplicationId))]
    public virtual Application Application { get; set; } = null!;
}

/// <summary>
/// Document uploads for an application
/// </summary>
public class ApplicationDocument
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationId { get; set; }

    public DocumentType DocType { get; set; }

    [Required, MaxLength(512)]
    public string FilePath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(ApplicationId))]
    public virtual Application Application { get; set; } = null!;
}

/// <summary>
/// Custom field values for an application
/// </summary>
public class ApplicationCustomValue
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationId { get; set; }

    public Guid FieldId { get; set; }

    public string? FieldValue { get; set; } // Stores text, number, or file path

    // Navigation
    [ForeignKey(nameof(ApplicationId))]
    public virtual Application Application { get; set; } = null!;

    [ForeignKey(nameof(FieldId))]
    public virtual PostCustomField Field { get; set; } = null!;
}

/// <summary>
/// Support ticket for helpdesk
/// </summary>
public class SupportTicket
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(20)]
    public string TicketNumber { get; set; } = string.Empty; // e.g., TKT-12345

    public Guid UserId { get; set; }

    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ApplicationNumber { get; set; }

    public Guid? PostId { get; set; }

    public TicketCategory Category { get; set; }

    [Required, MaxLength(255)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [MaxLength(512)]
    public string? AttachmentPath { get; set; }

    public TicketStatus Status { get; set; } = TicketStatus.OPEN;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<TicketReply> Replies { get; set; } = new List<TicketReply>();
}

/// <summary>
/// Reply to a support ticket
/// </summary>
public class TicketReply
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TicketId { get; set; }

    public Guid SenderId { get; set; }

    [MaxLength(100)]
    public string SenderName { get; set; } = string.Empty;

    public UserRole SenderRole { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(TicketId))]
    public virtual SupportTicket Ticket { get; set; } = null!;
}

/// <summary>
/// Site configuration entity for dynamic portal settings
/// </summary>
public class SiteConfig
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// News ticker item
/// </summary>
public class NewsItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Text { get; set; } = string.Empty;

    public bool IsNew { get; set; }

    [MaxLength(512)]
    public string? Link { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
