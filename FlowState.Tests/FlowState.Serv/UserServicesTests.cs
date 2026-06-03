using FlowState.Models;
using FlowState.Repositories;
using FlowState.Services;
using Moq;

namespace FlowState.Tests;

public class UserServicesTests
{
    private Mock<IUserRepo> _mockRepo;
    private UserServices _service;
    private List<User> _users;


    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<IUserRepo>();
        _service = new UserServices(_mockRepo.Object);

        _users = new List<User>
        {
            new User { Id = 1, Username = "KarlMax", Email = "KarlMax@example.com", PasswordHash = "hashed1", CreatedAt = DateTime.UtcNow },
            new User { Id = 2, Username = "WillSmith", Email = "Smithsonian@example.com", PasswordHash = "hashed2", CreatedAt = DateTime.UtcNow }
        };

    }

    //GetAllUsers

    [Test]
    public void GetAllUsers_ReturnsAllUsers_WhenUsersExist()
    {
        _mockRepo.Setup(r => r.GetAllUsers()).Returns(_users);
        var result = _service.GetAllUser();
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(u => u.Username == "KarlMax"), Is.True);

    }
    [Test] 
    public void GetAllUsers_ReturnsEmptyList_WhenNoUserExist()
    {
        _mockRepo.Setup(r => r.GetAllUsers()).Returns(new List<User>());
        var result = _service.GetAllUser();
        Assert.That(result, Is.Empty);
    }
 

        //ById
    [Test] 
    public void GetUserById_ReturnsCorrectUser_WhenGiven_ValidId()
    {
        _mockRepo.Setup(r => r.GetUserById(1)).Returns(_users[0]);
        var result = _service.GetUserById(1);
        Assert.That(result!.Username, Is.EqualTo("KarlMax"));
    }
    [Test] 
    public void GetUserById_ReturnsNull_WhenUser_DoesNotExist()
    {
        _mockRepo.Setup(r => r.GetUserById(9)).Returns((User?)null);
        var result = _service.GetUserById(9);
        Assert.That(result, Is.Null);
    }

   
    //Delete
    [Test]
    public void DeleteUser_ReturnsTrue_WhenUserExists()
    {
        _mockRepo.Setup(r => r.DeleteUser(1)).Returns(true);

        var result = _service.DeleteUser(1);

        Assert.That(result, Is.True);
    }
    [Test]
    public void DeleteUser_ReturnsFalse_WhenUserDoesNotExist()
    {
        _mockRepo.Setup(r => r.DeleteUser(99)).Returns(false);

        var result = _service.DeleteUser(99);

        Assert.That(result, Is.False);
    }

    //Update
    [Test]
    public void UpdateUser_ReturnsUpdatedUser_WhenUserExists()
    {
        var updated = new User {Email = "Karlmax1@example.com", PasswordHash = "hash" };
        _mockRepo.Setup(r => r.UpdateUser(1, updated)).Returns(updated);

        var result = _service.UpdateUser(1, updated);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo("Karlmax1@example.com"));
    }
    [Test]
    public void UpdateUser_ReturnsNull_WhenUserDoesNotExist()
    {
        var updated = new User { Username = "Camus", Email = "Camus@example.com", PasswordHash = "hash" };
        _mockRepo.Setup(r => r.UpdateUser(99, updated)).Returns((User?)null);

        var result = _service.UpdateUser(99, updated);

        Assert.That(result, Is.Null);
    }
    //Change Username
    [Test]
    public void ChangeUsername_ReturnsUpdatedUser_WhenValid()
    {
        var updated = new User { Id = 1, Username = "KarlitoMax" };
        _mockRepo.Setup(r => r.GetAllUsers()).Returns(_users);
        _mockRepo.Setup(r => r.ChangeUsername(1, "KarlitoMax")).Returns(updated);

        var result = _service.ChangeUsername(1, "KarlitoMax");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("KarlitoMax"));
    }
    [Test]
    public void ChangeUsername_ReturnsNull_WhenNewUsernameIsEmpty()
    {
        var result = _service.ChangeUsername(1, "");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ChangeUsername_ReturnsNull_WhenNewUsernameIsWhitespace()
    {
        var result = _service.ChangeUsername(1, "   ");

        Assert.That(result, Is.Null);
    }
    [Test]
    public void ChangeUsername_ReturnsNull_WhenUsernameIsTaken()
    {
        _mockRepo.Setup(r => r.GetAllUsers()).Returns(_users);

        var result = _service.ChangeUsername(2, "KarlMax");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ChangeUsername_ReturnsNull_WhenUserDoesNotExist()
    {
        _mockRepo.Setup(r => r.ChangeUsername(99, "JhonSmith")).Returns((User?)null);
        _mockRepo.Setup(r => r.GetAllUsers()).Returns(_users);

        var result = _service.ChangeUsername(99, "JhonSmith");

        Assert.That(result, Is.Null);
    }



}
