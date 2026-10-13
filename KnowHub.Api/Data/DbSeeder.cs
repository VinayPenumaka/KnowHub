using KnowHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using TaskStatus = KnowHub.Api.Models.TaskStatus;

namespace KnowHub.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();
        if (await db.Users.AnyAsync())
            return;
        var user = new User
        {
            Name = "Vinay",
            Email = "vinay@example.com",
            PasswordHash = "development-only",
        };
        var python = new Project
        {
            User = user,
            Name = "Python Learning",
            Description = "Learn Python APIs",
        };
        var fullstack = new Project
        {
            User = user,
            Name = "Full Stack Development",
            Description = "Build web applications",
        };
        var ai = new Project
        {
            User = user,
            Name = "AI Learning",
            Description = "Learn AI concepts later",
        };
        db.AddRange(user, python, fullstack, ai);
        db.Notes.AddRange(
            new Note
            {
                Project = python,
                Title = "What is an API?",
                Content = "An API is a contract that lets applications communicate.",
            },
            new Note
            {
                Project = fullstack,
                Title = "Understanding HTTP",
                Content = "HTTP uses requests, responses, verbs, and status codes.",
            },
            new Note
            {
                Project = python,
                Title = "Python project structure",
                Content = "Keep packages focused and dependencies explicit.",
            }
        );
        db.Tasks.AddRange(
            new TaskItem { Project = python, Title = "Build first API" },
            new TaskItem
            {
                Project = fullstack,
                Title = "Learn dependency injection",
                Status = TaskStatus.InProgress,
            },
            new TaskItem { Project = fullstack, Title = "Create PostgreSQL database" }
        );
        db.Activities.Add(
            new Activity
            {
                Project = python,
                Action = "ProjectCreated",
                Description = "Created project 'Python Learning'.",
            }
        );
        await db.SaveChangesAsync();
    }
}
