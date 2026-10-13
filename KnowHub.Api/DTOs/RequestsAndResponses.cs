using System.ComponentModel.DataAnnotations;
using KnowHub.Api.Models;
using TaskStatus = KnowHub.Api.Models.TaskStatus;

namespace KnowHub.Api.DTOs;

public record ApiResponse<T>(bool Success, T? Data, string? Message = null);

public class CreateProjectRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateProjectRequest : CreateProjectRequest { }

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int NoteCount,
    int TaskCount
);

public class CreateNoteRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}

public class UpdateNoteRequest : CreateNoteRequest { }

public record NoteResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public class CreateTaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public DateTime? DueDate { get; set; }
}

public class UpdateTaskRequest : CreateTaskRequest { }

public record TaskResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record DocumentResponse(
    Guid Id,
    Guid ProjectId,
    string FileName,
    string FileType,
    long FileSize,
    DateTime UploadedAt
);

public record ActivityResponse(Guid Id, string Action, string Description, DateTime CreatedAt);
