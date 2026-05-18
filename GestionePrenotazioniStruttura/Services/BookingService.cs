using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionePrenotazioniStruttura.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetByRoomAsync(int roomId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Where(b => b.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<Booking?> CreateAsync(Booking booking)
        {
            booking.StartTime = booking.StartTime.ToUniversalTime();
            booking.EndTime = booking.EndTime.ToUniversalTime();

            bool overlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                booking.StartTime < b.EndTime &&
                booking.EndTime > b.StartTime);

            if (overlap) return null;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking?> UpdateAsync(Booking booking)
        {
            var existing = await _context.Bookings.FindAsync(booking.Id);
            if (existing == null) return null;

            booking.StartTime = booking.StartTime.ToUniversalTime();
            booking.EndTime = booking.EndTime.ToUniversalTime();

            bool overlap = await _context.Bookings.AnyAsync(b =>
                b.Id != booking.Id &&
                b.RoomId == booking.RoomId &&
                booking.StartTime < b.EndTime &&
                booking.EndTime > b.StartTime);

            if (overlap) return null;

            existing.RoomId = booking.RoomId;
            existing.UserId = booking.UserId;

            var activity = await _context.Activities.FindAsync(booking.ActivityId);
            if (activity == null) return null; 
            existing.Activity = activity;

            existing.ActivityId = booking.ActivityId;
            existing.StartTime = booking.StartTime;
            existing.EndTime = booking.EndTime;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
