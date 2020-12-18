using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Data;
using WDPR_MVC.Models;
using WDPR_MVC.ViewModels;

namespace WDPR_MVC.Controllers
{
    [Authorize]
    public class MeldingController : Controller
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _um;

        public MeldingController(MyContext context, UserManager<ApplicationUser> um)
        {
            _context = context;
            _um = um;
        }

        public async Task<IActionResult> Index(int page)
        {
			// Set page to 0 if page is a negative number
            if (page < 0) page = 0;

            return View(await PaginatedList<Melding>.CreateAsync(_context.Meldingen, page, 6));
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorieen = await _context.Categorieen.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titel, Beschrijving, IsAnonymous, CategorieId")] Melding melding)
        {
            ModelState.Remove("AuteurId");

            if (ModelState.IsValid)
            {
                melding.Auteur = await _um.GetUserAsync(User);
                melding.DatumAangemaakt = DateTime.Now;

                _context.Add(melding);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorieen = await _context.Categorieen.ToListAsync();
            return View(melding);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var melding = await _context.Meldingen.FirstOrDefaultAsync(m => m.Id == id);

            if (melding == null)
            {
                return NotFound();
            }

            // Makkelijkste optie voor nu
            melding.KeerBekeken += 1;
            await _context.SaveChangesAsync();

            return View(melding);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            if (!MeldingExists(id))
            {
                return NotFound();
            }

            _context.Add(new Comment { Inhoud = comment, MeldingId = id, AuteurComment = await _um.GetUserAsync(User), DatumAangemaakt = DateTime.Now });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<int>> AddLike(int id)
        {
            var melding = _context.Meldingen.Find(id);

            if (melding == null)
            {
                return NotFound();
            }

            try
            {
                var user = await _um.GetUserAsync(User);

                // Check if logged in user already has already liked this melding.
                // This is used to add or remove a like from a melding.
                if (melding.Likes.Any(m => m.MeldingId == id && m.UserId == user.Id))
                {
                    melding.Likes.Remove(melding.Likes.First(m => m.MeldingId == id && m.UserId == user.Id));
                }
                else
                {
                    melding.Likes.Add(new MeldingLike { MeldingId = id, User = user });
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeldingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return melding.Likes.Count();
        }

        private bool MeldingExists(int id)
        {
            return _context.Meldingen.Any(e => e.Id == id);
        }
    }
}
