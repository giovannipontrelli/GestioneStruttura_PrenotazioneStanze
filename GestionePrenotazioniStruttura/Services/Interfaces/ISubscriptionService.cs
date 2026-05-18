public interface ISubscriptionService
{
    Task<ReadSubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto);
    Task<bool> DeleteSubscriptionAsync(int id);
    Task<List<ReadSubscriptionDto>> GetAllSubscriptionsAsync();
    Task<ReadSubscriptionDto?> GetSubscriptionByIdAsync(int id);
    Task<List<ReadSubscriptionDto>> GetByUserAsync(int userId);
    Task<ReadSubscriptionDto> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto dto);
}