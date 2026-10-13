using KnowHub.Api.DTOs;
using Microsoft.AspNetCore.Http;

namespace KnowHub.Api.Services.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken ct);
    Task<ProjectResponse> GetAsync(Guid id, CancellationToken ct);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, CancellationToken ct);
    Task<ProjectResponse> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface INoteService
{
    Task<IReadOnlyList<NoteResponse>> GetForProjectAsync(Guid projectId, CancellationToken ct);
    Task<NoteResponse> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<NoteResponse>> SearchAsync(string query, CancellationToken ct);
    Task<NoteResponse> CreateAsync(Guid projectId, CreateNoteRequest request, CancellationToken ct);
    Task<NoteResponse> UpdateAsync(Guid id, UpdateNoteRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetForProjectAsync(Guid projectId, CancellationToken ct);
    Task<TaskResponse> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken ct);
    Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface IDocumentService
{
    Task<IReadOnlyList<DocumentResponse>> GetForProjectAsync(Guid projectId, CancellationToken ct);
    Task<DocumentResponse> GetAsync(Guid id, CancellationToken ct);
    Task<DocumentResponse> UploadAsync(Guid projectId, IFormFile file, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface IActivityService
{
    Task<IReadOnlyList<ActivityResponse>> GetForProjectAsync(Guid projectId, CancellationToken ct);
    Task AddAsync(Guid projectId, string action, string description, CancellationToken ct);
}

public interface IFileStorage
{
    Task<string> SaveAsync(IFormFile file, CancellationToken ct);
    Task DeleteAsync(string relativePath, CancellationToken ct);
}
