using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GestionePrenotazioniStruttura.Tests
{
    public class SubscriptionServiceTests
    {
        [Fact]
        /*
         Test CreateSubscriptionAsync.
         Verifica che la subscription venga creata correttamente con le attività associate.
        */
        public async Task CreateSubscription_ShouldMapActivities()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubscriptionService(context);

            var activity1 = new Activity { Title = "A1", Description = "D1" };
            var activity2 = new Activity { Title = "A2", Description = "D2" };
            context.Activities.AddRange(activity1, activity2);
            await context.SaveChangesAsync();

            var dto = new CreateSubscriptionDto
            {
                Name = "Sub1",
                TotalPrice = 200,
                DurationInMonths = 2,
                ActivitiesIds = new List<int> { activity1.Id, activity2.Id },
                UserId = 1,
                PaymentId = 1
            };

            var result = await service.CreateSubscriptionAsync(dto);
            Assert.Equal(2, result.ActivitiesIds.Count);
            Assert.Equal("Sub1", result.Name);
        }

        [Fact]
        /*
         Test UpdateSubscriptionAsync.
         Verifica che la subscription esistente venga aggiornata correttamente
         e che lancia eccezione se la subscription non esiste.
        */
        public async Task UpdateSubscription_ShouldModifyOrFail()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubscriptionService(context);

            var activity1 = new Activity { Title = "A1", Description = "D1" };
            var activity2 = new Activity { Title = "A2", Description = "D2" };
            context.Activities.AddRange(activity1, activity2);
            await context.SaveChangesAsync();

            var sub = new Subscription
            {
                Name = "Old",
                TotalPrice = 100,
                DurationInMonths = 1,
                UserId = 1,
                Activities = new List<Activity> { activity1 }
            };
            context.Subscriptions.Add(sub);
            await context.SaveChangesAsync();

            var updateDto = new UpdateSubscriptionDto
            {
                Name = "New",
                TotalPrice = 150,
                DurationInMonths = 3,
                ActivitiesIds = new List<int> { activity2.Id },
                UserId = 1,
                PaymentId = 1
            };

            var result = await service.UpdateSubscriptionAsync(sub.Id, updateDto);
            Assert.Equal("New", result.Name);
            Assert.Single(result.ActivitiesIds);
            Assert.Equal(activity2.Id, result.ActivitiesIds.First());

           
            await Assert.ThrowsAsync<Exception>(() => service.UpdateSubscriptionAsync(999, updateDto));
        }

        [Fact]
        public async Task DeleteSubscription_ShouldRemoveOrFail()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubscriptionService(context);

            var sub = new Subscription { Name = "S1", TotalPrice = 100, DurationInMonths = 1, UserId = 1 };
            context.Subscriptions.Add(sub);
            await context.SaveChangesAsync();

            var result = await service.DeleteSubscriptionAsync(sub.Id);
            Assert.True(result);
            Assert.Empty(context.Subscriptions);

            var fail = await service.DeleteSubscriptionAsync(999);
            Assert.False(fail);
        }

        [Fact]
        public async Task GetSubscriptions_ShouldReturnCorrectly()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubscriptionService(context);

            var sub1 = new Subscription { Name = "S1", TotalPrice = 100, DurationInMonths = 1, UserId = 1 };
            var sub2 = new Subscription { Name = "S2", TotalPrice = 150, DurationInMonths = 2, UserId = 2 };
            context.Subscriptions.AddRange(sub1, sub2);
            await context.SaveChangesAsync();

            var all = await service.GetAllSubscriptionsAsync();
            Assert.Equal(2, all.Count);

            var byId = await service.GetSubscriptionByIdAsync(sub1.Id);
            Assert.Equal("S1", byId!.Name);

            var byUser = await service.GetByUserAsync(2);
            Assert.Single(byUser);
            Assert.Equal("S2", byUser.First().Name);
        }

    }
}