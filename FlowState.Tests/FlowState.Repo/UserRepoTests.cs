using FlowState.Models;
using FlowState.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowState.Tests;

public class UserRepoTests
{
    private MyDbContext _context;
    private UserRepo _repo;
    private List<User> _users;

    [SetUp]
    public void SetUp()
    {
        //new database generated for each test ensuring no crossover with Testdb
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new MyDbContext(options);
        _repo = new UserRepo(_context);

        //arrange
        _users = new List<User>
            {
                new User { Id = 1, Username = "alice", Email = "alice@example.com", PasswordHash = "hash1" },
                new User { Id = 2, Username = "bob", Email = "bob@example.com", PasswordHash = "hash2" },
                new User { Id = 3, Username = "charlie", Email = "charlie@example.com", PasswordHash = "hash3" },
                new User { Id = 4, Username = "dev123", Email = "dev123@example.com", PasswordHash = "hash4" },
                new User { Id = 5, Username = "JohnDoe", Email = "johndoe@example.com", PasswordHash = "hash5" },
                new User { Id = 6, Username = "x", Email = "x@example.com", PasswordHash = "hash6" }
            };
        _context.Users.AddRange(_users);
        _context.SaveChanges();

    }

    [TearDown]
    public void TearDown()
    {
        //After the test is done the context is disposed allowing us a fresh start
        _context.Dispose();
    }

