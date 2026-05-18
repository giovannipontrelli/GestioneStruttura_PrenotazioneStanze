using GestionePrenotazioniStruttura.DTOs;

public interface IPaymentService
{
    Task<List<ReadPaymentDto>> GetAllAsync();
    Task<ReadPaymentDto?> GetByIdAsync(int id);
    Task<ReadPaymentDto?> GetBySubscriptionAsync(int subscriptionId);
    Task<ReadPaymentDto> CreateAsync(CreatePaymentDto dto);
    Task<bool> UpdateAsync(UpdatePaymentDto dto, int id);
    Task<bool> DeleteAsync(int id);
}
