using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    public UserService(AppDbContext context) => _context = context;

    public async Task<List<User>> GetAllAsync() => await _context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        if (!_context.Users.Any(u => u.Id == user.Id)) return false;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
