using FlowState;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
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
        //one db per test, shared acorss requests 
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

    // Registers a user and returns its token plus the id baked into the token's "sub" claim

    private async Task<(string Token, int UserId)> RegisterAndGetTokenAsync(
        string username = "karl", string email = "karl@email.com")
    {
        var registerBody = JsonSerializer.Serialize(new
        {
            username,
            email,
            password = "Testpassword1!",
            confirmPassword = "Testpassword1!"
        });

        var registerResponse = await _client.PostAsync("/api/auth/register",
            new StringContent(registerBody, Encoding.UTF8, "application/json"));
        var json = await registerResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(json).RootElement.GetProperty("token").GetString()!;
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var sub = jwt.Claims.First(c => c.Type == "sub").Value;

        return (token, int.Parse(sub));


    }

    private void Authenticate(string token) =>  _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


  
    [Test]
    public async Task GetAllUser_WithToken_Returns403_NoRoleSystemYet()
    {
        var (token, _) = await RegisterAndGetTokenAsync();
        Authenticate(token);

        var response = await _client.GetAsync("/api/users");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
    [Test]
    public async Task GetUserById_WithToken_Returns200_ForOwnProfile()
    {
        var (token, id) = await RegisterAndGetTokenAsync();
        Assert.That(id, Is.GreaterThan(0));
        Authenticate(token);

        var response = await _client.GetAsync($"/api/users/{id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetUserById_WithToken_Returns403_ForAnotherUser()
    {
        var (token, id) = await RegisterAndGetTokenAsync();
        Authenticate(token);

        var response = await _client.GetAsync($"/api/users/{id + 1}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task CreateUser_WithToken_Returns201()
    {
        var (token, _) = await RegisterAndGetTokenAsync();
        Authenticate(token);

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
    public async Task UpdateUser_WithToken_Returns200_ForOwnProfile()
    {
        var (token, id) = await RegisterAndGetTokenAsync();
        Authenticate(token);

        var body = JsonSerializer.Serialize(new { email = "updated@test.com" });
        var response = await _client.PutAsync($"/api/users/{id}",
            new StringContent(body, Encoding.UTF8, "application/json"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateUser_WithToken_Returns403_ForAnotherUser()
    {
        var (token, id) = await RegisterAndGetTokenAsync();
        Authenticate(token);

        var body = JsonSerializer.Serialize(new { email = "notyours@test.com" });
        var response = await _client.PutAsync($"/api/users/{id + 1}",
            new StringContent(body, Encoding.UTF8, "application/json"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task DeleteUser_WithToken_Returns204_ForOwnAccount()
    {
        var (token, id) = await RegisterAndGetTokenAsync();
        Authenticate(token);

        var response = await _client.DeleteAsync($"/api/users/{id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteUser_WithToken_Returns403_ForAnotherUser()
    {
        var (token, id) = await RegisterAndGetTokenAsync();
        Authenticate(token);

        var response = await _client.DeleteAsync($"/api/users/{id + 1}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task ChangeUsername_WithToken_Returns200_ForOwnAccount()
    {
        var (token, id) = await RegisterAndGetTokenAsync();
        Authenticate(token);

        var body = JsonSerializer.Serialize(new { newUsername = "aliceupdated" });
        var response = await _client.PatchAsync($"/api/users/{id}/username",
            new StringContent(body, Encoding.UTF8, "application/json"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task ChangeUsername_WithToken_Returns403_ForAnotherUser()
    {
        var (token, id) = await RegisterAndGetTokenAsync();
        Authenticate(token);

        var body = JsonSerializer.Serialize(new { newUsername = "notyours" });
        var response = await _client.PatchAsync($"/api/users/{id + 1}/username",
            new StringContent(body, Encoding.UTF8, "application/json"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }





}

