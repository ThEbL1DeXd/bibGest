using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using bibGest.Data;
using bibGest.Models;

namespace bibGest.Controllers
{
    public class PenalitesController : Controller
    {
        private readonly BibliothequeContext _context;

        public PenalitesController(BibliothequeContext context)
        {
            _context = context;
        }

        // GET: Penalites
        public async Task<IActionResult> Index()
        {
            var bibliothequeContext = _context.Penalites.Include(p => p.Emprunt);
            return View(await bibliothequeContext.ToListAsync());
        }

        // GET: Penalites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penalite = await _context.Penalites
                .Include(p => p.Emprunt)
                .FirstOrDefaultAsync(m => m.PenaliteId == id);
            if (penalite == null)
            {
                return NotFound();
            }

            return View(penalite);
        }

        // GET: Penalites/Create
        public IActionResult Create()
        {
            ViewData["EmpruntId"] = new SelectList(_context.Emprunts, "EmpruntId", "EmpruntId");
            return View();
        }

        // POST: Penalites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PenaliteId,EmpruntId,Montant,DateCreation,EstPayee")] Penalite penalite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(penalite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpruntId"] = new SelectList(_context.Emprunts, "EmpruntId", "EmpruntId", penalite.EmpruntId);
            return View(penalite);
        }

        // GET: Penalites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penalite = await _context.Penalites.FindAsync(id);
            if (penalite == null)
            {
                return NotFound();
            }
            ViewData["EmpruntId"] = new SelectList(_context.Emprunts, "EmpruntId", "EmpruntId", penalite.EmpruntId);
            return View(penalite);
        }

        // POST: Penalites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PenaliteId,EmpruntId,Montant,DateCreation,EstPayee")] Penalite penalite)
        {
            if (id != penalite.PenaliteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(penalite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PenaliteExists(penalite.PenaliteId))
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
            ViewData["EmpruntId"] = new SelectList(_context.Emprunts, "EmpruntId", "EmpruntId", penalite.EmpruntId);
            return View(penalite);
        }

        // GET: Penalites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penalite = await _context.Penalites
                .Include(p => p.Emprunt)
                .FirstOrDefaultAsync(m => m.PenaliteId == id);
            if (penalite == null)
            {
                return NotFound();
            }

            return View(penalite);
        }

        // POST: Penalites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var penalite = await _context.Penalites.FindAsync(id);
            if (penalite != null)
            {
                _context.Penalites.Remove(penalite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PenaliteExists(int id)
        {
            return _context.Penalites.Any(e => e.PenaliteId == id);
        }
    }
}
