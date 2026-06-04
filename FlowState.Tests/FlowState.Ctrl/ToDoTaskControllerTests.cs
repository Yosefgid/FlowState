using FlowState.Controllers;
using FlowState.Models;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlowState.Tests;

public class ToDoTaskControllerTests
{
    private Mock<IToDoTaskService> _moqService;
    private ToDoTaskController taskController ;
    private List<ToDoTask> exampleTasks;

    [SetUp]
    public void Setup()
    {
        _moqService = new Mock<IToDoTaskService>();
        taskController = new ToDoTaskController(_moqService.Object);

        exampleTasks = new List<ToDoTask>
        {
            new ToDoTask("Finish API", "Complete CRUD endpoints for ToDo API", "google-1"),
            new ToDoTask("Study EF Core", "Review tracking, migrations, and relationships", "google-2"),
            new ToDoTask("Fix Toggle Bug", "Debug why IsCompleted is not persisting", "google-3"),
            new ToDoTask("Frontend UI", "Build Blazor or React task UI", "google-4"),
            new ToDoTask("Write Tests", "Add unit tests for service layer", "google-5")
        };

        for (int i = 0; i < exampleTasks.Count; i++)
        {
            exampleTasks[i].Id = i + 1;
        }
    }

    [TearDown]
    public void TearDown()
    {
        //taskController.Dispose();
    }

    [Test]
    public void GetTasks_ReturnsOkResult_WithListOfTasks()
    {
        // Arrange
        _moqService.Setup(service => service.GetAllTasks())
                   .Returns(exampleTasks);

        // Act
        var result = taskController.GetAllTasks();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        var okResult = result as OkObjectResult;

        Assert.That(okResult!.Value, Is.EqualTo(exampleTasks));
    }

    [Test]
    public void GetTask_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var task = exampleTasks[0];

        _moqService.Setup(service => service.GetTask(1))
                   .Returns(task);

        // Act
        var result = taskController.GetTask(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        var okResult = result as OkObjectResult;
        var returnedTask = okResult!.Value as ToDoTask;

        Assert.That(returnedTask, Is.Not.Null);
        Assert.That(returnedTask!.Id, Is.EqualTo(1));
    }

    [Test]
    public void GetTask_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _moqService.Setup(service => service.GetTask(99))
                   .Returns((ToDoTask?)null);

        // Act
        var result = taskController.GetTask(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void AddTask_ReturnsCreatedAtAction()
    {
        // Arrange
        var newTask = new ToDoTask( "New Task", "Testing POST", "google-6");
        newTask.Id = 6;

        _moqService.Setup(service => service.AddTask(newTask))
                   .Returns(newTask);

        // Act
        var result = taskController.AddTask(newTask);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());

        var createdResult = result as CreatedAtActionResult;
        var returnedTask = createdResult!.Value as ToDoTask;

        Assert.That(returnedTask, Is.Not.Null);
        Assert.That(returnedTask!.Id, Is.EqualTo(6));
    }

    [Test]
    public void DeleteTask_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _moqService.Setup(service => service.DeleteTask(1))
                   .Returns(true);

        // Act
        var result = taskController.DeleteTask(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public void DeleteTask_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _moqService.Setup(service => service.DeleteTask(99))
                   .Returns(false);

        // Act
        var result = taskController.DeleteTask(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void ToggleTaskCompleted_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var task = exampleTasks[0];

        task.IsCompleted = true;

        _moqService.Setup(service => service.GetTask(1))
                   .Returns(task);

        _moqService.Setup(service => service.ToggleTaskCompleted(1))
                   .Returns(task);

        // Act
        var result = taskController.ToggleTaskCompleted(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());

        var okResult = result as OkObjectResult;
        var returnedTask = okResult!.Value as ToDoTask;

        Assert.That(returnedTask, Is.Not.Null);
        Assert.That(returnedTask!.IsCompleted, Is.True);
    }

    [Test]
    public void ToggleTaskCompleted_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _moqService.Setup(service => service.GetTask(99))
                   .Returns((ToDoTask?)null);

        // Act
        var result = taskController.ToggleTaskCompleted(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
