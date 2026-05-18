using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class RoomServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddRoom_WhenStructureExists()
    {
        var context = GetInMemoryDb();
        context.Structures.Add(new Structure { Id = 1, Name = "S1" });
        await context.SaveChangesAsync();

        var service = new RoomService(context);
        var room = new Room { Name = "Room1", Capacity = 10, StructureId = 1 };
        var result = await service.CreateAsync(room);

        Assert.NotNull(result);
        Assert.Single(context.Rooms);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenStructureMissing()
    {
        var service = new RoomService(GetInMemoryDb());
        var room = new Room { Name = "Room1", Capacity = 10, StructureId = 999 };
        await Assert.ThrowsAsync<BadHttpRequestException>(() => service.CreateAsync(room));
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenRoomExists()
    {
        var context = GetInMemoryDb();
        var room = new Room { Name = "R1", Capacity = 5, StructureId = 1 };
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        var service = new RoomService(context);
        room.Capacity = 10;
        var result = await service.UpdateAsync(room);

        Assert.True(result);
        Assert.Equal(10, (await context.Rooms.FindAsync(room.Id)).Capacity);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
    {
        var service = new RoomService(GetInMemoryDb());
        var room = new Room { Id = 999, Name = "R", Capacity = 5, StructureId = 1 };
        var result = await service.UpdateAsync(room);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRoom_WhenExists()
    {
        var context = GetInMemoryDb();
        var room = new Room { Name = "R1", Capacity = 5, StructureId = 1 };
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        var service = new RoomService(context);
        var result = await service.DeleteAsync(room.Id);

        Assert.True(result);
        Assert.Empty(context.Rooms);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        var service = new RoomService(GetInMemoryDb());
        var result = await service.DeleteAsync(999);
        Assert.False(result);
    }
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRooms()
    {
        
        using var context = GetInMemoryDb();

        
        var structure = new Structure { Id = 1, Name = "Palestra Centrale", Address = "Via Roma 1" };
        context.Structures.Add(structure);
        await context.SaveChangesAsync();

        
        context.Rooms.AddRange(
            new Room { Id = 1, Name = "R1", Capacity = 5, StructureId = 1 },
            new Room { Id = 2, Name = "R2", Capacity = 10, StructureId = 1 }
        );
        await context.SaveChangesAsync();

        var service = new RoomService(context);

        
        var result = await service.GetAllAsync();

        
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.NotNull(r.Structure)); 
    }



    [Fact]
    public async Task GetByStructureAsync_ShouldReturnFilteredRooms()
    {
        var context = GetInMemoryDb();
        context.Rooms.AddRange(new List<Room>
        {
            new Room { Name = "R1", Capacity = 5, StructureId = 1 },
            new Room { Name = "R2", Capacity = 10, StructureId = 2 }
        });
        await context.SaveChangesAsync();

        var service = new RoomService(context);
        var result = await service.GetByStructureAsync(1);

        Assert.Single(result);
        Assert.Equal(1, result[0].StructureId);
    }
}
