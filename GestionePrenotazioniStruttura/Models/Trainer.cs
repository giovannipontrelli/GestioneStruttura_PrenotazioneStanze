using System.Collections.Generic;

namespace GestionePrenotazioniStruttura.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        
        public List<Activity> Activities { get; set; } = new List<Activity>();
    }
}
