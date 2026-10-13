using KnowHub.Api.DTOs;
using KnowHub.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KnowHub.Api.Controllers;

[ApiController]
public class NotesController(INoteService service) : ControllerBase
{
    [HttpGet("api/projects/{projectId:guid}/notes")]
    public async Task<IActionResult> ForProject(Guid projectId, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.GetForProjectAsync(projectId, ct)));

    [HttpGet("api/notes/{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.GetAsync(id, ct)));

    [HttpGet("api/notes/search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(
                new ApiResponse<object>(false, null, "Query parameter q is required.")
            );
        return Ok(new ApiResponse<object>(true, await service.SearchAsync(q, ct)));
    }

    [HttpPost("api/projects/{projectId:guid}/notes")]
    public async Task<IActionResult> Create(
        Guid projectId,
        CreateNoteRequest r,
        CancellationToken ct
    )
    {
        var n = await service.CreateAsync(projectId, r, ct);
        return CreatedAtAction(
            nameof(Get),
            new { id = n.Id },
            new ApiResponse<object>(true, n, "Note created.")
        );
    }

    [HttpPut("api/notes/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateNoteRequest r, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.UpdateAsync(id, r, ct), "Note updated."));

    [HttpDelete("api/notes/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
