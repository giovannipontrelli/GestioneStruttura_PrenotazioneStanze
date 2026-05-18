using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class SubscriptionServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateSubscription_ShouldAddSubscription()
    {
        var context = GetInMemoryDb();
        var service = new SubscriptionService(context);

        var dto = new CreateSubscriptionDto { Name = "Sub1", TotalPrice = 10 };
        var result = await service.CreateSubscriptionAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Sub1", result.Name);
        Assert.Single(context.Subscriptions);
    }

    [Fact]
    public async Task GetAllSubscriptions_ShouldReturnAll()
    {
        var context = GetInMemoryDb();
        context.Subscriptions.AddRange(new List<Subscription> { new Subscription { Name = "S1" }, new Subscription { Name = "S2" } });
        await context.SaveChangesAsync();

        var service = new SubscriptionService(context);
        var result = await service.GetAllSubscriptionsAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetById_ShouldReturnSubscription_WhenExists()
    {
        var context = GetInMemoryDb();
        var s = new Subscription { Name = "S1" };
        context.Subscriptions.Add(s);
        await context.SaveChangesAsync();

        var service = new SubscriptionService(context);
        var result = await service.GetSubscriptionByIdAsync(s.Id);
        Assert.NotNull(result);
        Assert.Equal("S1", result.Name);
    }

    [Fact]
    public async Task UpdateSubscription_ShouldReturnUpdated()
    {
        var context = GetInMemoryDb();
        var s = new Subscription { Name = "Old" };
        context.Subscriptions.Add(s);
        await context.SaveChangesAsync();

        var service = new SubscriptionService(context);
        var dto = new UpdateSubscriptionDto { Name = "New" };
        var updated = await service.UpdateSubscriptionAsync(s.Id, dto);

        Assert.NotNull(updated);
        Assert.Equal("New", updated.Name);
    }

    [Fact]
    public async Task UpdateSubscription_ShouldThrowException_WhenMissing()
    {
        var service = new SubscriptionService(GetInMemoryDb());
        var dto = new UpdateSubscriptionDto { Name = "X" };

       
        await Assert.ThrowsAsync<Exception>(() => service.UpdateSubscriptionAsync(999, dto));
    }

    [Fact]
    public async Task DeleteSubscription_ShouldReturnTrue_WhenDeleted()
    {
        var context = GetInMemoryDb();
        var s = new Subscription { Name = "S1" };
        context.Subscriptions.Add(s);
        await context.SaveChangesAsync();

        var service = new SubscriptionService(context);
        var deleted = await service.DeleteSubscriptionAsync(s.Id);
        Assert.True(deleted);
        Assert.Empty(context.Subscriptions);
    }

    [Fact]
    public async Task DeleteSubscription_ShouldReturnFalse_WhenMissing()
    {
        var service = new SubscriptionService(GetInMemoryDb());
        var result = await service.DeleteSubscriptionAsync(999);
        Assert.False(result);
    }
}
