using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class TrainerServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddTrainer()
    {
        var context = GetInMemoryDb();
        var service = new TrainerService(context);
        var dto = new TrainerCreateDto { Name = "T1" };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Single(context.Trainers);
        Assert.Equal("T1", result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTrainers()
    {
        var context = GetInMemoryDb();
        context.Trainers.AddRange(new List<Trainer> { new Trainer { Name = "T1" }, new Trainer { Name = "T2" } });
        await context.SaveChangesAsync();

        var service = new TrainerService(context);
        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTrainer_WhenExists()
    {
        var context = GetInMemoryDb();
        var t = new Trainer { Name = "T1" };
        context.Trainers.Add(t);
        await context.SaveChangesAsync();

        var service = new TrainerService(context);
        var result = await service.GetByIdAsync(t.Id);

        Assert.NotNull(result);
        Assert.Equal("T1", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenUpdated()
    {
        var context = GetInMemoryDb();
        var t = new Trainer { Name = "Old" };
        context.Trainers.Add(t);
        await context.SaveChangesAsync();

        var service = new TrainerService(context);
        var dto = new TrainerUpdateDto { Name = "New" };
        var updated = await service.UpdateAsync(dto, t.Id);

        Assert.True(updated);
        Assert.Equal("New", (await context.Trainers.FindAsync(t.Id)).Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenMissing()
    {
        var service = new TrainerService(GetInMemoryDb());
        var dto = new TrainerUpdateDto { Name = "X" };
        var result = await service.UpdateAsync(dto, 999);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTrainer()
    {
        var context = GetInMemoryDb();
        var t = new Trainer { Name = "T1" };
        context.Trainers.Add(t);
        await context.SaveChangesAsync();

        var service = new TrainerService(context);
        var deleted = await service.DeleteAsync(t.Id);

        Assert.True(deleted);
        Assert.Empty(context.Trainers);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenMissing()
    {
        var service = new TrainerService(GetInMemoryDb());
        var result = await service.DeleteAsync(999);
        Assert.False(result);
    }
}
