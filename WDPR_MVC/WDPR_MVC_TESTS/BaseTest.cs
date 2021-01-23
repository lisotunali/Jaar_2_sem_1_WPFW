using System;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Data;
using WDPR_MVC.Models;
using Xunit;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using WDPR_MVC.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Test;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace WDPR_MVC_TESTS
{
    public class BaseTest 
    {
        // declaraties van de mocks van alle services die we gebruiken in de applicatie

        public MyContext db;
        public UserStore<ApplicationUser> userstore;
        public RoleStore<IdentityRole> rolestore;
        public UserManager<ApplicationUser> _um;
        public RoleManager<IdentityRole> _rm;
        public Mock<IAuthorizationService> _as;

        private string databaseName; // zonder deze property kun je geen clean context maken.


        //zet nieuwe, lege mock objecten in de declaraties
        public void CleanContext()
        {
            db = GetInMemoryDBMetData();
            userstore = new UserStore<ApplicationUser>(db);
            rolestore = new RoleStore<IdentityRole>(db);
            _um = MockHelpers.TestUserManager<ApplicationUser>(userstore);
            _rm = MockHelpers.TestRoleManager<IdentityRole>(rolestore);
            _as = new Mock<IAuthorizationService>();
        }

        // met deze methode kunnen we een user naar keuze en zijn bijbehorende claims in "User" zetten en deze daarna gebruiken.
        // hiermee kunnen we dus authorisatie testen maar ook simpelweg getUserAsync(User) gebruiken. 
        public void SetClaimsPrincipal(Controller controller, string userId)
        {
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId )
            }, authenticationType: "Basic"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        public MyContext GetInMemoryDBMetData()
        {
            MyContext context = GetNewInMemoryDatabase(true);

            //Replace with other info
            //Users used for testing
            var user1 = new ApplicationUser
            {
                UserName = "Jan",
                Id = "1234",
                Email = "test1@email.com",
                Adres = new Adres { Straatnaam = "De Straat", Huisnummer = 243, Postcode = "1237TK" }
            };

            var user2 = new ApplicationUser
           {
                UserName = "Smit",
                Email = "test2@email.com",
                Adres = new Adres { Straatnaam = "De Weg", Huisnummer = 432,
                    Postcode = "9342YO"
                }
            };

            var user3 = new ApplicationUser 
            { 
                UserName = "Bruh", 
                Email = "test3@email.com", 
                Adres = new Adres { Straatnaam = "De Laan", Huisnummer = 12, Postcode = "4329FY" } 
            };

            //Categorieën voor testen
            var EchteTestCategorie = new Categorie { Naam = "EchteTestCategorie" };
            var NeppeTestCategorie = new Categorie { Naam = "NeppeTestCategorie" };

            //Meldingen voor testen
            var melding1 = new Melding { Id = 1, Auteur = user1, Titel = "Nieuwe titel", Beschrijving = "Dit is een beschrijving van een paar woorden.", DatumAangemaakt = DateTime.Now, Categorie = EchteTestCategorie};
            var melding2 = new Melding { Id = 2, Auteur = user2, Titel = "Naam voor de neppe melding", Beschrijving = "Beschrijving voor een nieuwe melding", DatumAangemaakt = DateTime.Now, Categorie = NeppeTestCategorie };
            var melding3 = new Melding { Id = 3, Auteur = user3, Titel = "Titel voor de echte melding", Beschrijving = "Hier komt de tekst voor een nieuwe melding te staan.", DatumAangemaakt = DateTime.Now, Categorie = EchteTestCategorie };

            //Likes voor melding2, 2 likes
            var meldingLike1 = new MeldingLike { MeldingId = 2, User = user1 };
            var meldingLike2 = new MeldingLike { MeldingId = 2, User = user3 };
            //Likes toevoegen aan de list 'Likes' van melding2
            melding2.Likes.Add(meldingLike1);
            melding2.Likes.Add(meldingLike2);

            //Alle test objecten toevoegen aan de 'database'
            context.Add(user1);
            context.Add(user2);
            context.Add(user3);
            context.Categorieen.Add(EchteTestCategorie);
            context.Categorieen.Add(NeppeTestCategorie);
            context.Meldingen.Add(melding1);
            context.Meldingen.Add(melding2);
            context.Meldingen.Add(melding3);
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
