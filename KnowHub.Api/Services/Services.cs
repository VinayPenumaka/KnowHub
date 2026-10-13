using KnowHub.Api.Data;
using KnowHub.Api.DTOs;
using KnowHub.Api.Models;
using KnowHub.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskStatus = KnowHub.Api.Models.TaskStatus;

namespace KnowHub.Api.Services;

public class ActivityService(AppDbContext db) : IActivityService
{
    public async Task AddAsync(
        Guid projectId,
        string action,
        string description,
        CancellationToken ct
    )
    {
        db.Activities.Add(
            new Activity
            {
                ProjectId = projectId,
                Action = action,
                Description = description,
            }
        );
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ActivityResponse>> GetForProjectAsync(
        Guid projectId,
        CancellationToken ct
    ) =>
        await db
            .Activities.Where(a => a.ProjectId == projectId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ActivityResponse(a.Id, a.Action, a.Description, a.CreatedAt))
            .ToListAsync(ct);
}

public class ProjectService(AppDbContext db, IActivityService activities) : IProjectService
{
    static ProjectResponse Map(Project p) =>
        new(p.Id, p.Name, p.Description, p.CreatedAt, p.UpdatedAt, p.Notes.Count, p.Tasks.Count);

    public async Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken ct) =>
        (
            await db
                .Projects.Include(p => p.Notes)
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync(ct)
        )
            .Select(Map)
            .ToList();

    public async Task<ProjectResponse> GetAsync(Guid id, CancellationToken ct) =>
        Map(
            await db
                .Projects.Include(p => p.Notes)
                .Include(p => p.Tasks)
                .SingleOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new KeyNotFoundException("Project not found.")
        );

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest r, CancellationToken ct)
    {
        var user =
            await db.Users.FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException("No development user is available.");
        var p = new Project
        {
            UserId = user.Id,
            Name = r.Name,
            Description = r.Description,
        };
        db.Projects.Add(p);
        await db.SaveChangesAsync(ct);
        await activities.AddAsync(p.Id, "ProjectCreated", $"Created project '{p.Name}'.", ct);
        return await GetAsync(p.Id, ct);
    }

    public async Task<ProjectResponse> UpdateAsync(
        Guid id,
        UpdateProjectRequest r,
        CancellationToken ct
    )
    {
        var p =
            await db.Projects.SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Project not found.");
        p.Name = r.Name;
        p.Description = r.Description;
        p.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var p =
            await db.Projects.FindAsync([id], ct)
            ?? throw new KeyNotFoundException("Project not found.");
        db.Projects.Remove(p);
        await db.SaveChangesAsync(ct);
    }
}

public class NoteService(AppDbContext db, IActivityService activities) : INoteService
{
    static NoteResponse Map(Note n) =>
        new(n.Id, n.ProjectId, n.Title, n.Content, n.CreatedAt, n.UpdatedAt);

    async Task<Note> Find(Guid id, CancellationToken ct) =>
        await db.Notes.FindAsync([id], ct) ?? throw new KeyNotFoundException("Note not found.");

    public async Task<IReadOnlyList<NoteResponse>> GetForProjectAsync(
        Guid pid,
        CancellationToken ct
    ) =>
        await db
            .Notes.Where(n => n.ProjectId == pid)
            .OrderByDescending(n => n.UpdatedAt)
            .Select(n => new NoteResponse(
                n.Id,
                n.ProjectId,
                n.Title,
                n.Content,
                n.CreatedAt,
                n.UpdatedAt
            ))
            .ToListAsync(ct);

    public async Task<NoteResponse> GetAsync(Guid id, CancellationToken ct) =>
        Map(await Find(id, ct));

    public async Task<IReadOnlyList<NoteResponse>> SearchAsync(string q, CancellationToken ct) =>
        await db
            .Notes.Where(n =>
                EF.Functions.ILike(n.Title, $"%{q}%") || EF.Functions.ILike(n.Content, $"%{q}%")
            )
            .OrderByDescending(n => n.UpdatedAt)
            .Select(n => new NoteResponse(
                n.Id,
                n.ProjectId,
                n.Title,
                n.Content,
                n.CreatedAt,
                n.UpdatedAt
            ))
            .ToListAsync(ct);

    public async Task<NoteResponse> CreateAsync(Guid pid, CreateNoteRequest r, CancellationToken ct)
    {
        if (!await db.Projects.AnyAsync(p => p.Id == pid, ct))
            throw new KeyNotFoundException("Project not found.");
        var n = new Note
        {
            ProjectId = pid,
            Title = r.Title,
            Content = r.Content,
        };
        db.Notes.Add(n);
        await db.SaveChangesAsync(ct);
        await activities.AddAsync(pid, "NoteCreated", $"Created note '{n.Title}'.", ct);
        return Map(n);
    }

    public async Task<NoteResponse> UpdateAsync(Guid id, UpdateNoteRequest r, CancellationToken ct)
    {
        var n = await Find(id, ct);
        n.Title = r.Title;
        n.Content = r.Content;
        n.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await activities.AddAsync(n.ProjectId, "NoteUpdated", $"Updated note '{n.Title}'.", ct);
        return Map(n);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        db.Notes.Remove(await Find(id, ct));
        await db.SaveChangesAsync(ct);
    }
}

public class TaskService(AppDbContext db, IActivityService activities) : ITaskService
{
    static TaskResponse Map(TaskItem t) =>
        new(
            t.Id,
            t.ProjectId,
            t.Title,
            t.Description,
            t.Status,
            t.DueDate,
            t.CreatedAt,
            t.UpdatedAt
        );

    async Task<TaskItem> Find(Guid id, CancellationToken ct) =>
        await db.Tasks.FindAsync([id], ct) ?? throw new KeyNotFoundException("Task not found.");

    public async Task<IReadOnlyList<TaskResponse>> GetForProjectAsync(
        Guid pid,
        CancellationToken ct
    ) =>
        await db
            .Tasks.Where(t => t.ProjectId == pid)
            .OrderBy(t => t.Status)
            .Select(t => new TaskResponse(
                t.Id,
                t.ProjectId,
                t.Title,
                t.Description,
                t.Status,
                t.DueDate,
                t.CreatedAt,
                t.UpdatedAt
            ))
            .ToListAsync(ct);

    public async Task<TaskResponse> CreateAsync(Guid pid, CreateTaskRequest r, CancellationToken ct)
    {
        if (!await db.Projects.AnyAsync(p => p.Id == pid, ct))
            throw new KeyNotFoundException("Project not found.");
        var t = new TaskItem
        {
            ProjectId = pid,
            Title = r.Title,
            Description = r.Description,
            Status = r.Status,
            DueDate = r.DueDate,
        };
        db.Tasks.Add(t);
        await db.SaveChangesAsync(ct);
        await activities.AddAsync(pid, "TaskCreated", $"Created task '{t.Title}'.", ct);
        return Map(t);
    }

    public async Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest r, CancellationToken ct)
    {
        var t = await Find(id, ct);
        var becameCompleted = t.Status != TaskStatus.Completed && r.Status == TaskStatus.Completed;
        t.Title = r.Title;
        t.Description = r.Description;
        t.Status = r.Status;
        t.DueDate = r.DueDate;
        t.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        if (becameCompleted)
            await activities.AddAsync(
                t.ProjectId,
                "TaskCompleted",
                $"Completed task '{t.Title}'.",
                ct
            );
        return Map(t);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        db.Tasks.Remove(await Find(id, ct));
        await db.SaveChangesAsync(ct);
    }
}
