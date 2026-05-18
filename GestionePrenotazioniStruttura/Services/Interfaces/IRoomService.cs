using GestionePrenotazioniStruttura.Models;

namespace GestionePrenotazioniStruttura.Services.Interfaces
{
    public interface IRoomService
    {
        Task<List<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(int id);
        Task<List<Room>> GetByStructureAsync(int structureId);
        Task<Room> CreateAsync(Room room);
        Task<bool> UpdateAsync(Room room);
        Task<bool> DeleteAsync(int id);
    }

}
