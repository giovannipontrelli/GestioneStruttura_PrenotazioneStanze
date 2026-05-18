using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class RoomService : IRoomService
{
    private readonly AppDbContext _context;
    public RoomService(AppDbContext context) => _context = context;

    public async Task<List<Room>> GetAllAsync() =>
        await _context.Rooms.Include(r => r.Structure).ToListAsync();

    public async Task<Room?> GetByIdAsync(int id) =>
        await _context.Rooms.Include(r => r.Structure).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<Room>> GetByStructureAsync(int structureId) =>
        await _context.Rooms.Where(r => r.StructureId == structureId).ToListAsync();

    public async Task<Room> CreateAsync(Room room)
    {
        if (!await _context.Structures.AnyAsync(s => s.Id == room.StructureId))
            throw new BadHttpRequestException("StructureId non valido o assente");

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<bool> UpdateAsync(Room room)
    {
        if (!_context.Rooms.Any(r => r.Id == room.Id)) return false;

        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return false;

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }
}
