using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WDPR_MVC.Areas.Identity.Data;

namespace WDPR_MVC.Models
{
    public class MeldingLike
    {
        public int MeldingId { get; set; }
        public virtual Melding Melding { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
