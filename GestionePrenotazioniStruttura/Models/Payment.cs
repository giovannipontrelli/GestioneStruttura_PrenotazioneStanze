using System;

namespace GestionePrenotazioniStruttura.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;
    }
}
