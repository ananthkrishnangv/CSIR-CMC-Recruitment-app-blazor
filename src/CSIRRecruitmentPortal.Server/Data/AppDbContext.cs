using Microsoft.EntityFrameworkCore;
using CSIRRecruitmentPortal.Shared.Entities;

namespace CSIRRecruitmentPortal.Server.Data;

/// <summary>
/// Entity Framework Core DbContext for the CSIR Recruitment Portal
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Core entities
    public DbSet<User> Users => Set<User>();
    public DbSet<JobPost> JobPosts => Set<JobPost>();
    public DbSet<PostCustomField> PostCustomFields => Set<PostCustomField>();
    public DbSet<Application> Applications => Set<Application>();
    
    // Application details
    public DbSet<ApplicationPersonalDetails> ApplicationPersonalDetails => Set<ApplicationPersonalDetails>();
    public DbSet<ApplicationEducation> ApplicationEducation => Set<ApplicationEducation>();
    public DbSet<ApplicationExperience> ApplicationExperience => Set<ApplicationExperience>();
    public DbSet<ApplicationDocument> ApplicationDocuments => Set<ApplicationDocument>();
    public DbSet<ApplicationCustomValue> ApplicationCustomValues => Set<ApplicationCustomValue>();
    
    // Support
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<TicketReply> TicketReplies => Set<TicketReply>();
    
    // Configuration
    public DbSet<SiteConfig> SiteConfigs => Set<SiteConfig>();
    public DbSet<NewsItem> NewsItems => Set<NewsItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Aadhaar).IsUnique();
            entity.HasIndex(e => e.Mobile);
        });

        // JobPost configuration
        modelBuilder.Entity<JobPost>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasMany(e => e.CustomFields)
                  .WithOne(e => e.Post)
                  .HasForeignKey(e => e.PostId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Application configuration
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasIndex(e => e.ApplicationNumber).IsUnique();
            entity.HasIndex(e => new { e.UserId, e.PostId }).IsUnique(); // One application per post per user
            
            entity.HasOne(e => e.PersonalDetails)
                  .WithOne(e => e.Application)
                  .HasForeignKey<ApplicationPersonalDetails>(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ApplicationEducation
        modelBuilder.Entity<ApplicationEducation>(entity =>
        {
            entity.HasOne(e => e.Application)
                  .WithMany(e => e.Education)
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ApplicationExperience
        modelBuilder.Entity<ApplicationExperience>(entity =>
        {
            entity.HasOne(e => e.Application)
                  .WithMany(e => e.Experience)
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ApplicationDocument
        modelBuilder.Entity<ApplicationDocument>(entity =>
        {
            entity.HasOne(e => e.Application)
                  .WithMany(e => e.Documents)
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ApplicationCustomValue
        modelBuilder.Entity<ApplicationCustomValue>(entity =>
        {
            entity.HasOne(e => e.Application)
                  .WithMany(e => e.CustomValues)
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Field)
                  .WithMany(e => e.CustomValues)
                  .HasForeignKey(e => e.FieldId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // SupportTicket
        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasIndex(e => e.TicketNumber).IsUnique();
            entity.HasMany(e => e.Replies)
                  .WithOne(e => e.Ticket)
                  .HasForeignKey(e => e.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // SiteConfig
        modelBuilder.Entity<SiteConfig>(entity =>
        {
            entity.HasIndex(e => e.Key).IsUnique();
        });

        // NewsItem
        modelBuilder.Entity<NewsItem>(entity =>
        {
            entity.HasIndex(e => e.SortOrder);
        });
    }
}
