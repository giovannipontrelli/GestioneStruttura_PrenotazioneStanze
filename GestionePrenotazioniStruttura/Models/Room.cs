using System.Collections.Generic;

namespace GestionePrenotazioniStruttura.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }

        public int StructureId { get; set; }
        public Structure Structure { get; set; } = null!;

        
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
