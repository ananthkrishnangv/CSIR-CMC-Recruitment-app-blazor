using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CSIRRecruitmentPortal.Server.Services;
using CSIRRecruitmentPortal.Shared.DTOs;
using System.Security.Claims;

namespace CSIRRecruitmentPortal.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HelpdeskController : ControllerBase
{
    private readonly IHelpdeskService _helpdeskService;

    public HelpdeskController(IHelpdeskService helpdeskService)
    {
        _helpdeskService = helpdeskService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SupportTicketDto>>> GetTickets()
    {
        // Admins see all, applicants see theirs
        if (User.IsInRole("ADMIN") || User.IsInRole("SUPERVISOR"))
        {
            return Ok(await _helpdeskService.GetAllTicketsAsync());
        }

        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        return Ok(await _helpdeskService.GetTicketsByUserAsync(userId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupportTicketDto>> GetTicket(Guid id)
    {
        var ticket = await _helpdeskService.GetTicketByIdAsync(id);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<SupportTicketDto>> CreateTicket([FromBody] CreateTicketRequest request)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var ticket = await _helpdeskService.CreateTicketAsync(userId, request);
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPost("{id}/reply")]
    public async Task<ActionResult<SupportTicketDto>> ReplyTicket(Guid id, [FromBody] TicketReplyRequest request)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var ticket = await _helpdeskService.ReplyToTicketAsync(id, userId, request);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    [HttpPost("{id}/resolve")]
    public async Task<IActionResult> ResolveTicket(Guid id)
    {
        var success = await _helpdeskService.ResolveTicketAsync(id);
        if (!success) return NotFound();
        return Ok(new { Message = "Ticket resolved" });
    }
}
