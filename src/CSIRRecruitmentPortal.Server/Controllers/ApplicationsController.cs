using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CSIRRecruitmentPortal.Server.Services;
using CSIRRecruitmentPortal.Shared.DTOs;
using System.Security.Claims;

namespace CSIRRecruitmentPortal.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _appService;

    public ApplicationsController(IApplicationService appService)
    {
        _appService = appService;
    }

    [HttpGet("my-applications")]
    public async Task<ActionResult<List<ApplicationListItemDto>>> GetMyApplications()
    {
        var aadhaar = User.FindFirst("aadhaar")?.Value;
        if (string.IsNullOrEmpty(aadhaar)) return BadRequest("User context missing Aadhaar");

        return Ok(await _appService.GetApplicationsByUserAsync(aadhaar));
    }

    [HttpGet("post/{postId}")]
    [Authorize(Roles = "ADMIN,SUPERVISOR,DIRECTOR")]
    public async Task<ActionResult<List<ApplicationListItemDto>>> GetApplicationsByPost(Guid postId)
    {
        return Ok(await _appService.GetApplicationsByPostAsync(postId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationFormDto>> GetApplication(Guid id)
    {
        var app = await _appService.GetApplicationByIdAsync(id);
        if (app == null) return NotFound();

        // Applicants can only view their own applications
        // TODO: Implement ownership check here or in service
        
        return Ok(app);
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationFormDto>> CreateApplication([FromBody] ApplicationFormDto request)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var app = await _appService.CreateApplicationAsync(userId, request);
        return CreatedAtAction(nameof(GetApplication), new { id = app.Id }, app);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApplicationFormDto>> UpdateApplication(Guid id, [FromBody] ApplicationFormDto request)
    {
        var app = await _appService.UpdateApplicationAsync(id, request);
        if (app == null) return NotFound();
        return Ok(app);
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitApplication(Guid id)
    {
        var success = await _appService.SubmitApplicationAsync(id);
        if (!success) return NotFound();
        return Ok(new { Message = "Application submitted successfully" });
    }

    [HttpPost("bulk-status")]
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkStatusUpdateRequest request)
    {
        await _appService.BulkUpdateStatusAsync(request);
        return Ok(new { Message = "Status updated successfully" });
    }

    [HttpGet("dashboard-stats")]
    [Authorize(Roles = "ADMIN,SUPERVISOR,DIRECTOR")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        return Ok(await _appService.GetDashboardStatsAsync());
    }

    [HttpGet("trends")]
    [Authorize(Roles = "ADMIN,SUPERVISOR,DIRECTOR")]
    public async Task<ActionResult<List<ApplicationTrendDto>>> GetTrends()
    {
        return Ok(await _appService.GetApplicationTrendsAsync());
    }

    [HttpGet("demographics")]
    [Authorize(Roles = "ADMIN,SUPERVISOR,DIRECTOR")]
    public async Task<ActionResult<List<CategoryDistributionDto>>> GetDemographics()
    {
        return Ok(await _appService.GetCategoryDistributionAsync());
    }
}
