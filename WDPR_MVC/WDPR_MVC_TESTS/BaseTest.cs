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
