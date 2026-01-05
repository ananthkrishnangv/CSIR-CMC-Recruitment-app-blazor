namespace CSIRRecruitmentPortal.Shared.Enums;

/// <summary>
/// User roles in the recruitment portal
/// </summary>
public enum UserRole
{
    GUEST,
    APPLICANT,
    ADMIN,
    SUPERVISOR,
    DIRECTOR
}

/// <summary>
/// Types of job posts available
/// </summary>
public enum PostType
{
    Scientist,
    TechnicalOfficer,
    Technician,
    TechnicalAssistant
}

/// <summary>
/// Status of a job post
/// </summary>
public enum PostStatus
{
    DRAFT,
    PUBLISHED,
    APPLICATION_CLOSED,
    SCRUTINY_IN_PROGRESS,
    PENDING_DIRECTOR_APPROVAL,
    DIRECTOR_APPROVED,
    RESULT_DECLARED
}

/// <summary>
/// Status of an application
/// </summary>
public enum ApplicationStatus
{
    Draft,
    Submitted,
    UnderScrutiny,
    ScrutinyCompleted,
    EligibleWritten,
    EligiblePractical,
    EligibleInterview,
    NotEligible,
    Selected,
    Rejected
}

/// <summary>
/// Reservation category
/// </summary>
public enum Category
{
    GEN,
    OBC,
    SC,
    ST,
    EWS,
    PWD
}

/// <summary>
/// Gender options
/// </summary>
public enum Gender
{
    Male,
    Female,
    Other
}

/// <summary>
/// Types of custom form fields
/// </summary>
public enum FieldType
{
    Text,
    Number,
    Date,
    Dropdown,
    Textarea,
    Checkbox,
    Radio,
    File
}

/// <summary>
/// Document types for application
/// </summary>
public enum DocumentType
{
    PHOTO,
    SIGNATURE,
    RESUME,
    CASTE_CERT,
    OTHER
}

/// <summary>
/// Support ticket status
/// </summary>
public enum TicketStatus
{
    OPEN,
    RESOLVED,
    CLOSED
}

/// <summary>
/// Support ticket categories
/// </summary>
public enum TicketCategory
{
    ApplicationIssue,
    DocumentUpload,
    PhotoUpload,
    PDFDownload,
    Payment,
    Other
}
