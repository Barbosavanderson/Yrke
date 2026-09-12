using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Yrke.Data;
using Yrke.Models;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotificationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var nots = _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new {
                id = n.Id,
                title = n.Title,
                message = n.Message,
                link = n.Link,
                isRead = n.IsRead,
                createdAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            })
            .ToList();

        return Ok(nots);
    }

    [HttpPost("markread/{id}")]
    public IActionResult MarkRead(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var noti = _context.Notifications.FirstOrDefault(n => n.Id == id && n.UserId == userId);
        if (noti == null) return NotFound();

        noti.IsRead = true;
        _context.SaveChanges();
        return Ok();
    }
}
