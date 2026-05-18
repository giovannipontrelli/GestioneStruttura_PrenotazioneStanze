using GestionePrenotazioniStruttura.Models;

namespace GestionePrenotazioniStruttura.Services.Interfaces
{
    public interface IStructureService
    {
        Task<List<Structure>> GetAllAsync();
        Task<Structure?> GetByIdAsync(int id);
        Task<Structure> CreateAsync(Structure structure);
        Task<bool> UpdateAsync(Structure structure);
        Task<bool> DeleteAsync(int id);
    }

}
