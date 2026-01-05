# CSIR Madras Complex Recruitment Portal

A modern, comprehensive recruitment management system built for **CSIR Madras Complex** (formerly CSIR-SERC). This application streamlines the entire recruitment lifecycle from job posting to application scrutiny, offering a seamless experience for both applicants and administrators.

## 🚀 Key Features

### For Applicants
-   **Aadhaar-Based Authentication**: Secure login using 12-digit Aadhaar number.
-   **Interactive Dashboard**: Real-time tracking of application status (Draft, Submitted, Shortlisted, etc.).
-   **Multi-Step Application Form**:
    -   Personal Details, Education, Experience, and Document Uploads.
    -   Dynamic fields based on specific job requirements.
-   **Job Discovery**: Search and filter active job openings by department or keywords.
-   **Responsive Design**: Mobile-friendly interface with a modern Glassmorphism aesthetic.

### For Administrators
-   **Role-Based Access**: Specialized dashboards for Admin, Supervisor, and Director roles.
-   **Scrutiny Interface**: Review applications, view documents, and update statuses (Shortlist/Reject) with remarks.
-   **Job Post Management**: Create, edit, and publish job advertisements with custom field configuration.
-   **Site Configuration**: Manage branding (Logos, Banners) and helpdesk tickets directly from the portal.
-   **Reporting**: At-a-glance statistics on active posts and application counts.

## 🛠️ Technology Stack

This project is built using the latest **.NET 10** ecosystem, ensuring high performance, security, and maintainability.

### Frontend
-   **Framework**: [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) (.NET 10) - Single Page Application (SPA).
-   **Component Library**: [MudBlazor](https://mudblazor.com/) - Material Design components.
-   **Styling**:
    -   **Tailwind CSS**: For utility-first styling and layout.
    -   **Fluent UI** Design Principles: For typography and interactive elements.
    -   Custom Glassmorphism/Acrylic effects.
-   **State Management**: `AuthenticationStateProvider` for auth state, `Blazored.LocalStorage` for client-side persistence.

### Backend
-   **Framework**: [ASP.NET Core Web API](https://dotnet.microsoft.com/apps/aspnet/apis) (.NET 10).
-   **Database**: Microsoft SQL Server.
-   **ORM**: [Entity Framework Core](https://docs.microsoft.com/ef/core/) (Code-First approach).
-   **Authentication**: JSON Web Tokens (JWT) with Bearer authentication scheme.
-   **Services**: Dependency Injection used for all business logic (Auth, Applications, Posts, Notifications).

### DevOps & Deployment
-   **Server**: Windows Server with Internet Information Services (IIS).
-   **Automation**: PowerShell (`setup.ps1`) for automated environment configuration.
-   **API Documentation**: Swagger / OpenAPI (Swashbuckle).

## 📂 Project Structure

-   `src/CSIRRecruitmentPortal.Web`: Blazor WebAssembly Client.
-   `src/CSIRRecruitmentPortal.Server`: ASP.NET Core Web API & Database Layer.
-   `src/CSIRRecruitmentPortal.Shared`: Shared DTOs, Enums, and Models.

## 🔧 Setup & Installation

See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed installation instructions on Windows Server.

### Quick Start (Local)
1.  **Prerequisites**: .NET 10 SDK, SQL Server.
2.  **Database**: Update connection string in `appsettings.json`.
3.  **Run Server**:
    ```bash
    cd src/CSIRRecruitmentPortal.Server
    dotnet run
    ```
4.  The application will seed default data and launch at `https://localhost:7196`.

---
© 2026 CSIR Madras Complex. All Rights Reserved.
