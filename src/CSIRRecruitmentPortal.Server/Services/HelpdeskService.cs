using Microsoft.EntityFrameworkCore;
using CSIRRecruitmentPortal.Server.Data;
using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Entities;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Server.Services;

/// <summary>
/// Helpdesk service implementation
/// </summary>
public class HelpdeskService : IHelpdeskService
{
    private readonly AppDbContext _context;

    public HelpdeskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupportTicketDto>> GetTicketsByUserAsync(Guid userId)
    {
        var tickets = await _context.SupportTickets
            .Include(t => t.Replies.OrderBy(r => r.Timestamp))
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tickets.Select(MapToDto).ToList();
    }

    public async Task<List<SupportTicketDto>> GetAllTicketsAsync()
    {
        var tickets = await _context.SupportTickets
            .Include(t => t.Replies.OrderBy(r => r.Timestamp))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tickets.Select(MapToDto).ToList();
    }

    public async Task<SupportTicketDto?> GetTicketByIdAsync(Guid id)
    {
        var ticket = await _context.SupportTickets
            .Include(t => t.Replies.OrderBy(r => r.Timestamp))
            .FirstOrDefaultAsync(t => t.Id == id);

        return ticket != null ? MapToDto(ticket) : null;
    }

    public async Task<SupportTicketDto> CreateTicketAsync(Guid userId, CreateTicketRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new InvalidOperationException("User not found");

        // Generate ticket number
        var count = await _context.SupportTickets.CountAsync() + 1;
        var ticketNumber = $"TKT-{count:D5}";

        var ticket = new SupportTicket
        {
            Id = Guid.NewGuid(),
            TicketNumber = ticketNumber,
            UserId = userId,
            UserName = user.FullName,
            ApplicationNumber = request.ApplicationNumber,
            PostId = request.PostId,
            Category = request.Category,
            Subject = request.Subject,
            Description = request.Message,
            Status = TicketStatus.OPEN,
            CreatedAt = DateTime.UtcNow
        };

        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync();

        return MapToDto(ticket);
    }

    public async Task<SupportTicketDto?> ReplyToTicketAsync(Guid ticketId, Guid senderId, TicketReplyRequest request)
    {
        var ticket = await _context.SupportTickets
            .Include(t => t.Replies)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null) return null;

        var sender = await _context.Users.FindAsync(senderId);
        if (sender == null) return null;

        var reply = new TicketReply
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            SenderId = senderId,
            SenderName = sender.Role == UserRole.APPLICANT ? sender.FullName : "Administrative Officer",
            SenderRole = sender.Role,
            Message = request.Message,
            Timestamp = DateTime.UtcNow
        };

        ticket.Replies.Add(reply);
        ticket.Status = TicketStatus.OPEN; // Re-open if replied

        await _context.SaveChangesAsync();

        return MapToDto(ticket);
    }

    public async Task<bool> ResolveTicketAsync(Guid ticketId)
    {
        var ticket = await _context.SupportTickets.FindAsync(ticketId);
        if (ticket == null) return false;

        ticket.Status = TicketStatus.RESOLVED;
        await _context.SaveChangesAsync();
        return true;
    }

    private static SupportTicketDto MapToDto(SupportTicket ticket) => new()
    {
        Id = ticket.Id,
        TicketNumber = ticket.TicketNumber,
        UserName = ticket.UserName,
        ApplicationNumber = ticket.ApplicationNumber,
        Category = ticket.Category,
        Subject = ticket.Subject,
        Description = ticket.Description,
        Status = ticket.Status,
        CreatedAt = ticket.CreatedAt,
        Replies = ticket.Replies.Select(r => new TicketReplyDto
        {
            Id = r.Id,
            SenderName = r.SenderName,
            SenderRole = r.SenderRole,
            Message = r.Message,
            Timestamp = r.Timestamp
        }).ToList()
    };
}

/// <summary>
/// Configuration service implementation
/// </summary>
public class ConfigService : IConfigService
{
    private readonly AppDbContext _context;

