using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

public class ActivityServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private ClaimsPrincipal GetUserWithStructure(int structureId)
    {
        var claims = new List<Claim>
        {
            new Claim("StructureId", structureId.ToString())
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActivities()
    {
        var context = GetInMemoryDb();
        context.Activities.Add(new Activity { Title = "Yoga", StructureId = 1 });
        context.Activities.Add(new Activity { Title = "Pilates", StructureId = 1 });
        await context.SaveChangesAsync();

        var service = new ActivityService(context);
        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, a => a.Title == "Yoga");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectActivity()
    {
        var context = GetInMemoryDb();
        var activity = new Activity { Title = "Yoga", StructureId = 1 };
        context.Activities.Add(activity);
        await context.SaveChangesAsync();

        var service = new ActivityService(context);
        var result = await service.GetByIdAsync(activity.Id);

        Assert.NotNull(result);
        Assert.Equal("Yoga", result.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateActivity_WithTrainers()
    {
        var context = GetInMemoryDb();
        var trainer = new Trainer { Name = "Mario" };
        context.Trainers.Add(trainer);
        var activity = new Activity { Title = "Yoga", StructureId = 1 };
        context.Activities.Add(activity);
        await context.SaveChangesAsync();

        var service = new ActivityService(context);
        var dto = new ActivityUpdateDto
        {
            Title = "Pilates",
            Description = "New Desc",
            TrainerIds = new List<int> { trainer.Id }
        };

        var updated = await service.UpdateAsync(dto, activity.Id);

        Assert.True(updated);
        var dbActivity = await context.Activities.Include(a => a.Trainers).FirstAsync();
        Assert.Equal("Pilates", dbActivity.Title);
        Assert.Single(dbActivity.Trainers);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveActivity()
    {
        var context = GetInMemoryDb();
        var activity = new Activity { Title = "Yoga", StructureId = 1 };
        context.Activities.Add(activity);
        await context.SaveChangesAsync();

        var service = new ActivityService(context);
        var deleted = await service.DeleteAsync(activity.Id);

        Assert.True(deleted);
        Assert.Empty(context.Activities);
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ShouldReturnFalse()
    {
        var context = GetInMemoryDb();
        var service = new ActivityService(context);
        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
}
