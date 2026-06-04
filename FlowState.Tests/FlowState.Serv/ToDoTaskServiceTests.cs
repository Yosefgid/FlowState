using FlowState.Models;
using FlowState.Repositories;
using FlowState.Services;
using Moq;
namespace FlowState.Tests;

public class ToDoTaskServiceTests
{
    private Mock<IToDoTaskRepo> _moqRepo;
    private ToDoTaskService taskService;
    private List<ToDoTask> exampleTasks;

    [SetUp]
    public void Setup()
    {
        _moqRepo = new Mock<IToDoTaskRepo>();
        taskService = new ToDoTaskService(_moqRepo.Object);

        exampleTasks = new List<ToDoTask>
        {
            new ToDoTask(0,"Finish API", "Complete CRUD endpoints for ToDo API", "google-1"),
            new ToDoTask(0,"Study EF Core", "Review tracking, migrations, and relationships", "google-2"),
            new ToDoTask(0,"Fix Toggle Bug", "Debug why IsCompleted is not persisting", "google-3"),
            new ToDoTask(0,"Frontend UI", "Build Blazor or React task UI", "google-4"),
            new ToDoTask(0,"Write Tests", "Add unit tests for service layer", "google-5")
        };

        for(int i = 0; i < exampleTasks.Count; i++)
        {
            exampleTasks[i].Id = i+1; 
        }
    }

    [Test]
    public void GetAllTasks_ReturnsAllTasks()
    {
        // Arrange
        _moqRepo.Setup(repo => repo.GetAllTasks())
                .Returns(exampleTasks);

        // Act
        var result = taskService.GetAllTasks();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result[0].Name, Is.EqualTo("Finish API"));
    }

    [Test]
    public void GetTask_WithValidId_ReturnsTask()
    {
        // Arrange
        var task = exampleTasks[0];

        _moqRepo.Setup(repo => repo.GetTask(1))
                .Returns(task);

        // Act
        var result = taskService.GetTask(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Finish API"));
    }

    [Test]
    public void AddTask_ReturnsCreatedTask()
    {
        // Arrange
        var newTask = new ToDoTask(0,"New Task", "Testing add method", "google-6");

        _moqRepo.Setup(repo => repo.AddTask(newTask))
                .Returns(newTask);

        // Act
        var result = taskService.AddTask(newTask);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("New Task"));
    }

    [Test]
    public void DeleteTask_WithValidId_ReturnsTrue()
    {
        // Arrange
        _moqRepo.Setup(repo => repo.DeleteTask(1))
                .Returns(true);

        // Act
        var result = taskService.DeleteTask(1);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void ToggleTaskCompleted_TogglesCompletionStatus()
    {
        // Arrange
        var task = exampleTasks[0];

        task.IsCompleted = false;

        task.IsCompleted = !task.IsCompleted;

        _moqRepo.Setup(repo => repo.ToggleTaskCompleted(1))
                .Returns(task);

        // Act
        var result = taskService.ToggleTaskCompleted(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.IsCompleted, Is.True);
    }
}
