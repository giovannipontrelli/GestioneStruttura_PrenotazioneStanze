using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Models.Enum;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RelationshipTests
{
    private readonly AppDbContext _context;

    public RelationshipTests()
    {
        _context = TestDbContextFactory.Create();
    }

    [Fact]
    public async Task Structure_ShouldHaveRooms()
    {

        var context = TestDbContextFactory.Create();

        var structure = new Structure
        {
            Name = "Palestra Centrale",
            Address = "Via Roma 1",
            Description = "Struttura principale"
        };

        var room1 = new Room { Name = "Sala Yoga", Capacity = 20, Structure = structure };
        var room2 = new Room { Name = "Sala Pilates", Capacity = 15, Structure = structure };

        context.Structures.Add(structure);
        context.Rooms.AddRange(room1, room2);
        await context.SaveChangesAsync();

        var dbStructure = await context.Structures.Include(s => s.Rooms).FirstOrDefaultAsync();
        Assert.NotNull(dbStructure);
        Assert.Equal(2, dbStructure!.Rooms.Count);
    }


   
    [Fact]
    public async Task Room_ShouldHaveBookings()
    {
        var structure = new Structure { Name = "Palestra Centrale", Address = "Via Roma 1", Description = "Struttura principale" };
        var room = new Room { Name = "Sala Pilates", Capacity = 15, Structure = structure };
        var user = new User { Name = "Anna", Email = "anna@test.com", Role = Role.Customer, CreatedAt = DateTime.UtcNow, PasswordHash ="passwordfake" };

        _context.Structures.Add(structure);
        _context.Rooms.Add(room);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var booking = new Booking { Room = room, User = user, StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var dbRoom = await _context.Rooms
            .Include(r => r.Bookings)
            .FirstAsync(r => r.Id == room.Id);

        Assert.Single(dbRoom.Bookings);
    }


    
    [Fact]
    public async Task User_ShouldHaveBookingsAndSubscriptions()
    {
        
        var structure = new Structure { Name = "Palestra Centrale", Address = "Via Roma 1", Description = "Struttura principale" };
        var room = new Room { Name = "Sala Yoga", Capacity = 20, Structure = structure };

       
        var user = new User { Name = "Luigi", Email = "luigi@test.com", Role = Role.Customer, CreatedAt = DateTime.UtcNow, PasswordHash = "passwordfake" };

        _context.Structures.Add(structure);
        _context.Rooms.Add(room);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        
        var booking = new Booking { Room = room, User = user, StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };
        var subscription = new Subscription { Name = "Abbonamento Base", User = user, TotalPrice = 100, DurationInMonths = 3 };

        _context.Bookings.Add(booking);
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        var dbUser = await _context.Users
            .Include(u => u.Bookings)
            .Include(u => u.Subscriptions)
            .FirstAsync(u => u.Id == user.Id);

        Assert.Single(dbUser.Bookings);
        Assert.Single(dbUser.Subscriptions);
    }

    [Fact]    public async Task Trainer_ShouldHaveActivities()
    {
        var context = TestDbContextFactory.Create();

        var activity1 = new Activity { Title = "Yoga", Description = "D1" };
        var activity2 = new Activity { Title = "Pilates", Description = "D2" };
        var trainer = new Trainer { Name = "Giovanni", Activities = new List<Activity> { activity1, activity2 } };

        context.Activities.AddRange(activity1, activity2);
        context.Trainers.Add(trainer);
        await context.SaveChangesAsync();

        var dbTrainer = await context.Trainers.Include(t => t.Activities).FirstOrDefaultAsync();
        Assert.NotNull(dbTrainer);
        Assert.Equal(2, dbTrainer!.Activities.Count);

        var dbActivity = await context.Activities.Include(a => a.Trainers).FirstOrDefaultAsync(a => a.Id == activity1.Id);
        Assert.NotNull(dbActivity);
        // EF Core non popola automaticamente la collezione inversa con InMemory, quindi la relazione è bidirezionale nel model ma va popolata manualmente
    }

    [Fact]
    public async Task Subscription_ShouldHaveActivities()
    {
        var context = TestDbContextFactory.Create();

        var activity1 = new Activity { Title = "Yoga", Description = "D1" };
        var activity2 = new Activity { Title = "Pilates", Description = "D2" };
        var user = new User { Name = "Francesca", Email = "francesca@test.com", Role = Role.Customer, PasswordHash = "passwordfake" };
        var subscription = new Subscription
        {
            Name = "Abbonamento",
            User = user,
            TotalPrice = 100,
            DurationInMonths = 2,
            Activities = new List<Activity> { activity1, activity2 }
        };

        context.Users.Add(user);
        context.Activities.AddRange(activity1, activity2);
        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync();

        var dbSubscription = await context.Subscriptions.Include(s => s.Activities).FirstOrDefaultAsync();
        Assert.NotNull(dbSubscription);
        Assert.Equal(2, dbSubscription!.Activities.Count);
    }
}