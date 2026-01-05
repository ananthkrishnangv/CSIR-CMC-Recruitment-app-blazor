using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Server.Services;

public interface INotificationService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendSmsAsync(string mobile, string message);
    Task SendWhatsAppAsync(string mobile, string message);
    Task NotifyApplicationStatusChangeAsync(string email, string mobile, string applicationNo, ApplicationStatus newStatus);
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Placeholder for SMTP implementation
        _logger.LogInformation($"[EMAIL] To: {to} | Subject: {subject} | Body: {body}");
        await Task.CompletedTask;
    }

    public async Task SendSmsAsync(string mobile, string message)
    {
        // Placeholder for SMS Gateway
        _logger.LogInformation($"[SMS] To: {mobile} | Message: {message}");
        await Task.CompletedTask;
    }

    public async Task SendWhatsAppAsync(string mobile, string message)
    {
        // Placeholder for WhatsApp API
        _logger.LogInformation($"[WHATSAPP] To: {mobile} | Message: {message}");
        await Task.CompletedTask;
    }

    public async Task NotifyApplicationStatusChangeAsync(string email, string mobile, string applicationNo, ApplicationStatus newStatus)
    {
        var subject = $"Application Status Update - {applicationNo}";
        var body = $"Dear Candidate,\n\nYour application {applicationNo} status has been updated to: {newStatus}.\n\nRegards,\nCSIR Madras Complex Recruitment Team";
        
        await SendEmailAsync(email, subject, body);
        await SendSmsAsync(mobile, $"CSIR-SERC: Your application {applicationNo} status is now {newStatus}.");
    }
}
