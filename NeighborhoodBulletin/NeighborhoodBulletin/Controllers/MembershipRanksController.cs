using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain;
using System.Security.Claims;
using NeighborhoodBulletin.Models;

namespace NeighborhoodBulletin.Controllers
{
    public class MembershipRanksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembershipRanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MembershipRanks
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
            var membershipRanks = await _context.MembershipRanks.Where(m => m.NeighborId == neighbor.Id).ToListAsync();
            var qRCode = neighbor.QRCode;
            QRCodeViewModel qRCodeViewModel = new QRCodeViewModel();
            qRCodeViewModel.Neigbor = neighbor;
            qRCodeViewModel.MembershipRanks = membershipRanks;

            return View(qRCodeViewModel);
        }

        // GET: MembershipRanks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipRank = await _context.MembershipRanks
                .Include(m => m.Neighbor)
                .Include(m => m.ShopOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (membershipRank == null)
            {
                return NotFound();
            }

            return View(membershipRank);
        }

        // GET: MembershipRanks/Create
        public IActionResult Create()
        {
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id");
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id");
            return View();
        }

        // POST: MembershipRanks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Rank,TotalSpent,Count,NeighborId,ShopOwnerId")] MembershipRank membershipRank)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipRank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", membershipRank.NeighborId);
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", membershipRank.ShopOwnerId);
            return View(membershipRank);
        }

        // GET: MembershipRanks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipRank = await _context.MembershipRanks.FindAsync(id);
            if (membershipRank == null)
            {
                return NotFound();
            }
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", membershipRank.NeighborId);
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", membershipRank.ShopOwnerId);
            return View(membershipRank);
        }

        // POST: MembershipRanks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rank,TotalSpent,Count,NeighborId,ShopOwnerId")] MembershipRank membershipRank)
        {
            if (id != membershipRank.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipRank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipRankExists(membershipRank.Id))
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
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", membershipRank.NeighborId);
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", membershipRank.ShopOwnerId);
            return View(membershipRank);
        }

        // GET: MembershipRanks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipRank = await _context.MembershipRanks
                .Include(m => m.Neighbor)
                .Include(m => m.ShopOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (membershipRank == null)
            {
                return NotFound();
            }

            return View(membershipRank);
        }

        // POST: MembershipRanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membershipRank = await _context.MembershipRanks.FindAsync(id);
            _context.MembershipRanks.Remove(membershipRank);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipRankExists(int id)
        {
            return _context.MembershipRanks.Any(e => e.Id == id);
        }
    }
}
