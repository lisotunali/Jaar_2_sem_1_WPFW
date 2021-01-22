using System;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Data;
using WDPR_MVC.Models;
using Xunit;

namespace WDPR_MVC_TESTS
{
    public class BaseTest 
    {
        private string databaseName; // zonder deze property kun je geen clean context maken.

        public MyContext GetInMemoryDBMetData()
        {
            MyContext context = GetNewInMemoryDatabase(true);

            //Replace with other info
            context.Add(new ApplicationUser { UserName = "Jan", Id = "1234", Email = "test1@email.com", Adres = new Adres { Straatnaam = "De Straat", Huisnummer = 243, Postcode = "1237TK" } });
            context.Add(new ApplicationUser { UserName = "Smit", Email = "test2@email.com", Adres = new Adres { Straatnaam = "De Weg", Huisnummer = 432, Postcode = "9342YO" } });
            context.Add(new ApplicationUser { UserName = "Bruh", Email = "test3@email.com", Adres = new Adres { Straatnaam = "De Laan", Huisnummer = 12, Postcode = "4329FY" } });
            context.Categorieen.Add(new Categorie { Naam = "EchteTestCategorie" });

            context.SaveChanges();
            return GetNewInMemoryDatabase(false); // gebruik een nieuw (clean) object voor de context
        }

        private MyContext GetNewInMemoryDatabase(bool NewDb)
        {
            if (NewDb) this.databaseName = Guid.NewGuid().ToString(); // unieke naam

            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(this.databaseName)
                .Options;

            return new MyContext(options);
        }
    }
}
