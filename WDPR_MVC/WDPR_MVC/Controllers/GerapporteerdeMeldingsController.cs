using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC.Authorization;
using WDPR_MVC.Data;
using WDPR_MVC.Models;

namespace WDPR_MVC.Controllers
{
    [Authorize(Policy = "CanViewProtectedPages")]
    public class GerapporteerdeMeldingsController : Controller
    {
        private readonly MyContext _context;
        private readonly IAuthorizationService _as;


        public GerapporteerdeMeldingsController(MyContext context, IAuthorizationService aservice)
        {
            _context = context;
            _as = aservice;
        }

        // GET: GerapporteerdeMeldings
        public async Task<IActionResult> Index()
        {
            var myContext = _context.GerapporteerdeMeldingen.Include(g => g.Melding);
            return View(await myContext.ToListAsync());
        }

        // GET: GerapporteerdeMeldings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gerapporteerdeMelding = await _context.GerapporteerdeMeldingen
                .Include(g => g.Melding)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gerapporteerdeMelding == null)
            {
                return NotFound();
            }

            return View(gerapporteerdeMelding);
        }


       

        // GET: GerapporteerdeMeldings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gerapporteerdeMelding = await _context.GerapporteerdeMeldingen
                .Include(g => g.Melding)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gerapporteerdeMelding == null)
            {
                return NotFound();
            }

            return View(gerapporteerdeMelding);
        }

        // POST: GerapporteerdeMeldings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gerapporteerdeMelding = await _context.GerapporteerdeMeldingen.FindAsync(id);
            _context.GerapporteerdeMeldingen.Remove(gerapporteerdeMelding);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sluit(int id)
        {
            try
            {
                var melding = _context.GerapporteerdeMeldingen.Find(id).Melding;
                var isAuthorized = await _as.AuthorizeAsync(User, melding, MeldingOperations.Lock);


                melding.IsClosed = true;
                _context.GerapporteerdeMeldingen.Remove(_context.GerapporteerdeMeldingen.Find(id));

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GerapporteerdeMeldingExists(id))
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

        private bool GerapporteerdeMeldingExists(int id)
        {
            return _context.GerapporteerdeMeldingen.Any(e => e.Id == id);
        }
    }
}
