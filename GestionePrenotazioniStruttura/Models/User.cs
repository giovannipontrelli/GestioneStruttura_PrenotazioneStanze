using System;
using System.Collections.Generic;
using GestionePrenotazioniStruttura.Models.Enum;

namespace GestionePrenotazioniStruttura.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public int? RevisorStructureId { get; set; }

    }
}
