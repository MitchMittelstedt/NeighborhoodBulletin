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
using NeighborhoodBulletin.Models;

namespace NeighborhoodBulletin.Controllers
{
    //[Authorize(Roles = "ShopOwner")]
    public class ShopOwnersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopOwnersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShopOwners
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
            var shopOwners = _context.ShopOwners.Where(s => s.ZipCode == neighbor.ZipCode);
            var applicationDbContext = _context.ShopOwners.Include(s => s.ApplicationUser);
            ShopOwnerSubscriptionViewModel shopOwnerSubscriptionViewModel = new ShopOwnerSubscriptionViewModel();
            shopOwnerSubscriptionViewModel.ShopOwners = await shopOwners.ToListAsync();
            shopOwnerSubscriptionViewModel.Neighbor = neighbor;
            shopOwnerSubscriptionViewModel.Hashtags = await _context.Hashtags.Where(h => h.NeighborId == neighbor.Id).ToListAsync();
            return View(shopOwnerSubscriptionViewModel);
        }

        // GET: ShopOwners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopOwner = await _context.ShopOwners
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopOwner == null)
            {
                return NotFound();
            }

            return View(shopOwner);
        }

        // GET: ShopOwners/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: ShopOwners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Address,City,State,ZipCode,BusinessName,ApplicationUserId")] ShopOwner shopOwner)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                shopOwner.ApplicationUserId = userId;
                _context.Add(shopOwner);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "ShopHashtags");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", shopOwner.ApplicationUserId);
            return View(shopOwner);
        }

        // GET: ShopOwners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopOwner = await _context.ShopOwners.FindAsync(id);
            if (shopOwner == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", shopOwner.ApplicationUserId);
            return View(shopOwner);
        }

        // POST: ShopOwners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ZipCode,BusinessName,ApplicationUserId")] ShopOwner shopOwner)
        {
            if (id != shopOwner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shopOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShopOwnerExists(shopOwner.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", shopOwner.ApplicationUserId);
            return View(shopOwner);
        }

        // GET: ShopOwners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopOwner = await _context.ShopOwners
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopOwner == null)
            {
                return NotFound();
            }

            return View(shopOwner);
        }

        // POST: ShopOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shopOwner = await _context.ShopOwners.FindAsync(id);
            _context.ShopOwners.Remove(shopOwner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShopOwnerExists(int id)
        {
            return _context.ShopOwners.Any(e => e.Id == id);
        }
    }
}
