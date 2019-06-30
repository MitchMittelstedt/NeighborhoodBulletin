using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain;
using System.Security.Claims;

namespace NeighborhoodBulletin.Controllers
{
    public class ShopHashtagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopHashtagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShopHashtags
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ShopHashtags.Include(s => s.ShopOwner);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ShopHashtags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopHashtag = await _context.ShopHashtags
                .Include(s => s.ShopOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopHashtag == null)
            {
                return NotFound();
            }

            return View(shopHashtag);
        }

        // GET: ShopHashtags/Create
        public IActionResult Create()
        {
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id");
            return View();
        }

        // POST: ShopHashtags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,ShopOwnerId")] ShopHashtag shopHashtag)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var shopOwner = _context.ShopOwners.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
                var shopHashtagTexts = shopHashtag.Text.Split(",").ToList();
                foreach(var s in shopHashtagTexts)
                {
                    ShopHashtag newShopHashtag = new ShopHashtag();
                    newShopHashtag.ShopOwnerId = shopOwner.Id;
                    newShopHashtag.Text = s;
                    _context.Add(newShopHashtag);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Home");
            }
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", shopHashtag.ShopOwnerId);
            return View(shopHashtag);
        }

        // GET: ShopHashtags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopHashtag = await _context.ShopHashtags.FindAsync(id);
            if (shopHashtag == null)
            {
                return NotFound();
            }
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", shopHashtag.ShopOwnerId);
            return View(shopHashtag);
        }

        // POST: ShopHashtags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,ShopOwnerId")] ShopHashtag shopHashtag)
        {
            if (id != shopHashtag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shopHashtag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShopHashtagExists(shopHashtag.Id))
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
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", shopHashtag.ShopOwnerId);
            return View(shopHashtag);
        }

        // GET: ShopHashtags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopHashtag = await _context.ShopHashtags
                .Include(s => s.ShopOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopHashtag == null)
            {
                return NotFound();
            }

            return View(shopHashtag);
        }

        // POST: ShopHashtags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shopHashtag = await _context.ShopHashtags.FindAsync(id);
            _context.ShopHashtags.Remove(shopHashtag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShopHashtagExists(int id)
        {
            return _context.ShopHashtags.Any(e => e.Id == id);
        }
    }
}
