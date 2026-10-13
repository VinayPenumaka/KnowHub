using KnowHub.Api.Data;
using KnowHub.Api.DTOs;
using KnowHub.Api.Models;
using KnowHub.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KnowHub.Api.Services;

public class LocalFileStorage(IWebHostEnvironment environment, IConfiguration configuration)
    : IFileStorage
{
    readonly string _root = Path.Combine(
        environment.ContentRootPath,
        configuration["Storage:UploadPath"] ?? "uploads"
    );

    public async Task<string> SaveAsync(IFormFile file, CancellationToken ct)
    {
        Directory.CreateDirectory(_root);
        var extension = Path.GetExtension(file.FileName);
        var allowed =
            configuration.GetSection("Storage:AllowedExtensions").Get<string[]>()
            ?? [".pdf", ".txt", ".docx"];
        if (!allowed.Contains(extension, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Only PDF, TXT, and DOCX files are supported.");
        var name = $"{Guid.NewGuid()}{extension}";
        await using var stream = File.Create(Path.Combine(_root, name));
        await file.CopyToAsync(stream, ct);
        return name;
    }

    public Task DeleteAsync(string path, CancellationToken ct)
    {
        var full = Path.Combine(_root, path);
        if (File.Exists(full))
            File.Delete(full);
        return Task.CompletedTask;
    }
}

public class DocumentService(AppDbContext db, IFileStorage storage, IActivityService activities)
    : IDocumentService
{
    static DocumentResponse Map(Document d) =>
        new(d.Id, d.ProjectId, d.FileName, d.FileType, d.FileSize, d.UploadedAt);

    async Task<Document> Find(Guid id, CancellationToken ct) =>
        await db.Documents.FindAsync([id], ct)
        ?? throw new KeyNotFoundException("Document not found.");

    public async Task<IReadOnlyList<DocumentResponse>> GetForProjectAsync(
        Guid pid,
        CancellationToken ct
    ) =>
        await db
            .Documents.Where(d => d.ProjectId == pid)
            .OrderByDescending(d => d.UploadedAt)
            .Select(d => new DocumentResponse(
                d.Id,
                d.ProjectId,
                d.FileName,
                d.FileType,
                d.FileSize,
                d.UploadedAt
            ))
            .ToListAsync(ct);

    public async Task<DocumentResponse> GetAsync(Guid id, CancellationToken ct) =>
        Map(await Find(id, ct));

    public async Task<DocumentResponse> UploadAsync(Guid pid, IFormFile file, CancellationToken ct)
    {
        if (!await db.Projects.AnyAsync(p => p.Id == pid, ct))
            throw new KeyNotFoundException("Project not found.");
        if (file.Length == 0)
            throw new ArgumentException("Choose a non-empty file.");
        var path = await storage.SaveAsync(file, ct);
        var d = new Document
        {
            ProjectId = pid,
            FileName = Path.GetFileName(file.FileName),
            FilePath = path,
            FileType = file.ContentType,
            FileSize = file.Length,
        };
        db.Documents.Add(d);
        await db.SaveChangesAsync(ct);
        await activities.AddAsync(
            pid,
            "DocumentUploaded",
            $"Uploaded document '{d.FileName}'.",
            ct
        );
        return Map(d);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var d = await Find(id, ct);
        await storage.DeleteAsync(d.FilePath, ct);
        db.Documents.Remove(d);
        await db.SaveChangesAsync(ct);
    }
}
