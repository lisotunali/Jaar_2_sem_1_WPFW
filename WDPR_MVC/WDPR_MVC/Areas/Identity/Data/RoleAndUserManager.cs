using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WDPR_MVC.Data;
using WDPR_MVC.Models;

namespace WDPR_MVC.Areas.Identity.Data
{
    public class RoleAndUserManager
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly RoleManager<IdentityRole> _rolemanager;

        public RoleAndUserManager(UserManager<ApplicationUser> um, RoleManager<IdentityRole> rolemanager)
        {
            _usermanager = um;
            _rolemanager = rolemanager;
        }



        public void CreateRoleManager()
        {
            var t = Task.Run(async () =>
            {
                await _rolemanager.CreateAsync(new IdentityRole { Name = "Mod" });
            });
            t.Wait();
        }

       
    }
}
