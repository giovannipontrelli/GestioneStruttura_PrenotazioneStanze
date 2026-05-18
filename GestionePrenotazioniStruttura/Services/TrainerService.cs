using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class TrainerService : ITrainerService
{
    private readonly AppDbContext _context;
    public TrainerService(AppDbContext context) => _context = context;

    public async Task<List<TrainerReadDto>> GetAllAsync() =>
        (await _context.Trainers.Include(t => t.Activities).ToListAsync())
        .Select(t => new TrainerReadDto
        {
            Id = t.Id,
            Name = t.Name,
            Activities = t.Activities.Select(a => new ActivityReadDto { Id = a.Id, Title = a.Title }).ToList()
        }).ToList();

    public async Task<TrainerReadDto?> GetByIdAsync(int id)
    {
        var trainer = await _context.Trainers.Include(t => t.Activities).FirstOrDefaultAsync(t => t.Id == id);
        if (trainer == null) return null;

        return new TrainerReadDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Activities = trainer.Activities.Select(a => new ActivityReadDto { Id = a.Id, Title = a.Title }).ToList()
        };
    }

    public async Task<TrainerReadDto> CreateAsync(TrainerCreateDto dto)
    {
        var trainer = new Trainer { Name = dto.Name };

        if (dto.ActivityIds != null && dto.ActivityIds.Any())
        {
            trainer.Activities = await _context.Activities.Where(a => dto.ActivityIds.Contains(a.Id)).ToListAsync();
        }

        _context.Trainers.Add(trainer);
        await _context.SaveChangesAsync();

        return new TrainerReadDto { Id = trainer.Id, Name = trainer.Name };
    }

    public async Task<bool> UpdateAsync(TrainerUpdateDto dto, int id)
    {
        var trainer = await _context.Trainers.Include(t => t.Activities).FirstOrDefaultAsync(t => t.Id == id);
        if (trainer == null) return false;

        trainer.Name = dto.Name;
        if (dto.ActivityIds != null)
            trainer.Activities = await _context.Activities.Where(a => dto.ActivityIds.Contains(a.Id)).ToListAsync();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var trainer = await _context.Trainers.FindAsync(id);
        if (trainer == null) return false;

        _context.Trainers.Remove(trainer);
        await _context.SaveChangesAsync();
        return true;
    }
}
