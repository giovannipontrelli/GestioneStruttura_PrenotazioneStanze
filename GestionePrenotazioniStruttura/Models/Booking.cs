using System;

namespace GestionePrenotazioniStruttura.Models
{
    public class Booking
    {
        public int Id { get; set; }

        
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
