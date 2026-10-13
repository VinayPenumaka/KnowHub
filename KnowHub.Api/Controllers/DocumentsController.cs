using KnowHub.Api.DTOs;
using KnowHub.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KnowHub.Api.Controllers;

[ApiController]
public class DocumentsController(IDocumentService service) : ControllerBase
{
    [HttpGet("api/projects/{projectId:guid}/documents")]
    public async Task<IActionResult> ForProject(Guid projectId, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.GetForProjectAsync(projectId, ct)));

    [HttpGet("api/documents/{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.GetAsync(id, ct)));

    [HttpPost("api/projects/{projectId:guid}/documents")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(Guid projectId, IFormFile file, CancellationToken ct) =>
        StatusCode(
            201,
            new ApiResponse<object>(
                true,
                await service.UploadAsync(projectId, file, ct),
                "Document uploaded."
            )
        );

    [HttpDelete("api/documents/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
