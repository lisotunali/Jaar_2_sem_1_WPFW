using System;
using System.Collections.Generic;
using Xunit;
using System.Text;
using WDPR_MVC.Data;
using System.Linq;
using WDPR_MVC.Models;
using WDPR_MVC.Controllers;
using Microsoft.AspNetCore.Identity;
using WDPR_MVC.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Test;
using WDPR_MVC;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WDPR_MVC_TESTS
{
    public class MeldingTest : BaseTest
    {
        MyContext db;
        UserStore<ApplicationUser> userstore;
        RoleStore<IdentityRole> rolestore;
        UserManager<ApplicationUser> _um;
        RoleManager<IdentityRole> _rm;
        MeldingController _mc;
        Mock<IAuthorizationService> _as;


        public MeldingTest()
        {
            CleanContext();
        }

        public void CleanContext()
        {
            db = GetInMemoryDBMetData();
            userstore = new UserStore<ApplicationUser>(db);
            rolestore = new RoleStore<IdentityRole>(db);
            _um = MockHelpers.TestUserManager<ApplicationUser>(userstore);
            _rm = MockHelpers.TestRoleManager<IdentityRole>(rolestore);
            _as = new Mock<IAuthorizationService>();
            _mc = new MeldingController(db, _um, _rm, _as.Object);
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1234" )
            }, authenticationType: "Basic"));

            _mc.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public void TestMeldingModel()
        {
            CleanContext();
            var user = db.Users.First();
            DateTime tijd = DateTime.Now;
            db.Meldingen.Add(new WDPR_MVC.Models.Melding
            {
                Auteur = user,
                Beschrijving = "test",
                Categorie = db.Categorieen.First(),
                DatumAangemaakt = tijd,
                Titel = "testtitel"
            }); 
            Assert.Equal(0, db.Meldingen.Count());
            db.SaveChanges();
            Assert.Equal(1, db.Meldingen.Count());
            Assert.Equal("test", db.Meldingen.First().Beschrijving);
        }

        [Fact]
        public async void TestMeldingCreate()
        {
            CleanContext();
            DateTime tijd = DateTime.Now;
            Melding melding = new Melding
            {
                Beschrijving = "test",
                Categorie = db.Categorieen.First(),
                DatumAangemaakt = tijd,
                Titel = "testtitel"
            };

            Assert.Equal(0, db.Meldingen.Count());
            await _mc.Create(melding);
            Assert.Equal(1, db.Meldingen.Count());

        }
    }
}
