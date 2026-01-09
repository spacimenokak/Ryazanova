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
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;
    private readonly ILogger<InstructorsController> _logger;

    public InstructorsController(IInstructorService instructorService, ILogger<InstructorsController> logger)
    {
        _instructorService = instructorService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<InstructorDto>>> GetAll()
    {
        var instructors = await _instructorService.GetAllAsync();
        return Ok(instructors);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorDto>> GetById(Guid id)
    {
        var instructor = await _instructorService.GetByIdAsync(id);
        if (instructor == null)
            return NotFound();
        return Ok(instructor);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<InstructorDto>> Create([FromBody] CreateInstructorDto dto)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var instructor = await _instructorService.CreateAsync(dto, userRole);
        return CreatedAtAction(nameof(GetById), new { id = instructor.Id }, instructor);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<InstructorDto>> Update(Guid id, [FromBody] UpdateInstructorDto dto)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var instructor = await _instructorService.UpdateAsync(id, dto, userRole);
        return Ok(instructor);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var deleted = await _instructorService.DeleteAsync(id, userRole);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
