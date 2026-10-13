using KnowHub.Api.DTOs;
using KnowHub.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KnowHub.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> Get(CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.GetAllAsync(ct)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Get(Guid id, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.GetAsync(id, ct)));

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest r, CancellationToken ct)
    {
        var p = await service.CreateAsync(r, ct);
        return CreatedAtAction(
            nameof(Get),
            new { id = p.Id },
            new ApiResponse<object>(true, p, "Project created.")
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(
        Guid id,
        UpdateProjectRequest r,
        CancellationToken ct
    ) =>
        Ok(new ApiResponse<object>(true, await service.UpdateAsync(id, r, ct), "Project updated."));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
