using FlowState.Models;
using FlowState.Repositories;
using FlowState.Services;
using Moq;

namespace FlowState.Tests;

public class GoogleServiceTest
{
    private Mock<IGoogleToDoTaskRepo> _mockRepo;
    private GoogleService _googleService;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IGoogleToDoTaskRepo>();
        _googleService = new GoogleService(_mockRepo.Object);
    }

    [Test]
    public void GetGoogleAuthUrl_ReturnsGoogleOAuthUrl()
    {
        var result = _googleService.GetGoogleAuthUrl();

        Assert.That(result, Is.EqualTo("https://accounts.google.com/o/oauth2/v2/auth"));
    }

    [Test]
    public async Task SaveImportedGoogleEventsAsync_ReturnsImportedTasks()
    {
        var importedTasks = new List<ToDoTask>
        {
            new ToDoTask { Id = 1, Name = "Google Event 1" },
            new ToDoTask { Id = 2, Name = "Google Event 2" }
        };

        var result = await _googleService.SaveImportedGoogleEventsAsync(importedTasks);

        Assert.That(result, Is.EqualTo(importedTasks));
    }

    [Test]
    public async Task SaveImportedGoogleEventsAsync_CallsRepoAddAsync_ForEachTask()
    {
        var importedTasks = new List<ToDoTask>
        {
            new ToDoTask { Id = 1, Name = "Google Event 1" },
            new ToDoTask { Id = 2, Name = "Google Event 2" }
        };

        await _googleService.SaveImportedGoogleEventsAsync(importedTasks);

        _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<ToDoTask>()), Times.Exactly(2));
    }

    [Test]
    public async Task SaveImportedGoogleEventsAsync_SavesTaskWithCorrectName()
    {
        var importedTasks = new List<ToDoTask>
        {
            new ToDoTask { Id = 1, Name = "Dentist Appointment" }
        };

        await _googleService.SaveImportedGoogleEventsAsync(importedTasks);

        _mockRepo.Verify(repo => repo.AddAsync(
            It.Is<ToDoTask>(task => task.Name == "Dentist Appointment")
        ), Times.Once);
    }

    [Test]
    public async Task SaveImportedGoogleEventsAsync_SavesTaskWithCorrectId()
    {
        var importedTasks = new List<ToDoTask>
        {
            new ToDoTask { Id = 99, Name = "Team Meeting" }
        };

        await _googleService.SaveImportedGoogleEventsAsync(importedTasks);

        _mockRepo.Verify(repo => repo.AddAsync(
            It.Is<ToDoTask>(task => task.Id == 99)
        ), Times.Once);
    }

    [Test]
    public async Task SaveImportedGoogleEventsAsync_WithEmptyList_DoesNotCallRepo()
    {
        var importedTasks = new List<ToDoTask>();

        var result = await _googleService.SaveImportedGoogleEventsAsync(importedTasks);

        _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<ToDoTask>()), Times.Never);

        Assert.That(result, Is.Empty);
    }
}