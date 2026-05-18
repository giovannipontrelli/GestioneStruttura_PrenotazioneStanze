using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using Microsoft.EntityFrameworkCore;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;
    public SubscriptionService(AppDbContext context) => _context = context;

    public async Task<ReadSubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto)
    {
        var activities = await _context.Activities.Where(a => dto.ActivitiesIds.Contains(a.Id)).ToListAsync();

        var sub = new Subscription
        {
            Name = dto.Name,
            TotalPrice = dto.TotalPrice,
            DurationInMonths = dto.DurationInMonths,
            UserId = dto.UserId,
            Activities = activities
        };

        _context.Subscriptions.Add(sub);
        await _context.SaveChangesAsync();
        return Map(sub);
    }

    public async Task<bool> DeleteSubscriptionAsync(int id)
    {
        var sub = await _context.Subscriptions.FindAsync(id);
        if (sub == null) return false;

        _context.Subscriptions.Remove(sub);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ReadSubscriptionDto>> GetAllSubscriptionsAsync() =>
        (await _context.Subscriptions.Include(s => s.Activities).ToListAsync()).Select(Map).ToList();

    public async Task<ReadSubscriptionDto?> GetSubscriptionByIdAsync(int id)
    {
        var sub = await _context.Subscriptions.Include(s => s.Activities).FirstOrDefaultAsync(s => s.Id == id);
        return sub == null ? null : Map(sub);
    }

    public async Task<List<ReadSubscriptionDto>> GetByUserAsync(int userId) =>
        (await _context.Subscriptions.Include(s => s.Activities).Where(s => s.UserId == userId).ToListAsync()).Select(Map).ToList();

    public async Task<ReadSubscriptionDto> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto dto)
    {
        var sub = await _context.Subscriptions.Include(s => s.Activities).FirstOrDefaultAsync(s => s.Id == id);
        if (sub == null) throw new Exception("Subscription not found");

        sub.Name = dto.Name;
        sub.TotalPrice = dto.TotalPrice;
        sub.DurationInMonths = dto.DurationInMonths;
        sub.UserId = dto.UserId;
        sub.Activities = await _context.Activities.Where(a => dto.ActivitiesIds.Contains(a.Id)).ToListAsync();

        await _context.SaveChangesAsync();
        return Map(sub);
    }

    private static ReadSubscriptionDto Map(Subscription s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        TotalPrice = s.TotalPrice,
        DurationInMonths = s.DurationInMonths,
        ActivitiesIds = s.Activities.Select(a => a.Id).ToList(),
        UserId = s.UserId
    };
}
