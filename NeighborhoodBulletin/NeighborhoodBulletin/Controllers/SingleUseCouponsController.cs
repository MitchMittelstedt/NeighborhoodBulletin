using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace NeighborhoodBulletin.Controllers
{
    public class SingleUseCouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SingleUseCouponsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SingleUseCoupons
        public async Task<IActionResult> Index()
        {
            return View(await _context.SingleUseCoupons.ToListAsync());
        }

        // GET: SingleUseCoupons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singleUseCoupon = await _context.SingleUseCoupons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (singleUseCoupon == null)
            {
                return NotFound();
            }

            return View(singleUseCoupon);
        }

        // GET: SingleUseCoupons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SingleUseCoupons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Value")] SingleUseCoupon singleUseCoupon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(singleUseCoupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(singleUseCoupon);
        }

        // GET: SingleUseCoupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singleUseCoupon = await _context.SingleUseCoupons.FindAsync(id);
            if (singleUseCoupon == null)
            {
                return NotFound();
            }
            return View(singleUseCoupon);
        }

        // POST: SingleUseCoupons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Value")] SingleUseCoupon singleUseCoupon)
        {
            if (id != singleUseCoupon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(singleUseCoupon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SingleUseCouponExists(singleUseCoupon.Id))
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
            return View(singleUseCoupon);
        }

        // GET: SingleUseCoupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singleUseCoupon = await _context.SingleUseCoupons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (singleUseCoupon == null)
            {
                return NotFound();
            }

            return View(singleUseCoupon);
        }

        // POST: SingleUseCoupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var singleUseCoupon = await _context.SingleUseCoupons.FindAsync(id);
            _context.SingleUseCoupons.Remove(singleUseCoupon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SingleUseCouponExists(int id)
        {
            return _context.SingleUseCoupons.Any(e => e.Id == id);
        }
    }
}
