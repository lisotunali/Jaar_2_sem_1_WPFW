using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WDPR_MVC.Models
{
    public class GerapporteerdeMelding
    {
        public int Id { get; set; }
        public int MeldingId { get; set; }
        public virtual Melding Melding { get; set; }
    }
}
