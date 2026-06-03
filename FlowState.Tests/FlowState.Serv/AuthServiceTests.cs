using FlowState.Models;
using FlowState.Repositories;
using FlowState.Services;
using Moq;

namespace FlowState.Tests;

public class AuthServiceTests
{
    private Mock<IUserRepo> _mockRepo;
    private IAuthServices _authServices;
    private User _existingUser;

    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<IUserRepo>();

        _existingUser = new User
        {
            Id = 1,
            Username = "Karl",
            Email = "karl@max.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            CreatedAt = DateTime.UtcNow
        };

        _authServices = new AuthService(_mockRepo.Object);

    }

    [Test]
    public void Register_WithValidData_ReturnsCreatedUser()
    {
        _mockRepo.Setup(r => r.GetUserByEmail("lenin@max.com")).Returns((User?)null);
        _mockRepo.Setup(r => r.GetUserByUsername("bob")).Returns((User?)null);
        _mockRepo.Setup(r => r.AddUser(It.IsAny<User>())).Returns<User>(u => u);

        var result = _authServices.Register("lenin", "lenin@max.com", "qwerty1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("lenin"));
        Assert.That(result.Email, Is.EqualTo("lenin@max.com"));
        Assert.That(result.PasswordHash, Is.Not.EqualTo("qwerty1"));

    }
    [Test]
    public void Register_WithDuplicateEmail_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetUserByEmail("karl@max.com")).Returns(_existingUser);

        var result = _authServices.Register("newuser", "karl@max.com", "qwerty1");

        Assert.That(result, Is.Null);
    }
    [Test]
    public void Register_WithDuplicateUsername_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetUserByEmail("new@example.com")).Returns((User?)null);
        _mockRepo.Setup(r => r.GetUserByUsername("Karl")).Returns(_existingUser);

        var result = _authServices.Register("Karl", "new@example.com", "qwerty2");

        Assert.That(result, Is.Null);
    }
    [Test]
    public void Register_HashesPassword_BeforePersisting()
    {
        User? captured = null;
        _mockRepo.Setup(r => r.GetUserByEmail(It.IsAny<string>())).Returns((User?)null);
        _mockRepo.Setup(r => r.GetUserByUsername(It.IsAny<string>())).Returns((User?)null);
        _mockRepo.Setup(r => r.AddUser(It.IsAny<User>()))
                 .Callback<User>(u => captured = u)
                 .Returns((User u) => u);

        _authServices.Register("VladmirLenin", "vlad@impaler.com", "Sup3rSecret!");

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.PasswordHash, Is.Not.EqualTo("Sup3rSecret!"));
        Assert.That(BCrypt.Net.BCrypt.Verify("Sup3rSecret!", captured.PasswordHash), Is.True);
    }

    //Login

    [Test]
    public void Login_WithValidCredential_ReturnsUser()
    {
        _mockRepo.Setup(r => r.GetUserByEmail("karl@max.com")).Returns(_existingUser);
        var result = _authServices.Login("karl@max.com", "correct-password");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo("karl@max.com"));
    }

    [Test]
    public void Login_WithUnkownEmail_ReturnsNuLL()
    {
        _mockRepo.Setup(r => r.GetUserByEmail("notkar@max.com")).Returns((User?)null);
        var result = _authServices.Login("notkarl@max.com", "notpassword");
        Assert.That(result, Is.Null);

    }
    [Test]
    public void Login_WithWrongPassword_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetUserByEmail("karl@max.com")).Returns(_existingUser);
        var result = _authServices.Login("karl@max.com", "notapassword");
        Assert.That(result, Is.Null);
    }

}
