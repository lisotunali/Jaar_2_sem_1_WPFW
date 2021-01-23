using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Authorization;
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
        private readonly RoleManager<IdentityRole> _rm;
        private readonly IAuthorizationService _as;

        public MeldingController(MyContext context, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm, IAuthorizationService authS)
        {
            _context = context;
            _um = um;
            _rm = rm;
            _as = authS;
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
            var meldingen = _context.Meldingen.Where(m => !m.IsAnonymous);


            // Zoek specifieke melding op titel of beschrijving
            meldingen = Search(meldingen, search);

            // Filter meldingen
            meldingen = await Filter(meldingen, closed, likedOnly, startDate, endDate);

            // Sorteer meldingen
            meldingen = Sort(meldingen, sort, order);

            return View(await PaginatedList<Melding>.CreateAsync(meldingen, page, 10));
        }

        public IQueryable<Melding> Search(IQueryable<Melding> meldingen, string search)
        {
            if (search == null)
            {
                return meldingen;
            }

            return meldingen
                .Where(m => m.Beschrijving.Contains(search) || m.Titel.Contains(search));
        }

        // Er moet gesorteerd kunnen worden op aantal views, aantal likes, en
        // datum.
        public IQueryable<Melding> Sort(IQueryable<Melding> list, string sort, string sortOrder)
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
        public async Task<IActionResult> Create([Bind("Titel, Beschrijving, IsAnonymous, CategorieId, Image")] Melding melding)
        {
            // Wilt de model niet als valid zetten zonder dit.
            ModelState.Remove("AuteurId");

            if (ModelState.IsValid)
            {
                melding.Auteur = await _um.GetUserAsync(User);
                melding.DatumAangemaakt = DateTime.Now;

                string fileName = await UploadImageAsync(melding.Image);
                if (fileName != null) melding.ImageName = fileName;

                _context.Add(melding);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorieen = await _context.Categorieen.ToListAsync();
            return View(melding);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var currentuser = await _um.GetUserAsync(User);
            var melding = await _context.Meldingen.FindAsync(id);

            if (id == null || melding == null)
            {
                return NotFound();
            }

            var isAuthorized = await _as.AuthorizeAsync(User, melding, MeldingOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            ViewBag.Categorieen = await _context.Categorieen.ToListAsync();
            return View(melding);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Titel, Beschrijving, CategorieId, Image")] Melding melding)
        {
            if (id != melding.Id)
            {
                return NotFound();
            }

            // Wilt de model niet als valid zetten zonder dit.
            // AuteurId is required maar die sturen we niet mee met de POST
            ModelState.Remove("AuteurId");

            if (ModelState.IsValid)
            {
                var oudemelding = _context.Meldingen.Find(id);
                var isAuthorized = await _as.AuthorizeAsync(User, oudemelding, MeldingOperations.Update);

                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                BewerkteMelding nieuwemelding = new BewerkteMelding
                {
                    Titel = melding.Titel,
                    Beschrijving = melding.Beschrijving,
                    Melding = oudemelding
                };
                _context.BewerkteMeldingen.Add(nieuwemelding);

                /*
                // TODO: Is er een betere manier om dit te doen?
                var dbEntityEntry = _context.Entry(melding);
                dbEntityEntry.Property(m => m.Titel).IsModified = true;
                dbEntityEntry.Property(m => m.Beschrijving).IsModified = true;
                dbEntityEntry.Property(m => m.CategorieId).IsModified = true;

                // TODO: verwijder vorige foto?
                if (melding.Image != null)
                {
                    string fileName = await UploadImageAsync(melding.Image);
                    if (fileName != null)
                    {
                        melding.ImageName = fileName;
                        dbEntityEntry.Property(m => m.ImageName).IsModified = true;
                    }
                }

                // Dit zou eigenlijk de code hierboven moeten vervangen maar in
                // plaats van updaten verwijdert het???
                //
                // var oldMelding = _context.Meldingen.First(m => m.Id == id);
                // _context.Entry(oldMelding).CurrentValues.SetValues(melding);
                */
                try
                {
                    // _context.Update(melding);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeldingExists(melding.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorieen = await _context.Categorieen.ToListAsync();
            return View(melding);
        }

        // Writes image to disk and generates a random file name for safety
        private async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Guid should be unique enough for us.
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                string directory = @"wwwroot/images";

                // If folder doesn't exist create it.
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), directory, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return fileName;
            }

            return null;
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

            if (melding.IsAnonymous)
            {
                var isAuthorized = await _as.AuthorizeAsync(User, melding, MeldingOperations.ReadAnonymous);

                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
            }

            // Makkelijkste optie voor nu
            melding.KeerBekeken += 1;
            await _context.SaveChangesAsync();

            return View(melding);
        }

        // TODO: Show button only when logged in as someone with permission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            if (!MeldingExists(id))
            {
                return NotFound();
            }

            var melding = await _context.Meldingen.FirstOrDefaultAsync(m => m.Id == id);

            if (melding == null)
            {
                return NotFound();
            }

            // Only add comment if melding is not closed
            if (!melding.IsClosed)
            {
                _context.Add(new Comment { Inhoud = comment, MeldingId = id, AuteurComment = await _um.GetUserAsync(User), DatumAangemaakt = DateTime.Now });
                melding.KeerBekeken -= 1;
                await _context.SaveChangesAsync();
            }
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
        public async Task<IActionResult> ToggleSluitMelding(int id)
        {
            try
            {
                var melding = _context.Meldingen.Find(id);
                var isAuthorized = await _as.AuthorizeAsync(User, melding, MeldingOperations.Lock);

                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                // false = true
                // true = false
                //
                // ;-)
                melding.IsClosed = !melding.IsClosed;

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

            return RedirectToAction(nameof(Details), new { id = id });
        }

        public async Task<IActionResult> Report(int id)
        {

            if (!MeldingExists(id))
            {
                return NotFound();
            }

            var melding = _context.Meldingen.Find(id);
            var userId = _um.GetUserId(User);

            // Check if already exists
            if (melding.Reports.Any(m => m.AuteurReportId == userId)) {
                return Ok("U heeft deze melding reeds succesvol gerapporteerd! Bedankt voor uw geduld. :)");
            }

            // If not add..
            melding.Reports.Add(new Report { Melding = melding, AuteurReportId = userId});
            await _context.GerapporteerdeMeldingen.AddAsync(new GerapporteerdeMelding { Melding = melding });
            await _context.SaveChangesAsync();

            return Ok("De melding is gerapporteerd aan het moderatorteam en zij zullen dit in behandeling nemen.");
        }

        private bool MeldingExists(int id)
        {
            return _context.Meldingen.Any(e => e.Id == id);
        }
    }
}
