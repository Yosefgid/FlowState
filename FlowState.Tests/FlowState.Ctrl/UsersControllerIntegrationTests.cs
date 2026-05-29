using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;

namespace FlowState.Tests;

public class UsersControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();

    }
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetAllUser_NoToken_Returns401()
    {
        var response = await _client.GetAsync("/api/users");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

    }
    [Test]
    public async Task GetUserById_NoToken_Returns401()
    {
        var response = await _client.GetAsync("/api/users/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task CreateUser_NoToken_Returns401()
    {
        var body = JsonSerializer.Serialize(new { username = "test", email = "test@test.com", password = "Password1!" });
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/users", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task UpdateUser_NoToken_Returns401()
    {
        var body = JsonSerializer.Serialize(new { email = "new@test.com" });
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("/api/users/1", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task DeleteUser_NoToken_Returns401()
    {
        var response = await _client.DeleteAsync("/api/users/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task ChangeUsername_NoToken_Returns401()
    {
        var body = JsonSerializer.Serialize(new { newUsername = "newname" });
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync("/api/users/1/username", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }


}
