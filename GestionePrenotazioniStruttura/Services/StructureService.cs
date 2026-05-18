using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class StructureService : IStructureService
{
    private readonly AppDbContext _context;
    public StructureService(AppDbContext context) => _context = context;

    public async Task<List<Structure>> GetAllAsync() =>
        await _context.Structures.Include(s => s.Rooms).ToListAsync();

    public async Task<Structure?> GetByIdAsync(int id) =>
        await _context.Structures.Include(s => s.Rooms).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Structure> CreateAsync(Structure structure)
    {
        _context.Structures.Add(structure);
        await _context.SaveChangesAsync();
        return structure;
    }

    public async Task<bool> UpdateAsync(Structure structure)
    {
        if (!_context.Structures.Any(s => s.Id == structure.Id)) return false;

        _context.Structures.Update(structure);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var structure = await _context.Structures.FindAsync(id);
        if (structure == null) return false;

        _context.Structures.Remove(structure);
        await _context.SaveChangesAsync();
        return true;
    }
}
