using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Models;

namespace WDPR_MVC.Data
{
    public class MyContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Melding> Meldingen { get; set; }
        public DbSet<BewerkteMelding> BewerkteMeldingen { get; set; }
        public DbSet<Categorie> Categorieen { get; set; }
        public DbSet<Adres> Adres { get; set; }

        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //Unieke categorienaam
            builder.Entity<Categorie>().HasIndex(c => c.Naam).IsUnique();

            //Many-To-Many voor Report
            builder.Entity<Report>()
                .HasKey(t => new { t.MeldingId, t.AuteurReportId });

            builder.Entity<Report>()
                .HasOne(pt => pt.Melding)
                .WithMany(p => p.Reports)
                .HasForeignKey(pt => pt.MeldingId);

            builder.Entity<Report>()
                .HasOne(pt => pt.AuteurReport)
                .WithMany(t => t.Reports)
                .HasForeignKey(pt => pt.AuteurReportId);

            //Composite key
            builder.Entity<MeldingLike>()
                .HasKey(ml => new { ml.MeldingId, ml.UserId });

            // KnownIpStatus
            builder.Entity<KnownIp>()
                .Property(i => i.Status)
                .HasConversion(
                        i => i.ToString(),
                        i => (KnownIpStatus)Enum.Parse(typeof(KnownIpStatus), i));
        }
    }
}