    public ConfigService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SiteConfigDto> GetSiteConfigAsync()
    {
        var configs = await _context.SiteConfigs.ToDictionaryAsync(c => c.Key, c => c.Value);
        var news = await GetNewsItemsAsync();

        return new SiteConfigDto
        {
            Header = new HeaderConfigDto
            {
                MinistryText = configs.GetValueOrDefault("header.ministryText", "MINISTRY OF SCIENCE & TECHNOLOGY"),
                OrganizationName = configs.GetValueOrDefault("header.organizationName", "CSIR-SERC"),
                OrganizationSubtitle = configs.GetValueOrDefault("header.organizationSubtitle", "Structural Engineering Research Centre"),
                ParentOrganization = configs.GetValueOrDefault("header.parentOrganization", "Council of Scientific & Industrial Research"),
                LogoUrl = configs.GetValueOrDefault("header.logoUrl", "https://upload.wikimedia.org/wikipedia/en/2/22/CSIR_Logo.svg")
            },
            Footer = new FooterConfigDto
            {
                AboutText = configs.GetValueOrDefault("footer.aboutText", ""),
                Address = configs.GetValueOrDefault("footer.address", ""),
                CopyrightText = configs.GetValueOrDefault("footer.copyrightText", ""),
                ContactEmail = configs.GetValueOrDefault("footer.contactEmail", "")
            },
            Assistance = new AssistanceConfigDto
            {
                Title = configs.GetValueOrDefault("assistance.title", "Need Assistance?"),
                Description = configs.GetValueOrDefault("assistance.description", "")
            },
            Landing = new LandingConfigDto
            {
                HeroImageUrl = configs.GetValueOrDefault("landing.heroImageUrl", "")
            },
            News = news
        };
    }

    public async Task<bool> UpdateSiteConfigAsync(SiteConfigDto config)
    {
        var updates = new Dictionary<string, string>
        {
            ["header.ministryText"] = config.Header.MinistryText,
            ["header.organizationName"] = config.Header.OrganizationName,
            ["header.organizationSubtitle"] = config.Header.OrganizationSubtitle,
            ["header.parentOrganization"] = config.Header.ParentOrganization,
            ["header.logoUrl"] = config.Header.LogoUrl,
            ["footer.aboutText"] = config.Footer.AboutText,
            ["footer.address"] = config.Footer.Address,
            ["footer.copyrightText"] = config.Footer.CopyrightText,
            ["footer.contactEmail"] = config.Footer.ContactEmail,
            ["assistance.title"] = config.Assistance.Title,
            ["assistance.description"] = config.Assistance.Description,
            ["landing.heroImageUrl"] = config.Landing.HeroImageUrl
        };

        foreach (var update in updates)
        {
            var existing = await _context.SiteConfigs.FirstOrDefaultAsync(c => c.Key == update.Key);
            if (existing != null)
            {
                existing.Value = update.Value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.SiteConfigs.Add(new SiteConfig
                {
                    Id = Guid.NewGuid(),
                    Key = update.Key,
                    Value = update.Value,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<NewsItemDto>> GetNewsItemsAsync()
    {
        var items = await _context.NewsItems
            .Where(n => n.IsActive)
            .OrderBy(n => n.SortOrder)
            .ToListAsync();

        return items.Select(n => new NewsItemDto
        {
            Id = n.Id,
            Text = n.Text,
            IsNew = n.IsNew,
            Link = n.Link
        }).ToList();
    }

    public async Task<NewsItemDto> AddNewsItemAsync(NewsItemDto item)
    {
        var maxOrder = await _context.NewsItems.MaxAsync(n => (int?)n.SortOrder) ?? 0;

        var newsItem = new NewsItem
        {
            Id = Guid.NewGuid(),
            Text = item.Text,
            IsNew = item.IsNew,
            Link = item.Link,
            SortOrder = maxOrder + 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.NewsItems.Add(newsItem);
        await _context.SaveChangesAsync();

        return new NewsItemDto
        {
            Id = newsItem.Id,
            Text = newsItem.Text,
            IsNew = newsItem.IsNew,
            Link = newsItem.Link
        };
    }

    public async Task<bool> DeleteNewsItemAsync(Guid id)
    {
        var item = await _context.NewsItems.FindAsync(id);
        if (item == null) return false;

        _context.NewsItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
