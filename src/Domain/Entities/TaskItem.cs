using System;

namespace Domain.Entities;

public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2,
    Cancelled = 3
}

public class TaskItem
{
    private TaskItem() { } // Required by EF Core

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime DueDate { get; private set; }
    public string AssignedTo { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public static TaskItem Create(
        string title,
        string description,
        TaskPriority priority,
        DateTime dueDate,
        string assignedTo,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (dueDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Due date cannot be in the past.", nameof(dueDate));

        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Status = TaskStatus.Todo,
            Priority = priority,
            DueDate = dueDate,
            AssignedTo = assignedTo?.Trim() ?? string.Empty,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public void UpdateDetails(
        string title,
        string description,
        TaskPriority priority,
        DateTime dueDate,
        string assignedTo)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        Priority = priority;
        DueDate = dueDate;
        AssignedTo = assignedTo?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(TaskStatus newStatus)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update status of a deleted task.");

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
