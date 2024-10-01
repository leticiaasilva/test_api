using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public UserController(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // GET /api/users with pagination, sorting, and filtering
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int perPage = 10,
                                              [FromQuery] string sortBy = "CreatedAt", [FromQuery] string order = "asc",
                                              [FromQuery] string status = null)
    {
        var query = _context.Users.AsQueryable();
        
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(u => u.Status == status);
        }

        // Sorting
        if (order.ToLower() == "desc")
        {
            query = query.OrderByDescending(u => EF.Property<object>(u, sortBy));
        }
        else
        {
            query = query.OrderBy(u => EF.Property<object>(u, sortBy));
        }

        // Pagination
        var users = await query.Skip((page - 1) * perPage).Take(perPage).ToListAsync();

        return Ok(users);
    }

    // GET /api/users/{id} to fetch details about a specific user
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // POST /api/users to create a new user
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // DELETE /api/users/{id} to delete a user
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET /api/users/statistics to get user statistics
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var verifiedCount = await _context.Users.CountAsync(u => u.Status == "verified");

        var today = DateTime.UtcNow.Date;
        var newUsersToday = await _context.Users.CountAsync(u => u.CreatedAt >= today);

        var yesterday = today.AddDays(-1);
        var newUsersYesterday = await _context.Users.CountAsync(u => u.CreatedAt >= yesterday && u.CreatedAt < today);

        return Ok(new
        {
            VerifiedCount = verifiedCount,
            NewUsersToday = newUsersToday,
            NewUsersYesterday = newUsersYesterday
        });
    }

    // POST /api/users/send_reminder to send reminder to unverified users
[HttpPost("send_reminder")]
public async Task<IActionResult> SendReminder()
{
    try
    {
        // Fetch users with "unverified" status
        var unverifiedUsers = await _context.Users
            .Where(u => u.Status == "unverified")
            .ToListAsync();

        if (!unverifiedUsers.Any())
        {
            return Ok("No unverified users found.");
        }

        foreach (var user in unverifiedUsers)
        {
            // Use the injected email service
            await _emailService.SendReminder(user.Email); 
        }

        return Ok($"{unverifiedUsers.Count} reminders sent.");
    }
    catch (Exception ex)
    {
        // Log the exception (consider using a logging framework)
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

}
