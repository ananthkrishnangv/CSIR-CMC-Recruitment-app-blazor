using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CSIRRecruitmentPortal.Server.Services;
using CSIRRecruitmentPortal.Shared.DTOs;
using CSIRRecruitmentPortal.Shared.Enums;

namespace CSIRRecruitmentPortal.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobPostDto>>> GetAllPosts()
    {
        // If public user, only return published posts
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Ok(await _postService.GetPublishedPostsAsync());
        }
        
        // If admin/official, return all posts
        if (User.IsInRole(nameof(UserRole.ADMIN)) || 
            User.IsInRole(nameof(UserRole.SUPERVISOR)) || 
            User.IsInRole(nameof(UserRole.DIRECTOR)))
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        // Default to published for applicants
        return Ok(await _postService.GetPublishedPostsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobPostDto>> GetPostById(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public async Task<ActionResult<JobPostDto>> CreatePost([FromBody] CreatePostRequest request)
    {
        var post = await _postService.CreatePostAsync(request);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN,SUPERVISOR")]
    public async Task<ActionResult<JobPostDto>> UpdatePost(Guid id, [FromBody] CreatePostRequest request)
    {
        var post = await _postService.UpdatePostAsync(id, request);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var success = await _postService.DeletePostAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "ADMIN,SUPERVISOR,DIRECTOR")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] PostStatus status, [FromQuery] string? resultPdfUrl = null)
    {
        var success = await _postService.UpdatePostStatusAsync(id, status, resultPdfUrl);
        if (!success) return NotFound();
        return NoContent();
    }
}