    //GetAll
    [Test] 
    public void GetAllUsers_Returns_ListOfUsers()
    {
        //Act
        var result = _repo.GetAllUsers();
        //Assert
        Assert.That(result.Count, Is.EqualTo(6));

    }
    [Test]
    public void GetAllUsers_ReturnsCorrectUsernames()
    {
        var result = _repo.GetAllUsers();
        Assert.That(result.Select(u => u.Username), Is.EquivalentTo(new[]
        {
            "alice", "bob", "charlie", "dev123", "JohnDoe", "x"
        }));
    }
    //GetById
    [Test]
    public void GetUserById_ReturnsCorrectUser_WhenUserExsists()
    {
        var result = _repo.GetUserById(1);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("alice"));

    }
    [Test]
    public void GetUserById_ReturnsUser_WithShortname()
    {
        var result = _repo.GetUserById(6);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("x"));
    }
    [Test]
    public void GetUserById_ReturnsNull_WhenUserDoesnotExist()
    {
        var result = _repo.GetUserById(7);
        Assert.That(result, Is.Null);
        
    }
    //Add
    [Test]
    public void AddUser_ReturnsAddedUser_WhenUserIsValid()
    {
        var newUser = new User { Username = "newuser", Email = "new@example.com", PasswordHash = "hash" };

        var result = _repo.AddUser(newUser);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("newuser"));
        Assert.That(_context.Users.Count(), Is.EqualTo(7));
    }
    [Test]
    public void AddUser_ReturnsNull_WhenUserIsNull()
    {
        var result = _repo.AddUser(null!);
        Assert.That(result, Is.Null);
    }
    [Test]
    public void AddUser_PersistsUser_ToDatabase()
    {
        var newUser = new User { Username = "newuser", Email = "new@example.com", PasswordHash = "hash" };

        _repo.AddUser(newUser);

        var fromDb = _context.Users.FirstOrDefault(u => u.Username == "newuser");
        Assert.That(fromDb, Is.Not.Null);

    }
    //Delete
    [Test]
    public void DeleteUser_ReturnsTrue_WhenUserExists()
    {
        var result = _repo.DeleteUser(1);
        Assert.That(result, Is.True);
    }

    [Test]
    public void DeleteUser_RemovesUser_FromDatabase()
    {
        _repo.DeleteUser(1);
        Assert.That(_context.Users.Count(), Is.EqualTo(5));
    }

    [Test]
    public void DeleteUser_ReturnsFalse_WhenUserDoesNotExist()
    {
        var result = _repo.DeleteUser(999);
        Assert.That(result, Is.False);
    }

    [Test]
    public void DeleteUser_DoesNotReduceCount_WhenUserDoesNotExist()
    {
        _repo.DeleteUser(999);
        Assert.That(_context.Users.Count(), Is.EqualTo(6));
    }

    //update
    [Test]
    public void UpdateUser_ReturnsUpdatedUser_WhenUserExists()
    {
        var updated = new User {Email = "updated@example.com", PasswordHash = "newhash" };

        var result = _repo.UpdateUser(1, updated);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("alice"));
        Assert.That(result.Email, Is.EqualTo("updated@example.com"));
    }

    [Test]
    public void UpdateUser_PersistsChanges_ToDatabase()
    {
        //This will fail as we removed the username from UpdateUser for now 
        //Could be fixed later on
        var updated = new User { Username = "alice_updated", Email = "updated@example.com", PasswordHash = "newhash" };

        _repo.UpdateUser(1, updated);

        var fromDb = _context.Users.FirstOrDefault(u => u.Id == 1);
        Assert.That(fromDb!.Username, Is.EqualTo("alice_updated"));
    }

    [Test]
    public void UpdateUser_ReturnsNull_WhenUserDoesNotExist()
    {
        var updated = new User { Username = "ghost", Email = "ghost@example.com", PasswordHash = "hash" };
        var result = _repo.UpdateUser(999, updated);
        Assert.That(result, Is.Null);
    }

    //ChangeUserName
    [Test]
    public void ChangeUsername_ReturnsUpdatedUser_WhenUsernameIsAvailable()
    {
        var result = _repo.ChangeUsername(1, "alice_new");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("alice_new"));
    }

    [Test]
    public void ChangeUsername_PersistsNewUsername_ToDatabase()
    {
        _repo.ChangeUsername(1, "alice_new");

        var fromDb = _context.Users.FirstOrDefault(u => u.Id == 1);
        Assert.That(fromDb!.Username, Is.EqualTo("alice_new"));
    }

    [Test]
    public void ChangeUsername_ReturnsNull_WhenUserDoesNotExist()
    {
        var result = _repo.ChangeUsername(999, "newname");
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ChangeUsername_ReturnsNull_WhenUsernameIsAlreadyTaken()
    {
        var result = _repo.ChangeUsername(1, "bob");
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ChangeUsername_ReturnsNull_WhenUsernameIsEmpty()
    {
        var result = _repo.ChangeUsername(1, "");
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ChangeUsername_ReturnsNull_WhenUsernameIsWhitespace()
    {
        var result = _repo.ChangeUsername(1, "   ");
        Assert.That(result, Is.Null);
    }
    //failing test

    [Test]
    public void ChangeUsername_IsCaseSensitive_WhenCheckingForDuplicates()
    {
        //failing test
        // "JohnDoe" exists — "johndoe" should be allowed if case sensitive
        var result = _repo.ChangeUsername(1, "johndoe");
        Assert.That(result, Is.Not.Null);
    }
    [Test]
    public void ChangeUsername_ReturnsNull_WhenUsernameisTaken_IsCaseInSensitive()
    {
        // "JohnDoe" exists — "johndoe" should not  be allowed if case sensitive
        var result = _repo.ChangeUsername(1, "johndoe");
        Assert.That(result, Is.Null);
    }

    // GetUserByEmail
    [Test]
    public void GetUserByEmail_ReturnsCorrectUser_WhenEmailExists()
    {
        var result = _repo.GetUserByEmail("alice@example.com");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("alice"));
    }

    [Test]
    public void GetUserByEmail_ReturnsNull_WhenEmailDoesNotExist()
    {
        var result = _repo.GetUserByEmail("nobody@example.com");
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetUserByEmail_IsCaseInsensitive()
    {
        var result = _repo.GetUserByEmail("ALICE@EXAMPLE.COM");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("alice"));
    }

    // GetUserByUsername
    [Test]
    public void GetUserByUsername_ReturnsCorrectUser_WhenUsernameExists()
    {
        var result = _repo.GetUserByUsername("alice");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo("alice@example.com"));
    }

    [Test]
    public void GetUserByUsername_ReturnsNull_WhenUsernameDoesNotExist()
    {
        var result = _repo.GetUserByUsername("nobody");
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetUserByUsername_IsCaseInsensitive()
    {
        var result = _repo.GetUserByUsername("ALICE");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo("alice@example.com"));
    }






}
