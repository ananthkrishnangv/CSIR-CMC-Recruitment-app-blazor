using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CSIRRecruitmentPortal.Server.Services;
using CSIRRecruitmentPortal.Shared.DTOs;

namespace CSIRRecruitmentPortal.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfigService _configService;

    public ConfigController(IConfigService configService)
    {
        _configService = configService;
    }

    [HttpGet]
    public async Task<ActionResult<SiteConfigDto>> GetConfig()
    {
        return Ok(await _configService.GetSiteConfigAsync());
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public async Task<IActionResult> UpdateConfig([FromBody] SiteConfigDto config)
    {
        await _configService.UpdateSiteConfigAsync(config);
        return Ok(new { Message = "Configuration updated successfully" });
    }

    [HttpGet("news")]
    public async Task<ActionResult<List<NewsItemDto>>> GetNews()
    {
        return Ok(await _configService.GetNewsItemsAsync());
    }

    [HttpPost("news")]
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public async Task<ActionResult<NewsItemDto>> AddNews([FromBody] NewsItemDto item)
    {
        return Ok(await _configService.AddNewsItemAsync(item));
    }

    [HttpDelete("news/{id}")]
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public async Task<IActionResult> DeleteNews(Guid id)
    {
        var success = await _configService.DeleteNewsItemAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("upload")]
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public async Task<ActionResult<string>> UploadAsset(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Only JPG, PNG, and SVG are allowed.");

        // Ensure wwwroot/uploads exists
        var env = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var uploadPath = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative URL
        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        return Ok($"{baseUrl}/uploads/{fileName}");
    }
}
