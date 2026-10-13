using KnowHub.Api.DTOs;
using KnowHub.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KnowHub.Api.Controllers;

[ApiController]
public class TasksController(ITaskService service) : ControllerBase
{
    [HttpGet("api/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> ForProject(Guid projectId, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.GetForProjectAsync(projectId, ct)));

    [HttpPost("api/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> Create(
        Guid projectId,
        CreateTaskRequest r,
        CancellationToken ct
    ) =>
        StatusCode(
            201,
            new ApiResponse<object>(
                true,
                await service.CreateAsync(projectId, r, ct),
                "Task created."
            )
        );

    [HttpPut("api/tasks/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest r, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.UpdateAsync(id, r, ct), "Task updated."));

    [HttpDelete("api/tasks/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
