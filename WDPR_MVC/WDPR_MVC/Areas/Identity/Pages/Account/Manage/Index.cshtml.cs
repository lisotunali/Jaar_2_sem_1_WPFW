using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Data;
using WDPR_MVC.Models;

namespace WDPR_MVC.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MyContext _mycontext;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            MyContext mycontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mycontext = mycontext;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Adres")]
            public Adres Adres { get; set; }
            
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Adres = user.Adres
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            // checking if all the address inputs are changed
            user.Adres = _mycontext.Adres.Find(user.AdresId);

            if (Input.Adres.Straatnaam != user.Adres.Straatnaam)
            {
                user.Adres.Straatnaam = Input.Adres.Straatnaam;
            }
            if (Input.Adres.Huisnummer != user.Adres.Huisnummer)
            {
                user.Adres.Huisnummer = Input.Adres.Huisnummer;
            }
            if (Input.Adres.Toevoeging != user.Adres.Toevoeging)
            {
                user.Adres.Toevoeging = Input.Adres.Toevoeging;
            }
            if (Input.Adres.Postcode != user.Adres.Postcode)
            {
                user.Adres.Postcode = Input.Adres.Postcode;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
