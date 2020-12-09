using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WDPR_MVC.Areas.Identity.Data;

namespace WDPR_MVC.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int MeldingId { get; set; }
        public Melding Melding { get; set; }

        [Required]
        public string AuteurCommentId { get; set; }
        public ApplicationUser AuteurComment { get; set; }

        [Required]
        public string Inhoud { get; set; }
        public DateTime DatumAangemaakt { get; set; }
        public int AantalLikes { get; set; }
    }
}
