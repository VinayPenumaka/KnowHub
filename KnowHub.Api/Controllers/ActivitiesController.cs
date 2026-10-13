using KnowHub.Api.DTOs;
using KnowHub.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KnowHub.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/activities")]
public class ActivitiesController(IActivityService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid projectId, CancellationToken ct) =>
        Ok(new ApiResponse<object>(true, await service.GetForProjectAsync(projectId, ct)));
}
