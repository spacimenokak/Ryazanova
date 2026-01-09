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
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;
    private readonly ILogger<ClassesController> _logger;

    public ClassesController(IClassService classService, ILogger<ClassesController> logger)
    {
        _classService = classService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<ClassDto>>> GetAll(
        [FromQuery] ClassFilterDto filter,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await _classService.GetFilteredAsync(filter, page, pageSize);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassDto>> GetById(Guid id)
    {
        var classEntity = await _classService.GetByIdAsync(id);
        if (classEntity == null)
            return NotFound();
        return Ok(classEntity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ClassDto>> Create([FromBody] CreateClassDto dto)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var classEntity = await _classService.CreateAsync(dto, userRole);
        return CreatedAtAction(nameof(GetById), new { id = classEntity.Id }, classEntity);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ClassDto>> Update(Guid id, [FromBody] UpdateClassDto dto)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var classEntity = await _classService.UpdateAsync(id, dto, userRole);
        return Ok(classEntity);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var deleted = await _classService.DeleteAsync(id, userRole);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
