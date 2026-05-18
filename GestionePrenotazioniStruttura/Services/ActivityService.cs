using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class ActivityService : IActivityService
{
    private readonly AppDbContext _context;

    public ActivityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ActivityReadDto>> GetAllAsync()
    {
        return await _context.Activities
            .Include(a => a.Trainers)
            .Select(a => new ActivityReadDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Trainers = a.Trainers.Select(t => new TrainerReadDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList(),
                StructureId = a.StructureId
            })
            .ToListAsync();
    }

    public async Task<ActivityReadDto?> GetByIdAsync(int id)
    {
        var activity = await _context.Activities
            .Include(a => a.Trainers)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activity == null) return null;

        return new ActivityReadDto
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            Trainers = activity.Trainers.Select(t => new TrainerReadDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList(),
            StructureId = activity.StructureId
        };
    }

    public async Task<ActivityReadDto> CreateAsync(ActivityCreateDto dto)
    {
       

        var activity = new Activity
        {
            Title = dto.Title,
            Description = dto.Description,
            StructureId = dto.StructureId
        };

        
        if (dto.TrainerIds != null && dto.TrainerIds.Any())
        {
            var trainers = await _context.Trainers
                .Where(t => dto.TrainerIds.Contains(t.Id))
                .ToListAsync();
            activity.Trainers.AddRange(trainers);
        }

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        return new ActivityReadDto
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            Trainers = activity.Trainers.Select(t => new TrainerReadDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList(),
            StructureId = activity.StructureId
        };
    }

    public async Task<bool> UpdateAsync(ActivityUpdateDto dto, int id)
    {
        var activity = await _context.Activities
            .Include(a => a.Trainers)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activity == null) return false;

        activity.Title = dto.Title;
        activity.Description = dto.Description;

       
        if (dto.TrainerIds != null)
        {
            var trainers = await _context.Trainers
                .Where(t => dto.TrainerIds.Contains(t.Id))
                .ToListAsync();
            activity.Trainers.Clear();
            activity.Trainers.AddRange(trainers);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var activity = await _context.Activities.FindAsync(id);
        if (activity == null) return false;

        _context.Activities.Remove(activity);
        await _context.SaveChangesAsync();
        return true;
    }
}
