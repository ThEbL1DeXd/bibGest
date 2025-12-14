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
    public class EmpruntsController : Controller
    {
        private readonly BibliothequeContext _context;

        public EmpruntsController(BibliothequeContext context)
        {
            _context = context;
        }

        // GET: Emprunts
        public async Task<IActionResult> Index()
        {
            var bibliothequeContext = _context.Emprunts.Include(e => e.Livre).Include(e => e.Utilisateur);
            return View(await bibliothequeContext.ToListAsync());
        }

        // GET: Emprunts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(m => m.EmpruntId == id);
            if (emprunt == null)
            {
                return NotFound();
            }

            return View(emprunt);
        }

        // GET: Emprunts/Create
        public IActionResult Create()
        {
            ViewData["LivreId"] = new SelectList(_context.Livres, "LivreId", "LivreId");
            ViewData["UtilisateurId"] = new SelectList(_context.Utilisateurs, "UtilisateurId", "UtilisateurId");
            return View();
        }

        // POST: Emprunts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpruntId,LivreId,UtilisateurId,DateEmprunt,DateRetourPrevue,DateRetourReelle,Statut")] Emprunt emprunt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emprunt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LivreId"] = new SelectList(_context.Livres, "LivreId", "LivreId", emprunt.LivreId);
            ViewData["UtilisateurId"] = new SelectList(_context.Utilisateurs, "UtilisateurId", "UtilisateurId", emprunt.UtilisateurId);
            return View(emprunt);
        }

        // GET: Emprunts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt == null)
            {
                return NotFound();
            }
            ViewData["LivreId"] = new SelectList(_context.Livres, "LivreId", "LivreId", emprunt.LivreId);
            ViewData["UtilisateurId"] = new SelectList(_context.Utilisateurs, "UtilisateurId", "UtilisateurId", emprunt.UtilisateurId);
            return View(emprunt);
        }

        // POST: Emprunts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmpruntId,LivreId,UtilisateurId,DateEmprunt,DateRetourPrevue,DateRetourReelle,Statut")] Emprunt emprunt)
        {
            if (id != emprunt.EmpruntId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emprunt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpruntExists(emprunt.EmpruntId))
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
            ViewData["LivreId"] = new SelectList(_context.Livres, "LivreId", "LivreId", emprunt.LivreId);
            ViewData["UtilisateurId"] = new SelectList(_context.Utilisateurs, "UtilisateurId", "UtilisateurId", emprunt.UtilisateurId);
            return View(emprunt);
        }

        // GET: Emprunts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(m => m.EmpruntId == id);
            if (emprunt == null)
            {
                return NotFound();
            }

            return View(emprunt);
        }

        // POST: Emprunts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt != null)
            {
                _context.Emprunts.Remove(emprunt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpruntExists(int id)
        {
            return _context.Emprunts.Any(e => e.EmpruntId == id);
        }
    }
}
