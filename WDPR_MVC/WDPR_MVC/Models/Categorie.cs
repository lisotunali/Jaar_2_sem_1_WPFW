using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WDPR_MVC.Models
{
    public class Categorie
    {
        public int Id { get; set; }
        [Required]
        public string Naam { get; set; }

        public ICollection<Melding> Meldingen { get; set; } = new List<Melding>();
    }
}
