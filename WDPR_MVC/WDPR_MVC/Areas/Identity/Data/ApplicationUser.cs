using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public bool FirstLog { get; set; }

        // All known ip's by this user
        //public virtual List<KnownIp> KnownIps { get; set; }

        public virtual List<Device> Devices { get; set; } = new List<Device>();
    }

    //public class KnownIp
    //{
    //    public int Id { get; set; }

    //    [Required]
    //    public string Ip { get; set; }

    //    [Required]
    //    public KnownIpStatus Status { get; set; }

    //    [Required]
    //    public string UserId { get; set; }
    //    public virtual ApplicationUser User { get; set; }
    //}

    //public enum KnownIpStatus
    //{
    //    // User has verified that this is their ip
    //    Allowed,

    //    // User has verified that this is not their ip
    //    Deny,

    //    // User has not verified the status of this ip
    //    Unknown
    //}

    public class Device
    {
        public int Id { get; set; }

        [Required]
        public string Ip { get; set; }

        [Required]
        public string UserAgent { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
