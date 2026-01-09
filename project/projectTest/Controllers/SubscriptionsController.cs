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
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
    {
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<SubscriptionDto>>> GetAll()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var subscriptions = await _subscriptionService.GetAllAsync(userRole);
        return Ok(subscriptions);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SubscriptionDto>> GetById(Guid id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var subscription = await _subscriptionService.GetByIdAsync(id, userRole, userId);
        if (subscription == null)
            return NotFound();
        return Ok(subscription);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<SubscriptionDto>>> GetByUserId(Guid userId)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var subscriptions = await _subscriptionService.GetByUserIdAsync(userId, userRole, currentUserId);
        return Ok(subscriptions);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SubscriptionDto>> Create([FromBody] CreateSubscriptionDto dto)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var subscription = await _subscriptionService.CreateAsync(dto, userRole);
        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SubscriptionDto>> Update(Guid id, [FromBody] UpdateSubscriptionDto dto)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var subscription = await _subscriptionService.UpdateAsync(id, dto, userRole, userId);
        return Ok(subscription);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var deleted = await _subscriptionService.DeleteAsync(id, userRole);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
