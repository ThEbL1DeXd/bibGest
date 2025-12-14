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
            ViewData["LivreId"] = new SelectList(_context.Livres, "LivreId", "Titre");
            ViewData["UtilisateurId"] = new SelectList(
                _context.Utilisateurs
                    .Select(u => new { u.UtilisateurId, NomComplet = u.Nom + " " + u.Prenom })
                    .ToList(), 
                "UtilisateurId", 
                "NomComplet");
            var emprunt = new Emprunt
            {
                DateEmprunt = DateTime.Now,
                DateRetourPrevue = DateTime.Now.AddDays(14),
                Statut = 0
            };
            return View(emprunt);
        }

        // POST: Emprunts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LivreId,UtilisateurId,DateRetourPrevue,DateRetourReelle,Statut")] Emprunt emprunt)
        {
            // Remove validation errors for navigation properties
            ModelState.Remove("Livre");
            ModelState.Remove("Utilisateur");
            ModelState.Remove("Penalites");
            
            // Debug: Check what values we received
            System.Diagnostics.Debug.WriteLine($"LivreId: {emprunt.LivreId}");
            System.Diagnostics.Debug.WriteLine($"UtilisateurId: {emprunt.UtilisateurId}");
            System.Diagnostics.Debug.WriteLine($"DateRetourPrevue: {emprunt.DateRetourPrevue}");
            System.Diagnostics.Debug.WriteLine($"Statut: {emprunt.Statut}");
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
            }
            
            if (ModelState.IsValid)
            {
                emprunt.DateEmprunt = DateTime.Now;
                _context.Add(emprunt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // If we get here, something went wrong - repopulate dropdowns
            ViewData["LivreId"] = new SelectList(_context.Livres, "LivreId", "Titre", emprunt.LivreId);
            ViewData["UtilisateurId"] = new SelectList(
                _context.Utilisateurs
                    .Select(u => new { u.UtilisateurId, NomComplet = u.Nom + " " + u.Prenom })
                    .ToList(), 
                "UtilisateurId", 
                "NomComplet", 
                emprunt.UtilisateurId);
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
            ViewData["LivreId"] = new SelectList(_context.Livres, "LivreId", "Titre", emprunt.LivreId);
            ViewData["UtilisateurId"] = new SelectList(
                _context.Utilisateurs
                    .Select(u => new { u.UtilisateurId, NomComplet = u.Nom + " " + u.Prenom })
                    .ToList(), 
                "UtilisateurId", 
                "NomComplet", 
                emprunt.UtilisateurId);
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

            // Remove validation errors for navigation properties
            ModelState.Remove("Livre");
            ModelState.Remove("Utilisateur");
            ModelState.Remove("Penalites");

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
            ViewData["LivreId"] = new SelectList(_context.Livres, "LivreId", "Titre", emprunt.LivreId);
            ViewData["UtilisateurId"] = new SelectList(
                _context.Utilisateurs
                    .Select(u => new { u.UtilisateurId, NomComplet = u.Nom + " " + u.Prenom })
                    .ToList(), 
                "UtilisateurId", 
                "NomComplet", 
                emprunt.UtilisateurId);
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
