using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class BookingServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        using var context = GetInMemoryDb();

        
        var room = new Room { Id = 1, Name = "Sala 1", Capacity = 10, StructureId = 1 };
        var user = new User { Id = 1, Name = "testuser", Email = "test@test.com" };

        context.Rooms.Add(room);
        context.Users.Add(user);
        await context.SaveChangesAsync(); 

       
        context.Bookings.AddRange(
            new Booking { Id = 1, RoomId = 1, UserId = 1, StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) },
            new Booking { Id = 2, RoomId = 1, UserId = 1, StartTime = DateTime.UtcNow.AddHours(2), EndTime = DateTime.UtcNow.AddHours(3) }
        );
        await context.SaveChangesAsync();

        var service = new BookingService(context);

       
        var result = await service.GetAllAsync();

      
        Assert.Equal(2, result.Count);
        Assert.NotNull(result[0].Room); 
    }
    [Fact]
    public async Task GetByIdAsync_ShouldReturnBooking_WhenExists()
    {
        using var context = GetInMemoryDb();

       
        context.Rooms.Add(new Room { Id = 1, Name = "Sala 1", StructureId = 1 });
        context.Users.Add(new User { Id = 1, Name = "User1" });

        var booking = new Booking { RoomId = 1, UserId = 1, StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync(); 

        var service = new BookingService(context);

        
        var result = await service.GetByIdAsync(booking.Id);

        Assert.NotNull(result);
        Assert.Equal(booking.Id, result!.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddBooking_WhenNoOverlap()
    {
        var context = GetInMemoryDb();
        var service = new BookingService(context);

        var booking = new Booking
        {
            RoomId = 1,
            UserId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2)
        };

        var created = await service.CreateAsync(booking);

        Assert.NotNull(created);
        Assert.Equal(1, context.Bookings.Count());
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNull_WhenOverlap()
    {
        var context = GetInMemoryDb();
        var service = new BookingService(context);

        var existing = new Booking { RoomId = 1, UserId = 1, StartTime = DateTime.UtcNow.AddHours(1), EndTime = DateTime.UtcNow.AddHours(2) };
        context.Bookings.Add(existing);
        await context.SaveChangesAsync();

        var overlapping = new Booking { RoomId = 1, UserId = 2, StartTime = DateTime.UtcNow.AddHours(1).AddMinutes(30), EndTime = DateTime.UtcNow.AddHours(2).AddMinutes(30) };
        var result = await service.CreateAsync(overlapping);

        Assert.Null(result);
        Assert.Single(context.Bookings);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenOverlap()
    {
        var context = GetInMemoryDb();
        var booking1 = new Booking { RoomId = 1, UserId = 1, StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };
        var booking2 = new Booking { RoomId = 1, UserId = 2, StartTime = DateTime.UtcNow.AddHours(1), EndTime = DateTime.UtcNow.AddHours(2) };
        context.Bookings.AddRange(booking1, booking2);
        await context.SaveChangesAsync();

        var service = new BookingService(context);
        booking1.StartTime = DateTime.UtcNow.AddMinutes(30);
        booking1.EndTime = DateTime.UtcNow.AddHours(1).AddMinutes(30);

        var result = await service.UpdateAsync(booking1);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBooking()
    {
        var context = GetInMemoryDb();
        var booking = new Booking { RoomId = 1, UserId = 1, StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var service = new BookingService(context);
        var deleted = await service.DeleteAsync(booking.Id);

        Assert.True(deleted);
        Assert.Empty(context.Bookings);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        var context = GetInMemoryDb();
        var service = new BookingService(context);
        var result = await service.DeleteAsync(999);
        Assert.False(result);
    }
}