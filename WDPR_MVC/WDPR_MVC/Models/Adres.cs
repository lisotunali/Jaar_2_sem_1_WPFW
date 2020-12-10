using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WDPR_MVC.Models
{
    public class Adres
    {

        public int Id { get; set; }
        [Required]
        public string Straatnaam { get; set; }

        [Required]
        public int Huisnummer { get; set; }

        public string Toevoeging { get; set; }

        [Required]
        public string Postcode { get; set; }
    }
}
