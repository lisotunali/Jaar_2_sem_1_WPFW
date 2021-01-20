using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC.Areas.Identity.Data;
using WDPR_MVC.Data;
using WDPR_MVC.Models;

namespace WDPR_MVC.Controllers
{
    [Authorize(Policy = "CanViewProtectedPages")]
    public class CommentsController : Controller
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _um;

        public CommentsController(MyContext context, UserManager<ApplicationUser> um)
        {
            _context = context;
            _um = um;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var myContext = _context.Comment.Include(c => c.AuteurComment).Include(c => c.Melding);
            return View(await myContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.AuteurComment)
                .Include(c => c.Melding)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["AuteurCommentId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["MeldingId"] = new SelectList(_context.Meldingen, "Id", "AuteurId");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MeldingId,AuteurCommentId,Inhoud,DatumAangemaakt,AantalLikes")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuteurCommentId"] = new SelectList(_context.Users, "Id", "Id", comment.AuteurCommentId);
            ViewData["MeldingId"] = new SelectList(_context.Meldingen, "Id", "AuteurId", comment.MeldingId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["AuteurCommentId"] = new SelectList(_context.Users, "Id", "Id", comment.AuteurCommentId);
            ViewData["MeldingId"] = new SelectList(_context.Meldingen, "Id", "AuteurId", comment.MeldingId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MeldingId,AuteurCommentId,Inhoud,DatumAangemaakt,AantalLikes")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _um.GetUserAsync(User);
                    comment.Inhoud += $"\n\n[Bewerkt door moderator {user.UserName}]";
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["AuteurCommentId"] = new SelectList(_context.Users, "Id", "Id", comment.AuteurCommentId);
            ViewData["MeldingId"] = new SelectList(_context.Meldingen, "Id", "AuteurId", comment.MeldingId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.AuteurComment)
                .Include(c => c.Melding)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
