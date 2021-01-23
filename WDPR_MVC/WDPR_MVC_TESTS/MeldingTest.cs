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

        public MeldingTest()
        {
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
