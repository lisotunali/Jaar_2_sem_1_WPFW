using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WDPR_MVC.Areas.Identity.Data;

namespace WDPR_MVC.Models
{
    public class Melding
    {
        public int Id { get; set; }

        [Required]
        public string AuteurId { get; set; }
        public ApplicationUser Auteur { get; set; }

        [Required]
        public string Titel { get; set; }

        [Required]
        public string Beschrijving { get; set; }
        public int AantalLikes { get; set; }
        public DateTime DatumAangemaakt { get; set; }
        public int KeerBekeken { get; set; }
        public bool IsClosed { get; set; }
        public bool IsAnonymous { get; set; }

        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
