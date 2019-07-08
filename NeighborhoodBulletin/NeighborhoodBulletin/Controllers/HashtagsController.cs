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
    public class HashtagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HashtagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hashtags
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Hashtags.Include(h => h.Neighbor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Hashtags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hashtag = await _context.Hashtags
                .Include(h => h.Neighbor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hashtag == null)
            {
                return NotFound();
            }

            return View(hashtag);
        }

        // GET: Hashtags/Create
        public IActionResult Create()
        {
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id");
            return View();
        }

        // POST: Hashtags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,NeighborId")] Hashtag hashtag)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
                var hashtagTexts = hashtag.Text.Split(",").ToList();
                foreach (var h in hashtagTexts)
                {
                    Hashtag newHashtag = new Hashtag();
                    newHashtag.NeighborId = neighbor.Id;
                    newHashtag.Text = h;
                    _context.Add(newHashtag);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Subscriptions");
            }
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", hashtag.NeighborId);
            return View(hashtag);
        }

        // GET: Hashtags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hashtag = await _context.Hashtags.FindAsync(id);
            if (hashtag == null)
            {
                return NotFound();
            }
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", hashtag.NeighborId);
            return View(hashtag);
        }

        // POST: Hashtags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,NeighborId")] Hashtag hashtag)
        {
            if (id != hashtag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hashtag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HashtagExists(hashtag.Id))
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
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", hashtag.NeighborId);
            return View(hashtag);
        }

        // GET: Hashtags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hashtag = await _context.Hashtags
                .Include(h => h.Neighbor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hashtag == null)
            {
                return NotFound();
            }

            return View(hashtag);
        }

        // POST: Hashtags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hashtag = await _context.Hashtags.FindAsync(id);
            _context.Hashtags.Remove(hashtag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HashtagExists(int id)
        {
            return _context.Hashtags.Any(e => e.Id == id);
        }
    }
}
