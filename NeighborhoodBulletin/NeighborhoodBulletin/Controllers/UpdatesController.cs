﻿using System;
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
    public class UpdatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UpdatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Updates
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shopOwner = _context.ShopOwners.Where(s => s.ApplicationUserId == userId).FirstOrDefault();
            UpdateIndexViewModel updateIndexViewModel = new UpdateIndexViewModel();
            var updatesBeforeDateCheck = await _context.Updates.Where(u => u.ShopOwnerId == shopOwner.Id).ToListAsync();
            var currentUpdates = CurrentUpdates(updatesBeforeDateCheck);
            var pastUpdates = PastUpdates(updatesBeforeDateCheck);
            var scheduledUpdates = ScheduledUpdates(updatesBeforeDateCheck);
            updateIndexViewModel.Updates = currentUpdates.OrderByDescending(u => u.StartDate).ToList();
            updateIndexViewModel.AllUpdates = pastUpdates.OrderByDescending(u => u.StartDate).ToList();
            updateIndexViewModel.ScheduledUpdates = scheduledUpdates.OrderByDescending(u => u.StartDate).ToList(); ;
            updateIndexViewModel.Messages = await _context.Messages.Where(m => m.ZipCode == shopOwner.ZipCode).ToListAsync();
            var messages = _context.Updates.Where(u => u.ShopOwnerId == shopOwner.Id);
            //var applicationDbContext = _context.Updates.Include(u => u.ShopOwner);
            return View(updateIndexViewModel);
        }

        public List<Update> CurrentUpdates(List<Update> updates)
        {
            var newUpdates = new List<Update>();
            foreach(var u in updates)
            {
                if(u.EndDate <= DateTime.Now || u.StartDate > DateTime.Now)
                {
                    u.Valid = false;
                }
                else
                {
                    u.Valid = true;
                }
                if(u.Valid)
                {
                    newUpdates.Add(u);
                }
            }
            return newUpdates;

        }
        public List<Update> PastUpdates(List<Update> updates)
        {
            var pastUpdates = new List<Update>();
            foreach(var u in updates)
            {
                if (u.EndDate <= DateTime.Now || u.StartDate > DateTime.Now)
                {
                    u.Valid = false;
                }
                else
                {
                    u.Valid = true;
                }
                if (!u.Valid && u.StartDate < DateTime.Now && u.EndDate < DateTime.Now)
                {

                    pastUpdates.Add(u);
                }
            }
            return pastUpdates;
        }
        public List<Update> ScheduledUpdates(List<Update> updates)
        {
            var scheduledUpdates = new List<Update>();
            foreach (var u in updates)
            {
                if (u.EndDate <= DateTime.Now || u.StartDate > DateTime.Now)
                {
                    u.Valid = false;
                }
                else
                {
                    u.Valid = true;
                }
                if (!u.Valid && u.StartDate > DateTime.Now && u.EndDate > DateTime.Now)
                {

                    scheduledUpdates.Add(u);
                }
            }
            return scheduledUpdates;
        }


        // GET: Updates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var update = await _context.Updates
                .Include(u => u.ShopOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (update == null)
            {
                return NotFound();
            }

            return View(update);
        }

        // GET: Updates/Create
        public IActionResult Create()
        {
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id");
            return View();
        }

        // POST: Updates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,ZipCode,StartDate,EndDate")] Update update)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var shopOwner = _context.ShopOwners.Where(s => s.ApplicationUserId == userId).FirstOrDefault();
                update.ShopOwnerId = shopOwner.Id;
                update.ZipCode = shopOwner.ZipCode;
                update.BusinessName = shopOwner.BusinessName;
                if (update.StartDate > update.EndDate)
                {
                    return View();
                }
                _context.Add(update);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", update.ShopOwnerId);
            return View(update);
        }

        // GET: Updates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var update = await _context.Updates.FindAsync(id);
            if (update == null)
            {
                return NotFound();
            }
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", update.ShopOwnerId);
            return View(update);
        }

        // POST: Updates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShopOwnerId,Text,SubmitButton,EditButton,DeleteButton,Like,Reply")] Update update)
        {
            if (id != update.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(update);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UpdateExists(update.Id))
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
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", update.ShopOwnerId);
            return View(update);
        }

        // GET: Updates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var update = await _context.Updates
                .Include(u => u.ShopOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (update == null)
            {
                return NotFound();
            }

            return View(update);
        }

        // POST: Updates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var update = await _context.Updates.FindAsync(id);
            _context.Updates.Remove(update);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UpdateExists(int id)
        {
            return _context.Updates.Any(e => e.Id == id);
        }
    }
}
