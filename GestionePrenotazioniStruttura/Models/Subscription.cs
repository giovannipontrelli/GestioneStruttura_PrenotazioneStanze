using System.Collections.Generic;

namespace GestionePrenotazioniStruttura.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double TotalPrice { get; set; }
        public int DurationInMonths { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? PaymentId { get; set; }
        public Payment? Payment { get; set; }

        
        public List<Activity> Activities { get; set; } = new List<Activity>();
    }
}
