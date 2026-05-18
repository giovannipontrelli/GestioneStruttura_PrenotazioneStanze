using GestionePrenotazioniStruttura.Models;
using System.Security.Claims;

namespace GestionePrenotazioniStruttura.Services.Interfaces
{
    public interface IActivityService
    {
        Task<List<ActivityReadDto>> GetAllAsync();
        Task<ActivityReadDto?> GetByIdAsync(int id);
        Task<ActivityReadDto> CreateAsync(ActivityCreateDto dto);
        Task<bool> UpdateAsync(ActivityUpdateDto dto, int id);
        Task<bool> DeleteAsync(int id);
    }

}


