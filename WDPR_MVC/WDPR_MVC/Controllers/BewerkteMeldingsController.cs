using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC.Data;
using WDPR_MVC.Models;

namespace WDPR_MVC.Controllers
{
    public class BewerkteMeldingsController : Controller
    {
        private readonly MyContext _context;

        public BewerkteMeldingsController(MyContext context)
        {
            _context = context;
        }

        // GET: BewerkteMeldings
        public async Task<IActionResult> Index()
        {
            var myContext = _context.BewerkteMeldingen.Include(b => b.Melding);
            return View(await myContext.ToListAsync());
        }

        // GET: BewerkteMeldings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bewerkteMelding = await _context.BewerkteMeldingen
                .Include(b => b.Melding)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bewerkteMelding == null)
            {
                return NotFound();
            }

            return View(bewerkteMelding);
        }

        // GET: BewerkteMeldings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bewerkteMelding = await _context.BewerkteMeldingen.FindAsync(id);
            if (bewerkteMelding == null)
            {
                return NotFound();
            }
            ViewData["MeldingId"] = new SelectList(_context.Meldingen, "Id", "AuteurId", bewerkteMelding.MeldingId);
            return View(bewerkteMelding);
        }

        // POST: BewerkteMeldings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MeldingId,Titel,Beschrijving")] BewerkteMelding bewerkteMelding)
        {
            if (id != bewerkteMelding.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bewerkteMelding);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BewerkteMeldingExists(bewerkteMelding.Id))
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
            ViewData["MeldingId"] = new SelectList(_context.Meldingen, "Id", "AuteurId", bewerkteMelding.MeldingId);
            return View(bewerkteMelding);
        }

        // GET: BewerkteMeldings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bewerkteMelding = await _context.BewerkteMeldingen
                .Include(b => b.Melding)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bewerkteMelding == null)
            {
                return NotFound();
            }

            return View(bewerkteMelding);
        }

        // POST: BewerkteMeldings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bewerkteMelding = await _context.BewerkteMeldingen.FindAsync(id);
            _context.BewerkteMeldingen.Remove(bewerkteMelding);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: BewerkteMeldings/Goedkeuren/5
        public async Task<IActionResult> Goedkeuren(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bewerkteMelding = await _context.BewerkteMeldingen
                .Include(b => b.Melding)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bewerkteMelding == null)
            {
                return NotFound();
            }

            return View(bewerkteMelding);
        }
        // POST: BewerkteMeldings/Delete/5
        [HttpPost, ActionName("Goedkeuren")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Goedkeuren(int id) {
            // Get current bewerkte dingens
            var bewerkt = await _context.BewerkteMeldingen.FindAsync(id);
            Melding oud = await _context.Meldingen.FindAsync(bewerkt.Melding.Id);

            // The real update
            oud.Titel = bewerkt.Titel;
            oud.Beschrijving = bewerkt.Beschrijving;

            await _context.SaveChangesAsync();

            // Ja dit kunnen we wel doen
            return await DeleteConfirmed(id);
        }

        private bool BewerkteMeldingExists(int id)
        {
            return _context.BewerkteMeldingen.Any(e => e.Id == id);
        }
    }
}
