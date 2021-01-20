using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Models.CustomValidation;

namespace WDPR_MVC.Models
{
    public class Melding
    {
        public int Id { get; set; }

        [Required]
        public string AuteurId { get; set; }
        public virtual ApplicationUser Auteur { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Titel { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(2000)]
        public string Beschrijving { get; set; }
        public DateTime DatumAangemaakt { get; set; }
        public int KeerBekeken { get; set; }
        public bool IsClosed { get; set; }
        public bool IsAnonymous { get; set; }

        public int CategorieId { get; set; }
        public virtual Categorie Categorie { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

        // We moeten bijhouden wie er allemaal heeft geliked
        public virtual ICollection<MeldingLike> Likes { get; set; } = new List<MeldingLike>();

        // Used for upload image
        [NotMapped]
        [AllowedImageExtensions]
        [MaxFileSize(10000000, ErrorMessage = "{0} is too large. Max 10 MB")]
        public IFormFile Image { get; set; }

        // Unieke naam voor de image
        public string ImageName { get; set; }
    }
}
