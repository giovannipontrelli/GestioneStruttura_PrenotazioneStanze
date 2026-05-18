using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.DTOs;
using GestionePrenotazioniStruttura.Models;
using GestionePrenotazioniStruttura.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;

    public PaymentService(AppDbContext context) => _context = context;

    public async Task<ReadPaymentDto> CreateAsync(CreatePaymentDto dto)
    {
        var payment = new Payment
        {
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            PaymentMethod = dto.PaymentMethod,
            SubscriptionId = dto.SubscriptionId
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return Map(payment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return false;

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ReadPaymentDto>> GetAllAsync()
    {
        return await _context.Payments.Select(p => Map(p)).ToListAsync();
    }

    public async Task<ReadPaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        return payment == null ? null : Map(payment);
    }

    public async Task<ReadPaymentDto?> GetBySubscriptionAsync(int subscriptionId)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.SubscriptionId == subscriptionId);
        return payment == null ? null : Map(payment);
    }

    public async Task<bool> UpdateAsync(UpdatePaymentDto dto, int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return false;

        payment.Amount = dto.Amount;
        payment.PaymentDate = dto.PaymentDate;
        payment.PaymentMethod = dto.PaymentMethod;
        payment.SubscriptionId = dto.SubscriptionId;

        await _context.SaveChangesAsync();
        return true;
    }

    private static ReadPaymentDto Map(Payment p) => new()
    {
        Id = p.Id,
        Amount = p.Amount,
        PaymentDate = p.PaymentDate,
        PaymentMethod = p.PaymentMethod,
        SubscriptionId = p.SubscriptionId
    };
}
