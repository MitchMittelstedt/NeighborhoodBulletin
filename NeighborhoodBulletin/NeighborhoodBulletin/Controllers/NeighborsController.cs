using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace NeighborhoodBulletin.Controllers
{
    //[Authorize(Roles = "Neighbor")]
    public class NeighborsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NeighborsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Neighbors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Neighbors.Include(n => n.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Neighbors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var neighbor = await _context.Neighbors
                .Include(n => n.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (neighbor == null)
            {
                return NotFound();
            }

            return View(neighbor);
        }

        // GET: Neighbors/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Neighbors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ZipCode,Username,ApplicationUserId")] Neighbor neighbor)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                neighbor.ApplicationUserId = userId;
                _context.Add(neighbor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "ShopOwners");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", neighbor.ApplicationUserId);
            return View(neighbor);
        }

        // GET: Neighbors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var neighbor = await _context.Neighbors.FindAsync(id);
            if (neighbor == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", neighbor.ApplicationUserId);
            return View(neighbor);
        }

        // POST: Neighbors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ZipCode,Username,ApplicationUserId")] Neighbor neighbor)
        {
            if (id != neighbor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(neighbor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NeighborExists(neighbor.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", neighbor.ApplicationUserId);
            return View(neighbor);
        }

        // GET: Neighbors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var neighbor = await _context.Neighbors
                .Include(n => n.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (neighbor == null)
            {
                return NotFound();
            }

            return View(neighbor);
        }

        // POST: Neighbors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var neighbor = await _context.Neighbors.FindAsync(id);
            _context.Neighbors.Remove(neighbor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NeighborExists(int id)
        {
            return _context.Neighbors.Any(e => e.Id == id);
        }
    }
}
