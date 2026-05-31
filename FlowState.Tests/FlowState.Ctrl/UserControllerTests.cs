using FlowState.Controllers;
using FlowState.Models;
using FlowState.Models.DTOs;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlowState.Tests;

public class UserControllerTests
{
    private Mock<IUserServices> _mockUserServices;
    private UsersController _controller;
    private User _userOne;
    private User _userTwo;
    [SetUp]
    public void Setup()
    {
        _mockUserServices = new Mock<IUserServices>();
        _controller = new UsersController(_mockUserServices.Object);

        _userOne = new User
        {
            Id = 1,
            Username = "karl",
            Email = "karl@max.com",
            PasswordHash = "superstrongpassword",
            CreatedAt = DateTime.UtcNow
        };
        _userTwo = new User
        {
            Id = 2,
            Username = "Lenin",
            Email = "Lenin@max.com",
            PasswordHash = "superstrongpassword1",
            CreatedAt = DateTime.UtcNow
        };

    }

    //Get
    [Test]
    public void GetAllUsers_ReturnOk_WithListtOfUser()
    {
        _mockUserServices.Setup(s => s.GetAllUser()).Returns(new List<User> { _userOne, _userTwo });
        var result = _controller.GetAllUsers();
        var ok = result as OkObjectResult;

        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        Assert.That(ok.Value as List<User>, Has.Count.EqualTo(2));

    }

    [Test]
    public void GetAllUsers_Returns500_WhenServiceThrows()
    {
        _mockUserServices
            .Setup(s => s.GetAllUser())
            .Throws(new Exception("DB failure"));

        var result = _controller.GetAllUsers();

        var status = result as ObjectResult;
        Assert.That(status, Is.Not.Null);
        Assert.That(status!.StatusCode, Is.EqualTo(500));
    }

    //GetById
    [Test]
    public void GetUserById_ReturnsOk_WhenUserExists()
    {
        _mockUserServices
            .Setup(s => s.GetUserById(1))
            .Returns(_userOne);

        var result = _controller.GetUserById(1) ;

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        Assert.That((ok.Value as User)!.Username, Is.EqualTo("karl"));
    }

    [Test]
    public void GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _mockUserServices
            .Setup(s => s.GetUserById(6))
            .Returns((User?)null);

        var result = _controller.GetUserById(6);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    //Add user
    [Test]
    public void CreateUser_ReturnsCreated_WhenDtoIsValid()
    {
        var dto = new CreateUserDto
        {
            Username = "alice",
            Email = "notbob@Max.com",
            Password = "Qwerty1"
        };
        var created = new User { Id = 3, Username = "alice", Email = "notbob@Max.com" };
        _mockUserServices.Setup(s => s.AddUser(It.IsAny<User>(), dto.Password)).Returns(created);

        var result = _controller.CreateUser(dto);
        var createdResult = result as CreatedAtActionResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
        Assert.That((createdResult.Value as User)!.Username, Is.EqualTo("alice"));

    }

    [Test]
    public void CreateUser_ReturnsBadRequest_WhenUsernameIsEmpty()
    {
        var dto = new CreateUserDto { Username = "", Email = "roam@roam.com", Password = "pass" };

        var result = _controller.CreateUser(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public void CreateUser_ReturnsBadRequest_WhenEmailIsEmpty()
    {
        var dto = new CreateUserDto { Username = "Charles", Email = "", Password = "pass" };

        var result = _controller.CreateUser(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public void CreateUser_ReturnsBadRequest_WhenPaasswordIsEmpty()
    {
        var dto = new CreateUserDto { Username = "Charles", Email = "lol@lol.com", Password = "" };

        var result = _controller.CreateUser(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public void CreateUser_ReturnsBadRequest_WhenServiceReturnsNull()
    {
        var dto = new CreateUserDto
        {
            Username = "dupliKate",
            Email = "dup@invincible.com",
            Password = "qwerty"
        };

        _mockUserServices
            .Setup(s => s.AddUser(It.IsAny<User>(), dto.Password))
            .Returns((User?)null);

        var result = _controller.CreateUser(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

    }

    //update

    [Test]
    public void UpdateUser_ReturnsOk_WhenUpdateSucceeds()
    {
        var dto = new UpdateUserDto {Email = "karl1@Max.com" };

        var updated = new User { Id = 1, Username = "Karl_updated", Email = "karl1@Max.com" };

        _mockUserServices
            .Setup(s => s.GetUserById(1))
            .Returns(_userOne);

        _mockUserServices
            .Setup(s => s.UpdateUser(1, It.IsAny<User>()))
            .Returns(updated);

        var result = _controller.UpdateUser(1, dto);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        Assert.That((ok.Value as User)!.Email, Is.EqualTo("karl1@Max.com"));
    }

    [Test]
    public void UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var dto = new UpdateUserDto {Email = "Camus@example.com"};

        _mockUserServices
            .Setup(s => s.GetUserById(99))
            .Returns((User?)null);

        var result = _controller.UpdateUser(99, dto);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
    [Test]
    public void UpdateUser_ReturnsBadRequest_WhenEmailIsEmpty()
    {
        var dto = new UpdateUserDto { Email = "" };

        var result = _controller.UpdateUser(1, dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    //Delete
    [Test]
    public void DeleteUser_ReturnsNoContent_WhenUserExists()
    {
        _mockUserServices
            .Setup(s => s.DeleteUser(1))
            .Returns(true);

        var result = _controller.DeleteUser(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public void DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _mockUserServices
            .Setup(s => s.DeleteUser(99))
            .Returns(false);

        var result = _controller.DeleteUser(99);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
    //Change username
    [Test]
    public void ChangeUsername_ReturnsOk_WhenUpdateSucceeds()
    {
        var dto = new ChangeUsernameDto { NewUsername = "karl_updated" };
        var updated = new User { Id = 1, Username = "karl_updated", Email = "karl@max.com" };

        _mockUserServices
            .Setup(s => s.ChangeUsername(1, dto.NewUsername))
            .Returns(updated);

        var result = _controller.ChangeUsername(1, dto);
        var ok = result as OkObjectResult;

        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        Assert.That((ok.Value as User)!.Username, Is.EqualTo("karl_updated"));
    }

    [Test]
    public void ChangeUsername_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var dto = new ChangeUsernameDto { NewUsername = "ghost" };

        _mockUserServices
            .Setup(s => s.ChangeUsername(99, dto.NewUsername))
            .Returns((User?)null);

        var result = _controller.ChangeUsername(99, dto);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
    [Test]
    public void ChangeUsername_ReturnsBadRequest_WhenNewUsernameIsEmpty()
    {
        var dto = new ChangeUsernameDto { NewUsername = "" };

        var result = _controller.ChangeUsername(1, dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }



}
