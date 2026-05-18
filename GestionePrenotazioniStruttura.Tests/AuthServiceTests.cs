using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Models.Enum;
using GestionePrenotazioniStruttura.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

public class AuthServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private IConfiguration GetConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
          {"Jwt:Key", "SUPER_SECRET_TEST_KEY_123456789_1234567890123456789012345678901234567890"},
           {"Jwt:Issuer", "TestIssuer"},
           {"Jwt:Audience", "TestAudience"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }

    private User CreateConfirmedUser(string password = "Password123!")
    {
        return new User
        {
            Name = "Mario",
            Email = "mario@test.com",
            Role = Role.Customer,
            CreatedAt = new DateTime(2026, 1, 1),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Valid_Token_And_Save_RefreshToken()
    {
        var context = GetDbContext();
        var config = GetConfiguration();
        var service = new AuthService(context, config);

        var user = CreateConfirmedUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var loginDto = new UserLoginDto
        {
            Email = user.Email,
            Password = "Password123!"
        };

        var response = await service.LoginAsync(loginDto);

        Assert.NotNull(response.Token);
        Assert.Equal(Role.Customer.ToString(), response.Role);

        var savedUser = await context.Users.FirstAsync();
        Assert.NotNull(savedUser.RefreshToken);
        Assert.NotNull(savedUser.RefreshTokenExpiryTime);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(response.Token);
        foreach (var c in jwt.Claims)
        {
            Console.WriteLine($"TYPE: {c.Type}  VALUE: {c.Value}");
        }

        Assert.Equal(user.Id.ToString(),
    jwt.Claims.First(c => c.Type == "nameid").Value);

        Assert.Equal(user.Email,
            jwt.Claims.First(c => c.Type == "email").Value);

        Assert.Equal(user.Role.ToString(),
            jwt.Claims.First(c => c.Type == "role").Value);

    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_Password_Is_Wrong()
    {
        var context = GetDbContext();
        var config = GetConfiguration();
        var service = new AuthService(context, config);

        var user = CreateConfirmedUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var loginDto = new UserLoginDto
        {
            Email = user.Email,
            Password = "WrongPassword"
        };

        await Assert.ThrowsAsync<Exception>(() => service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_User_Not_Confirmed()
    {
        var context = GetDbContext();
        var config = GetConfiguration();
        var service = new AuthService(context, config);

        var user = CreateConfirmedUser();
        user.CreatedAt = new DateTime(2024, 1, 1);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var loginDto = new UserLoginDto
        {
            Email = user.Email,
            Password = "Password123!"
        };

        await Assert.ThrowsAsync<Exception>(() => service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User_With_Hashed_Password()
    {
        var context = GetDbContext();
        var config = GetConfiguration();
        var service = new AuthService(context, config);

        var dto = new UserRegisterDto
        {
            Name = "Luigi",
            Email = "luigi@test.com",
            Password = "Password123!"
        };

        var user = await service.RegisterAsync(dto);

        Assert.NotEqual(dto.Password, user.PasswordHash);
        Assert.Equal(Role.Customer, user.Role);
    }

    [Fact]
    public async Task LogoutByUserIdAsync_Should_Clear_RefreshToken()
    {
        var context = GetDbContext();
        var config = GetConfiguration();
        var service = new AuthService(context, config);

        var user = CreateConfirmedUser();
        user.RefreshToken = "test";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        await service.LogoutByUserIdAsync(user.Id);

        var updatedUser = await context.Users.FindAsync(user.Id);
        Assert.Null(updatedUser!.RefreshToken);
        Assert.Null(updatedUser.RefreshTokenExpiryTime);
    }

    [Fact]
    public async Task ConfermaUtenti_Should_Update_CreatedAt_When_Yes()
    {
        var context = GetDbContext();
        var config = GetConfiguration();
        var service = new AuthService(context, config);

        var user = CreateConfirmedUser();
        user.CreatedAt = DateTime.MinValue;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        await service.ConfermaUtenti(user.Id, "YES");

        var updated = await context.Users.FindAsync(user.Id);
        Assert.True(updated!.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task ConfermaUtenti_Should_Remove_User_When_Not_Yes()
    {
        var context = GetDbContext();
        var config = GetConfiguration();
        var service = new AuthService(context, config);

        var user = CreateConfirmedUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() =>
            service.ConfermaUtenti(user.Id, "NO"));

        var exists = await context.Users.AnyAsync(u => u.Id == user.Id);
        Assert.False(exists);
    }

}
