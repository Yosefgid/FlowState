using FlowState;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
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
        var dbName = Guid.NewGuid().ToString();
        _factory = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MyDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<MyDbContext>(options =>
                    options.UseInMemoryDatabase(dbName));
            });
        });
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

    //Authenticated tests with helper function 
    private async Task<string> GetTokenAsync()
    {
        var registerBody = JsonSerializer.Serialize(new
        {
            username = "alice",
            email = "alice@test.com",
            password = "Testpassword1!",
            confirmPassword = "Testpassword1!"
        });

        var registerResponse = await _client.PostAsync("/api/auth/register",
            new StringContent(registerBody, Encoding.UTF8, "application/json"));

        var json = await registerResponse.Content.ReadAsStringAsync();
        var jsonDocObj = JsonDocument.Parse(json);
        return jsonDocObj.RootElement.GetProperty("token").GetString()!;
    }


    [Test]
    public async Task GetAllUser_WithToken_Returns200()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/users");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));


    }
    [Test]
    public async Task GetUserById_WithToken_Returns404_WhenUserDoesNotExist()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/users/900");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateUser_WithToken_Returns201()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = JsonSerializer.Serialize(new
        {
            username = "mansatester",
            email = "mansatest@test.com",
            password = "Password123!"
        });
        var response = await _client.PostAsync("/api/users",
            new StringContent(body, Encoding.UTF8, "application/json"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
    [Test]
    public async Task UpdateUser_WithToken_Returns404_WhenUserDoesNotExist()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = JsonSerializer.Serialize(new { email = "notauser@test.com" });
        var response = await _client.PutAsync("/api/users/900",
            new StringContent(body, Encoding.UTF8, "application/json"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task DeleteUser_WithToken_Returns404_WhenUserDoesNotExist()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.DeleteAsync("/api/users/99999");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task ChangeUsername_WithToken_Returns404_WhenUserDoesNotExist()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = JsonSerializer.Serialize(new { newUsername = "newname" });
        var response = await _client.PatchAsync("/api/users/99999/username",
            new StringContent(body, Encoding.UTF8, "application/json"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }



}

