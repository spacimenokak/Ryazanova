using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using projectTest.Models.DTO;
using projectTest.Services.Interfaces;

namespace projectTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<BookingDto>>> GetAll()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var bookings = await _bookingService.GetAllAsync(userRole);
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BookingDto>> GetById(Guid id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var booking = await _bookingService.GetByIdAsync(id, userRole, userId);
        if (booking == null)
            return NotFound();
        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<BookingDto>>> GetByUserId(Guid userId)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var bookings = await _bookingService.GetByUserIdAsync(userId, userRole, currentUserId);
        return Ok(bookings);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
        var booking = await _bookingService.CreateAsync(dto, userId, idempotencyKey);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BookingDto>> Update(Guid id, [FromBody] UpdateBookingDto dto)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var booking = await _bookingService.UpdateAsync(id, dto, userRole, userId);
        return Ok(booking);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var deleted = await _bookingService.DeleteAsync(id, userRole, userId);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
