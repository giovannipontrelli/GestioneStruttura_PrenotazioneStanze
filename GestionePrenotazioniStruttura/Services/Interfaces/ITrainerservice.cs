using GestionePrenotazioniStruttura.Models;

namespace GestionePrenotazioniStruttura.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<List<TrainerReadDto>> GetAllAsync();
        Task<TrainerReadDto?> GetByIdAsync(int id);
        Task<TrainerReadDto> CreateAsync(TrainerCreateDto dto);
        Task<bool> UpdateAsync(TrainerUpdateDto dto, int id);
        Task<bool> DeleteAsync(int id);
    }
}
