using System.Collections.Generic;

namespace GestionePrenotazioniStruttura.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double PriceForMonth { get; set; }

       
        public List<Trainer> Trainers { get; set; } = new List<Trainer>();

        
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

       
        public List<Booking> Bookings { get; set; } = new List<Booking>();

       
        public int StructureId { get; set; }
        public Structure Structure { get; set; } = null!;
    }
}
