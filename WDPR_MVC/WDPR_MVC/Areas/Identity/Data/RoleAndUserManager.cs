using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                await _usermanager.AddToRoleAsync(_usermanager.Users.First(m => m.Email == "lisotunali@hotmail.com"), "mod");

            });
            t.Wait();
        }
    }
}
