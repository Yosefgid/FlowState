using FlowState.Controllers;
using FlowState.Models;
using FlowState.Models.DTOs;
using FlowState.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

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

    private void SetLoggedInUser(int userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    //Get
    [Test]
    public void GetAllUsers_ReturnsForbid_UntilRoleSystemExists()
    {
        SetLoggedInUser(1);
        var result = _controller.GetAllUsers();
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    //GetById
    [Test]
    public void GetUserById_ReturnsOk_WhenAccessingOwnData()
    {
        SetLoggedInUser(1);
        _mockUserServices.Setup(s => s.GetUserById(1)).Returns(_userOne);

        var result = _controller.GetUserById(1);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        Assert.That((ok.Value as User)!.Username, Is.EqualTo("karl"));
    }

    [Test]
    public void GetUserById_ReturnsForbid_WhenAccessingAnotherUser()
    {
        SetLoggedInUser(1);
        var result = _controller.GetUserById(2);
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public void GetUserById_ReturnsNotFound_WhenOwnRecordMissing()
    {
        SetLoggedInUser(6);
        _mockUserServices.Setup(s => s.GetUserById(6)).Returns((User?)null);

        var result = _controller.GetUserById(6);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void GetUserById_ReturnsUnauthorized_WhenTokenHasNoUserId()
    {
        var identity = new ClaimsIdentity(new[] { new Claim("foo", "bar") }, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };

        var result = _controller.GetUserById(1);

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    //Add user (no ownership check — slated for removal next ticket)
    [Test]
    public void CreateUser_ReturnsCreated_WhenDtoIsValid()
    {
        var dto = new CreateUserDto { Username = "alice", Email = "notbob@Max.com", Password = "Qwerty1" };
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
        var dto = new CreateUserDto { Username = "dupliKate", Email = "dup@invincible.com", Password = "qwerty" };
        _mockUserServices.Setup(s => s.AddUser(It.IsAny<User>(), dto.Password)).Returns((User?)null);

        var result = _controller.CreateUser(dto);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }



    //Update
    [Test]
    public void UpdateUser_ReturnsOk_WhenUpdatingOwnData()
    {
        SetLoggedInUser(1);
        var dto = new UpdateUserDto { Email = "karl1@Max.com" };
        var updated = new User { Id = 1, Username = "Karl_updated", Email = "karl1@Max.com" };

        _mockUserServices.Setup(s => s.GetUserById(1)).Returns(_userOne);
        _mockUserServices.Setup(s => s.UpdateUser(1, It.IsAny<User>())).Returns(updated);

        var result = _controller.UpdateUser(1, dto);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        Assert.That((ok.Value as User)!.Email, Is.EqualTo("karl1@Max.com"));
    }

    [Test]
    public void UpdateUser_ReturnsForbid_WhenUpdatingAnotherUser()
    {
        SetLoggedInUser(1);
        var result = _controller.UpdateUser(2, new UpdateUserDto { Email = "new@x.com" });
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public void UpdateUser_ReturnsNotFound_WhenOwnRecordMissing()
    {
        SetLoggedInUser(99);
        _mockUserServices.Setup(s => s.GetUserById(99)).Returns((User?)null);

        var result = _controller.UpdateUser(99, new UpdateUserDto { Email = "Camus@example.com" });

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void UpdateUser_ReturnsBadRequest_WhenEmailIsEmpty()
    {
        SetLoggedInUser(1);
        var result = _controller.UpdateUser(1, new UpdateUserDto { Email = "" });
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    //Delete
    [Test]
    public void DeleteUser_ReturnsNoContent_WhenDeletingOwnAccount()
    {
        SetLoggedInUser(1);
        _mockUserServices.Setup(s => s.DeleteUser(1)).Returns(true);

        var result = _controller.DeleteUser(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public void DeleteUser_ReturnsForbid_WhenDeletingAnotherUser()
    {
        SetLoggedInUser(1);
        var result = _controller.DeleteUser(2);
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public void DeleteUser_ReturnsNotFound_WhenOwnRecordMissing()
    {
        SetLoggedInUser(99);
        _mockUserServices.Setup(s => s.DeleteUser(99)).Returns(false);

        var result = _controller.DeleteUser(99);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    //Change username
    [Test]
    public void ChangeUsername_ReturnsOk_WhenChangingOwnUsername()
    {
        SetLoggedInUser(1);
        var dto = new ChangeUsernameDto { NewUsername = "karl_updated" };
        var updated = new User { Id = 1, Username = "karl_updated", Email = "karl@max.com" };
        _mockUserServices.Setup(s => s.ChangeUsername(1, dto.NewUsername)).Returns(updated);

        var result = _controller.ChangeUsername(1, dto);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        Assert.That((ok.Value as User)!.Username, Is.EqualTo("karl_updated"));
    }

    [Test]
    public void ChangeUsername_ReturnsForbid_WhenChangingAnotherUser()
    {
        SetLoggedInUser(1);
        var result = _controller.ChangeUsername(2, new ChangeUsernameDto { NewUsername = "newname" });
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public void ChangeUsername_ReturnsNotFound_WhenOwnRecordMissing()
    {
        SetLoggedInUser(99);
        var dto = new ChangeUsernameDto { NewUsername = "ghost" };
        _mockUserServices.Setup(s => s.ChangeUsername(99, dto.NewUsername)).Returns((User?)null);

        var result = _controller.ChangeUsername(99, dto);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void ChangeUsername_ReturnsBadRequest_WhenNewUsernameIsEmpty()
    {
        SetLoggedInUser(1);
        var result = _controller.ChangeUsername(1, new ChangeUsernameDto { NewUsername = "" });
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }


}
