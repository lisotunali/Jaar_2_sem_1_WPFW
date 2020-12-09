using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WDPR_MVC.Areas.Identity.Data;

namespace WDPR_MVC.Models
{
    public class Melding
    {
        public int Id { get; set; }

        public string AuteurId { get; set; }
        public ApplicationUser Auteur { get; set; }

        public string Titel { get; set; }
        public string Beschrijving { get; set; }
        public int AantalLikes { get; set; }
        public DateTime DatumAangemaakt { get; set; }
        public int KeerBekeken { get; set; }
        public bool IsClosed { get; set; }
        public IEnumerable<Categorie> Categorie { get; set; } = new List<Categorie>();
        public int CategorieId { get; set; }
        public bool IsAnonymous { get; set; }
        public List<Report> Reports { get; set; } = new List<Report>();
    }
}
