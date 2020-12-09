using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WDPR_MVC.Areas.Identity.Data;

namespace WDPR_MVC.Models
{
    public class Report
    {
        public int MeldingId { get; set; }
        public Melding Melding { get; set; }
        public string AuteurReportId { get; set; }
        public ApplicationUser AuteurReport { get; set; }
    }
}
