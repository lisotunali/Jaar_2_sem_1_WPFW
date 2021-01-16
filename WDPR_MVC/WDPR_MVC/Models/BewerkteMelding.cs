using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WDPR_MVC.Models
{
    public class BewerkteMelding
    {
        public int Id { get; set; }
        public int MeldingId { get; set; }
        public virtual Melding Melding { get; set; }
        public string Titel { get; set; }
        public string Beschrijving { get; set; }

        
        
    }
}
