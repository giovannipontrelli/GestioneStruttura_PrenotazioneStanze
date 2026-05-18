using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class PaymentServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddPayment()
    {
        var context = GetInMemoryDb();
        var service = new PaymentService(context);

        var dto = new CreatePaymentDto
        {
            Amount = 50,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = "CreditCard",
            SubscriptionId = 1
        };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Amount, result.Amount);
        Assert.Single(context.Payments);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPayments()
    {
        var context = GetInMemoryDb();
        context.Payments.AddRange(new List<Payment>
        {
            new Payment { Amount=10, PaymentDate=DateTime.UtcNow, PaymentMethod="Cash", SubscriptionId=1 },
            new Payment { Amount=20, PaymentDate=DateTime.UtcNow, PaymentMethod="Card", SubscriptionId=2 }
        });
        await context.SaveChangesAsync();

        var service = new PaymentService(context);
        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPayment_WhenExists()
    {
        var context = GetInMemoryDb();
        var payment = new Payment { Amount = 10, PaymentDate = DateTime.UtcNow, PaymentMethod = "Cash", SubscriptionId = 1 };
        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        var service = new PaymentService(context);
        var result = await service.GetByIdAsync(payment.Id);

        Assert.NotNull(result);
        Assert.Equal(payment.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var service = new PaymentService(GetInMemoryDb());
        var result = await service.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenUpdated()
    {
        var context = GetInMemoryDb();
        var payment = new Payment { Amount = 10, PaymentDate = DateTime.UtcNow, PaymentMethod = "Cash", SubscriptionId = 1 };
        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        var service = new PaymentService(context);
        var dto = new UpdatePaymentDto { Amount = 50, PaymentDate = DateTime.UtcNow, PaymentMethod = "Card", SubscriptionId = 1 };

        var result = await service.UpdateAsync(dto, payment.Id);
        Assert.True(result);

        var updated = await context.Payments.FindAsync(payment.Id);
        Assert.Equal(50, updated.Amount);
        Assert.Equal("Card", updated.PaymentMethod);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
    {
        var service = new PaymentService(GetInMemoryDb());
        var dto = new UpdatePaymentDto { Amount = 50, PaymentDate = DateTime.UtcNow, PaymentMethod = "Card", SubscriptionId = 1 };
        var result = await service.UpdateAsync(dto, 999);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemovePayment()
    {
        var context = GetInMemoryDb();
        var payment = new Payment { Amount = 10, PaymentDate = DateTime.UtcNow, PaymentMethod = "Cash", SubscriptionId = 1 };
        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        var service = new PaymentService(context);
        var deleted = await service.DeleteAsync(payment.Id);

        Assert.True(deleted);
        Assert.Empty(context.Payments);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        var service = new PaymentService(GetInMemoryDb());
        var result = await service.DeleteAsync(999);
        Assert.False(result);
    }
}
