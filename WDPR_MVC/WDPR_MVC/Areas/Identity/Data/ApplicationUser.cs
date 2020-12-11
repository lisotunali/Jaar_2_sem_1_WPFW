using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WDPR_MVC.Models;

namespace WDPR_MVC.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual List<Melding> Meldingen { get; set; } = new List<Melding>();
        public virtual List<Report> Reports { get; set; } = new List<Report>();
        
        [PersonalData]
        public virtual Adres Adres { get; set; }
        public int AdresId { get; set; }

    }
}
