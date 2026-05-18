using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class UserServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddUser()
    {
        var context = GetInMemoryDb();
        var service = new UserService(context);

        var user = new User { Name = "U1", Email = "u1@test.com", Role = Role.Customer };
        var created = await service.CreateAsync(user);

        Assert.NotNull(created);
        Assert.Single(context.Users);
        Assert.Equal("U1", created.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        var context = GetInMemoryDb();
        context.Users.AddRange(new List<User> { new User { Name = "U1" }, new User { Name = "U2" } });
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var result = await service.GetAllAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
    {
        var context = GetInMemoryDb();
        var u = new User { Name = "U1" };
        context.Users.Add(u);
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var result = await service.GetByIdAsync(u.Id);

        Assert.NotNull(result);
        Assert.Equal("U1", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenUpdated()
    {
        var context = GetInMemoryDb();
        var u = new User { Name = "Old", Role = Role.Customer };
        context.Users.Add(u);
        await context.SaveChangesAsync();

        var service = new UserService(context);
        u.Name = "New";
        var updated = await service.UpdateAsync(u);

        Assert.True(updated);
        Assert.Equal("New", (await context.Users.FindAsync(u.Id)).Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenMissing()
    {
        var service = new UserService(GetInMemoryDb());
        var u = new User { Id = 999, Name = "X", Role = Role.Customer };
        var result = await service.UpdateAsync(u);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser()
    {
        var context = GetInMemoryDb();
        var u = new User { Name = "U1" };
        context.Users.Add(u);
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var deleted = await service.DeleteAsync(u.Id);
        Assert.True(deleted);
        Assert.Empty(context.Users);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenMissing()
    {
        var service = new UserService(GetInMemoryDb());
        var result = await service.DeleteAsync(999);
        Assert.False(result);
    }
}
