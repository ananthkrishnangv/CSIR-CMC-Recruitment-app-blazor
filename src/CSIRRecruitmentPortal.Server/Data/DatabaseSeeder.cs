using CSIRRecruitmentPortal.Server.Data;
using CSIRRecruitmentPortal.Shared.Entities;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Server.Data;

/// <summary>
/// Seeds the database with initial data
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Seed default users
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"),
                    FullName = "System Administrator (ICT)",
                    Email = "ictadmin",
                    Mobile = "9999999999",
                    Aadhaar = "111111111111",
                    Role = UserRole.ADMIN,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Dda5a3d52a#4815")
                },
                new User
                {
                    Id = Guid.Parse("B2C3D4E5-F6A7-8901-BCDE-F12345678901"),
                    FullName = "Administrative Officer",
                    Email = "admoff.serc@csir.res.in",
                    Mobile = "8888888888",
                    Aadhaar = "222222222222",
                    Role = UserRole.SUPERVISOR,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Serc@123456789")
                },
                new User
                {
                    Id = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"),
                    FullName = "Director CSIR-SERC",
                    Email = "director.serc@csir.res.in",
                    Mobile = "7777777777",
                    Aadhaar = "333333333333",
                    Role = UserRole.DIRECTOR,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Serc@123456789")
                }
            };

            context.Users.AddRange(users);
        }

        // Seed default job posts
        if (!context.JobPosts.Any())
        {
            var posts = new List<JobPost>
            {
                new JobPost
                {
                    Id = Guid.NewGuid(),
                    Code = "SCI-01-2024",
                    Title = "Scientist \"C\"",
                    Type = PostType.Scientist,
                    Department = "Structural Dynamics",
                    LastDate = DateTime.UtcNow.AddMonths(2),
                    Vacancies = 4,
                    Description = "Research in earthquake engineering and structural dynamics.",
                    Status = PostStatus.PUBLISHED,
                    CustomFields = new List<PostCustomField>
                    {
                        new PostCustomField
                        {
                            Id = Guid.NewGuid(),
                            Label = "Area of Specialization",
                            FieldType = FieldType.Text,
                            IsRequired = true,
                            Placeholder = "e.g., Seismic Design",
                            SortOrder = 1
                        },
                        new PostCustomField
                        {
                            Id = Guid.NewGuid(),
                            Label = "GATE Score",
                            FieldType = FieldType.Number,
                            IsRequired = false,
                            SortOrder = 2
                        }
                    }
                },
                new JobPost
                {
                    Id = Guid.NewGuid(),
                    Code = "TO-02-2024",
                    Title = "Technical Officer",
                    Type = PostType.TechnicalOfficer,
                    Department = "IT Infrastructure",
                    LastDate = DateTime.UtcNow.AddMonths(1),
                    Vacancies = 2,
                    Description = "Managing data center operations and network security.",
                    Status = PostStatus.PUBLISHED,
                    CustomFields = new List<PostCustomField>
                    {
                        new PostCustomField
                        {
                            Id = Guid.NewGuid(),
                            Label = "Certifications",
                            FieldType = FieldType.Dropdown,
                            IsRequired = true,
                            OptionsJson = "[\"CCNA\", \"AWS\", \"Azure\", \"None\"]",
                            SortOrder = 1
                        }
                    }
                },
                new JobPost
                {
                    Id = Guid.NewGuid(),
                    Code = "TA-01-2024",
                    Title = "Technical Assistant",
                    Type = PostType.TechnicalAssistant,
                    Department = "Material Testing",
                    LastDate = DateTime.UtcNow.AddMonths(2),
                    Vacancies = 10,
                    Description = "Assisting in lab testing of concrete and steel structures.",
                    Status = PostStatus.DIRECTOR_APPROVED
                },
                new JobPost
                {
                    Id = Guid.NewGuid(),
                    Code = "TECH-03-2024",
                    Title = "Technician (Electrical)",
                    Type = PostType.Technician,
                    Department = "Maintenance",
                    LastDate = DateTime.UtcNow.AddMonths(3),
                    Vacancies = 6,
                    Description = "Maintenance of electrical substations and campus wiring.",
                    Status = PostStatus.PENDING_DIRECTOR_APPROVAL
                }
            };

            context.JobPosts.AddRange(posts);
        }

        // Seed default news items
        if (!context.NewsItems.Any())
        {
            var newsItems = new List<NewsItem>
            {
                new NewsItem
                {
                    Id = Guid.NewGuid(),
                    Text = "Welcome to CSIR-SERC Recruitment Portal - Official Government Portal",
                    IsNew = false,
                    SortOrder = 1
                },
                new NewsItem
                {
                    Id = Guid.NewGuid(),
                    Text = "Last Date for Scientist 'C' (SCI-01-2024) extended to March 2025",
                    IsNew = true,
                    SortOrder = 2
                },
                new NewsItem
                {
                    Id = Guid.NewGuid(),
                    Text = "Download the latest GIGW 3.0 Compliance Guidelines from Helpdesk",
                    IsNew = false,
                    SortOrder = 3
                },
                new NewsItem
                {
                    Id = Guid.NewGuid(),
                    Text = "Interviews for Technical Officer posts scheduled for February 2025",
                    IsNew = false,
                    SortOrder = 4
                }
            };

            context.NewsItems.AddRange(newsItems);
        }

        // Seed default site configuration
        if (!context.SiteConfigs.Any())
        {
            var configs = new List<SiteConfig>
            {
                new SiteConfig { Key = "header.ministryText", Value = "MINISTRY OF SCIENCE & TECHNOLOGY" },
                new SiteConfig { Key = "header.organizationName", Value = "CSIR Madras Complex" },
                new SiteConfig { Key = "header.organizationSubtitle", Value = "Madras Complex" },
                new SiteConfig { Key = "header.parentOrganization", Value = "Council of Scientific & Industrial Research" },
                new SiteConfig { Key = "header.logoUrl", Value = "https://upload.wikimedia.org/wikipedia/en/2/22/CSIR_Logo.svg" },
                new SiteConfig { Key = "landing.heroImageUrl", Value = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c9/CSIR-SERC_Main_Building.jpg/1200px-CSIR-SERC_Main_Building.jpg" },
                new SiteConfig { Key = "footer.aboutText", Value = "Council of Scientific & Industrial Research - Madras Complex, Chennai. Pioneering advanced scientific research and solutions for the nation." },
                new SiteConfig { Key = "footer.address", Value = "CSIR Road, Taramani,\nChennai - 600 113\nIndia." },
                new SiteConfig { Key = "footer.copyrightText", Value = "© 2025 CSIR Madras Complex. All Rights Reserved. | Compliance to GIGW 3.0 | Noto Sans Font" },
                new SiteConfig { Key = "footer.contactEmail", Value = "recruit@serc.res.in" },
                new SiteConfig { Key = "assistance.title", Value = "Need Assistance?" },
                new SiteConfig { Key = "assistance.description", Value = "For technical queries regarding the online application portal, please contact our helpdesk. Check the FAQ section before raising a ticket." }
            };

            context.SiteConfigs.AddRange(configs);
        }

        await context.SaveChangesAsync();
    }
}
