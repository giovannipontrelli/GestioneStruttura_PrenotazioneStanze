using System.Collections.Generic;

namespace GestionePrenotazioniStruttura.Models
{
    public class Structure
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

       
        public List<Room> Rooms { get; set; } = new List<Room>();

       
        public List<Activity> Activities { get; set; } = new List<Activity>();
    }
}
