using GestionePrenotazioniStruttura.Models;

namespace GestionePrenotazioniStruttura.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<List<Booking>> GetByRoomAsync(int roomId);
        Task<Booking?> CreateAsync(Booking booking);
        Task<Booking?> UpdateAsync(Booking booking);
        Task<bool> DeleteAsync(int id);
    }
}
