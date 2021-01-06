using System;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<IActionResult> Index(
            int page,
            string search,
            string sort,
            string order,
            bool closed,
            bool likedOnly,
            DateTime startDate,
            DateTime endDate)
        {
            // Set page to 0 if page is a negative number
            if (page < 0) page = 0;

            // De meldingen. Natuurlijk stuur je nooit anonymous meldingen mee.
            var meldingen = _context.Meldingen.Where(m => m.IsAnonymous == false);

            // Zoek specifieke melding op titel of beschrijving
            meldingen = Search(meldingen, search);

            // Filter meldingen
            meldingen = await Filter(meldingen, closed, likedOnly, startDate, endDate);

            // Sorteer meldingen
            meldingen = Sort(meldingen, sort, order);

            return View(await PaginatedList<Melding>.CreateAsync(meldingen, page, 10));
        }

        IQueryable<Melding> Search(IQueryable<Melding> meldingen, string search)
        {
            Console.WriteLine(search);
            if (search == null)
            {
                return _context.Meldingen;
            }

            return meldingen
                .Where(m => m.Beschrijving.Contains(search) || m.Titel.Contains(search));
        }


        // Er moet gesorteerd kunnen worden op aantal views, aantal likes, en
        // datum.
        IQueryable<Melding> Sort(IQueryable<Melding> list, string sort, string sortOrder)
        {
            return sort?.ToLower() switch
            {
                "views" => SortThisPls(list, m => m.KeerBekeken, sortOrder),
                "likes" => SortThisPls(list, m => m.Likes.Count(), sortOrder),
                "date" => SortThisPls(list, m => m.DatumAangemaakt, sortOrder),
                _ => SortThisPls(list, m => m.Id, sortOrder)

            };
        }

        // Generic method to sort IQueryable things easier.
        // Defaults to DESC if sortOrder is not ASC.
        IQueryable<T> SortThisPls<T, V>(
            IQueryable<T> list,
            Expression<Func<T, V>> selector,
            String sortOrder)
        {
            if (sortOrder?.ToLower() == "asc")
            {
                return list.OrderBy(selector);
            }

            return list.OrderByDescending(selector);
        }

        async Task<IQueryable<Melding>> Filter(
            IQueryable<Melding> meldingen,
            bool closed,
            bool likedOnly,
            DateTime startDate,
            DateTime endDate)
        {
            // Standaard worden alle open meldingen weergegeven, maar de
            // gebruiker kan er ook voor kiezen gesloten meldingen ook
            // zichtbaar te maken.
            if (!closed)
            {
                meldingen = meldingen.Where(m => !m.IsClosed);
            }

            // De gebruiker kan er ook voor kiezen te filteren op meldingen
            // waarop de gebruiker heeft geliket.
            if (likedOnly)
            {
                var user = await _um.GetUserAsync(User);
                meldingen = meldingen.Where(m => m.Likes.Any(m => m.UserId == user.Id));
            }

            // De gebruiker kan een datum rangeaangeven waarbinnen de meldingen
            // gepost moeten zijn.
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                meldingen = meldingen
                    .Where(m => m.DatumAangemaakt >= startDate)
                    .Where(m => m.DatumAangemaakt <= endDate);
            }

            return meldingen;
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

        // TODO: Add check if melding is anonymous
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
