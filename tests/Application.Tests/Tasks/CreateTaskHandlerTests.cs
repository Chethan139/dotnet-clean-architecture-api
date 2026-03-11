using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Tasks.Commands.CreateTask;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Tasks;

public sealed class CreateTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();

        _currentUserMock.Setup(x => x.UserName).Returns("testuser@example.com");
        _currentUserMock.Setup(x => x.UserId).Returns("user-123");
        _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);

        _handler = new CreateTaskHandler(_repositoryMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsTaskDto()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Title: "Implement login feature",
            Description: "Add OAuth2 login with Google provider.",
            Priority: TaskPriority.High,
            DueDate: DateTime.UtcNow.AddDays(5),
            AssignedTo: "dev@company.com");

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be("Implement login feature");
        result.Description.Should().Be("Add OAuth2 login with Google provider.");
        result.Priority.Should().Be("High");
        result.Status.Should().Be("Todo");
        result.AssignedTo.Should().Be("dev@company.com");
        result.CreatedBy.Should().Be("testuser@example.com");
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryAddOnce()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Title: "Write unit tests",
            Description: "Cover all handler classes with unit tests.",
            Priority: TaskPriority.Medium,
            DueDate: DateTime.UtcNow.AddDays(3),
            AssignedTo: "qa@company.com");

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NoAuthenticatedUser_UsesSystemAsCreatedBy()
    {
        // Arrange
        _currentUserMock.Setup(x => x.UserName).Returns((string?)null);

        var command = new CreateTaskCommand(
            Title: "Background job task",
            Description: "Automated task created by the system.",
            Priority: TaskPriority.Low,
            DueDate: DateTime.UtcNow.AddDays(10),
            AssignedTo: string.Empty);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.CreatedBy.Should().Be("system");
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsStatusToTodo()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Title: "Deploy to staging",
            Description: "Deploy the latest build to the staging environment.",
            Priority: TaskPriority.Critical,
            DueDate: DateTime.UtcNow.AddDays(1),
            AssignedTo: "devops@company.com");

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Todo");
    }

    [Fact]
    public async Task Handle_ValidCommand_AssignsNewGuidId()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Title: "Code review",
            Description: "Review PR #142.",
            Priority: TaskPriority.Medium,
            DueDate: DateTime.UtcNow.AddDays(2),
            AssignedTo: "senior@company.com");

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);

        // Assert — each call creates a unique ID
        result1.Id.Should().NotBe(result2.Id);
    }

    [Fact]
    public async Task Handle_CancellationRequested_PropagatesCancellation()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Title: "Cancellable task",
            Description: "This task is used to test cancellation.",
            Priority: TaskPriority.Low,
            DueDate: DateTime.UtcNow.AddDays(7),
            AssignedTo: string.Empty);

        var cts = new CancellationTokenSource();

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns<TaskItem, CancellationToken>((_, token) =>
            {
                token.ThrowIfCancellationRequested();
                return Task.CompletedTask;
            });

        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(command, cts.Token));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void TaskItem_Create_WithEmptyTitle_ThrowsArgumentException(string invalidTitle)
    {
        // Arrange & Act & Assert
        var act = () => TaskItem.Create(
            invalidTitle,
            "Some description",
            TaskPriority.Low,
            DateTime.UtcNow.AddDays(1),
            "user@company.com",
            "admin");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title*");
    }

    [Fact]
    public void TaskItem_Create_WithPastDueDate_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        var act = () => TaskItem.Create(
            "Valid title",
            "Description",
            TaskPriority.Medium,
            DateTime.UtcNow.AddDays(-1), // past date
            "user@company.com",
            "admin");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Due date*");
    }

    [Fact]
    public void TaskItem_SoftDelete_SetsIsDeletedFlag()
    {
        // Arrange
        var task = TaskItem.Create(
            "Task to delete",
            "Will be soft-deleted.",
            TaskPriority.Low,
            DateTime.UtcNow.AddDays(5),
            "user@company.com",
            "admin");

        // Act
        task.SoftDelete();

        // Assert
        task.IsDeleted.Should().BeTrue();
        task.UpdatedAt.Should().NotBeNull();
    }
}
