using Castle.Core.Configuration;
using FlowState.Controllers;
using FlowState.Models;
using FlowState.Models.DTOs.Auth;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FlowState.Tests;

public class AuthControllerTests
{
    private Mock<IAuthServices> _mockAuthServices;
    private IConfiguration _configuration;
    private AuthController _controller;

    private User _validUser;
      
    [SetUp]
    public void Setup()
    {

        _mockAuthServices = new Mock<IAuthServices>();
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key",     "super-secret-test-key-that-is-long-enough-32chars" },
            { "Jwt:Issuer",  "flowstate-test" },
            { "Jwt:Audience","flowstate-test" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _validUser = new User
        {
            Id = 1,
            Username = "karl",
            Email = "karl@max.com",
            CreatedAt = DateTime.UtcNow
        };

        _controller = new AuthController(_mockAuthServices.Object, _configuration);

    }

    [Test]
    public void Register_WithValidDto_Returns201WithToken() 
    {
        var dto = new RegisterDto
        {
            Username = "karl",
            Email = "karl@max.com",
            Password = "Qwerty123!",
            ConfirmPassword = "Qwerty123!"
        };

        _mockAuthServices.Setup(s => s.Register(dto.Username, dto.Email, dto.Password))
            .Returns(_validUser);

        var result = _controller.Register(dto);
        Assert.That(result, Is.InstanceOf<CreatedResult>());
        var created = (CreatedResult)result;
        Assert.That(created.Value, Is.InstanceOf<AuthResponseDto>());
        var response = (AuthResponseDto)created.Value!;
        Assert.That(response.Token, Is.Not.Empty);
        Assert.That(response.Username, Is.EqualTo("karl"));


    }
    [Test]
    public void Register_WithDuplicateUser_Returns400()
    {
        var dto = new RegisterDto
        {
            Username = "karl",
            Email = "karl@max.com",
            Password = "Qwerty123!",
            ConfirmPassword = "Qwerty123"
        };

        _mockAuthServices.Setup(s => s.Register(dto.Username, dto.Email, dto.Password))
            .Returns((User?)null);

        var result = _controller.Register(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public void Register_WithWeakPassword_Returns400()
    {
        var dto = new RegisterDto
        {
            Username = "karl",
            Email = "karl@max.com",
            Password = "weak",
            ConfirmPassword = "weak"
        };

        var result = _controller.Register(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public void Register_WithInvalidEmail_Returns400()
    {
        var dto = new RegisterDto
        {
            Username = "karl",
            Email = "notanemail",
            Password = "Qwerty123!",
            ConfirmPassword = "Qwerty123!"
        };

        var result = _controller.Register(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public void Register_WithEmptyUsername_Returns400()
    {
        var dto = new RegisterDto
        {
            Username = "",
            Email = "karl@max.com",
            Password = "Qwerty123!",
            ConfirmPassword = "Qwerty123!"
        };

        var result = _controller.Register(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }


    [Test]
    public void Login_WithValidCredentials_Returns200WithToken()
    {
        var dto = new LoginDto
        {
            Email = "karl@max.com",
            Password = "Qwerty123"
        };

        _mockAuthServices
            .Setup(s => s.Login(dto.Email, dto.Password))
            .Returns(_validUser);

        var result = _controller.Login(dto);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var ok = (OkObjectResult)result;
        var response = (AuthResponseDto)ok.Value!;
        Assert.That(response.Token, Is.Not.Empty);
    }
    [Test]
    public void Login_WithInvalidCredentials_Returns401()
    {
        var dto = new LoginDto
        {
            Email = "karl@max.com",
            Password = "notapassword"
        };

        _mockAuthServices.Setup(s => s.Login(dto.Email, dto.Password))
            .Returns((User?)null);

        var result = _controller.Login(dto);

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }


}
