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
using System.Net;
using Newtonsoft.Json;

namespace NeighborhoodBulletin.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subscriptions
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
            var shopOwners = new List<ShopOwner>();
            var nonlocalShopOwners = new List<ShopOwner>();
            var shopOwnersInZip = _context.ShopOwners.Where(s => s.ZipCode == neighbor.ZipCode);
            var zipCodeList = await _context.ZipCodes.Where(z => z.NeighborId == neighbor.Id).ToListAsync();
            var shopHashtags = _context.ShopHashtags.Where(s => s.ZipCode == neighbor.ZipCode);
            var allShopHashtags = _context.ShopHashtags;
            var shopOwnerIds = new List<int>();
            var nonlocalShopOwnerIds = new List<int?>();
            var hashtags = _context.Hashtags.Where(h => h.NeighborId == neighbor.Id);
            var subscriptions = await _context.Subscriptions.Where(s => s.NeighborId == neighbor.Id).ToListAsync();
            foreach (var h in hashtags)
            {
                //adds shopOwnerId of the shopHashtag whose Text matches one of the neighbor's Hashtags to the list of shopOwnerIds if it doesn't already exist there.
                foreach (var s in shopHashtags)
                {
                    if (h.Text == s.Text)
                    {
                        var shopOwnerId = s.ShopOwnerId;

                        if (shopOwnerIds.Contains(shopOwnerId) == true)
                        {
                            continue;
                        }
                        else
                        {
                            shopOwnerIds.Add(shopOwnerId);
                        }
                    }
                }
            }
            foreach (var shopOwnerId in shopOwnerIds)
            {
                var shopOwner = _context.ShopOwners.Where(s => s.Id == shopOwnerId).FirstOrDefault();
                if (shopOwners.Contains(shopOwner))
                {
                    continue;
                }
                else
                {
                    shopOwners.Add(shopOwner);
                }

            }
            //var subscribed = new bool();
            //foreach(var subscription in subscriptions)
            //{
            //    if (subscription.Neighbor.Id == neighbor.Id)
            //    {
            //        subscribed = true;
            //    }
            //    else
            //    {
            //        subscribed = false;
            //    }
            //}



            List<ShopOwnerSubscriptionViewModel> shopOwnerSubscriptionViewModelList = new List<ShopOwnerSubscriptionViewModel>();

            var allShopOwners = await _context.ShopOwners.ToListAsync();
            List<ShopOwner> shopOwnersInNonlocalZipCodes = new List<ShopOwner>();
            var shopOwnersList = await _context.ShopOwners.Where(s => s.ZipCode == neighbor.ZipCode).ToListAsync();
            foreach(var z in zipCodeList)
            {
                foreach(var s in allShopOwners)
                {
                    if(z.NonLocalZipCode == s.ZipCode)
                    {
                        shopOwnersInNonlocalZipCodes.Add(s);
                    }
                }

            }

            var nonlocalShopHashtags = new List<ShopHashtag>();
            foreach(var s in allShopHashtags)
            {
                foreach(var z in zipCodeList)
                {
                    if (s.ZipCode == z.NonLocalZipCode)
                    {
                        nonlocalShopHashtags.Add(s);
                    }
                }

            }

            foreach(var s in nonlocalShopHashtags)
            {
                foreach (var h in hashtags)
                {
                    if (s.Text == h.Text)
                    {
                        var nonlocalShopOwnerId = s.ShopOwnerId;
                        if(nonlocalShopOwnerIds.Contains(nonlocalShopOwnerId))
                        {
                            continue;
                        }
                        else
                        {
                            nonlocalShopOwnerIds.Add(nonlocalShopOwnerId);
                        }
                    }
                }
                
            }

            foreach(var i in nonlocalShopOwnerIds)
            {
                var nonlocalShopOwner = _context.ShopOwners.Where(s => s.Id == i).FirstOrDefault();
                if(nonlocalShopOwners.Contains(nonlocalShopOwner))
                {
                    continue;
                }
                else
                {
                    nonlocalShopOwners.Add(nonlocalShopOwner);
                }
            }

            foreach (ShopOwner shopOwner in shopOwnersList)
            {
                ShopOwnerSubscriptionViewModel shopOwnerSubscriptionViewModelForShopOwner = new ShopOwnerSubscriptionViewModel();
                shopOwnerSubscriptionViewModelForShopOwner.ShopOwner = shopOwner;
                shopOwnerSubscriptionViewModelForShopOwner.ShopOwners = shopOwners;
                shopOwnerSubscriptionViewModelForShopOwner.Neighbor = neighbor;
                shopOwnerSubscriptionViewModelForShopOwner.Subscriptions = subscriptions;
                foreach (var s in subscriptions)
                {
                    shopOwnerSubscriptionViewModelForShopOwner.Subscription = s;
                }
                shopOwnerSubscriptionViewModelForShopOwner.Subscribed = _context.Subscriptions.Where(s => s.ShopOwnerId == shopOwner.Id && s.NeighborId == neighbor.Id).Select(s => s.SubscriptionStatus).SingleOrDefault();
                shopOwnerSubscriptionViewModelForShopOwner.ShopOwnerIds = shopOwnerIds;
                shopOwnerSubscriptionViewModelList.Add(shopOwnerSubscriptionViewModelForShopOwner);
            }

            foreach (ShopOwner nonlocalShopOwner in nonlocalShopOwners)
            {
                ShopOwnerSubscriptionViewModel shopOwnerSubscriptionViewModelForNonlocalShopOwner = new ShopOwnerSubscriptionViewModel();
                shopOwnerSubscriptionViewModelForNonlocalShopOwner.NonlocalShopOwner = nonlocalShopOwner;
                shopOwnerSubscriptionViewModelForNonlocalShopOwner.NonlocalShopOwners = nonlocalShopOwners;
                shopOwnerSubscriptionViewModelForNonlocalShopOwner.Neighbor = neighbor;
                shopOwnerSubscriptionViewModelForNonlocalShopOwner.Subscriptions = subscriptions;
                foreach (var s in subscriptions)
                {
                    shopOwnerSubscriptionViewModelForNonlocalShopOwner.Subscription = s;
                }
                shopOwnerSubscriptionViewModelForNonlocalShopOwner.Subscribed = _context.Subscriptions.Where(s => s.ShopOwnerId == nonlocalShopOwner.Id && s.NeighborId == neighbor.Id).Select(s => s.SubscriptionStatus).SingleOrDefault();
                shopOwnerSubscriptionViewModelForNonlocalShopOwner.NonlocalShopOwnerIds = nonlocalShopOwnerIds;
                shopOwnerSubscriptionViewModelList.Add(shopOwnerSubscriptionViewModelForNonlocalShopOwner);
            }
            //var applicationDbContext = _context.ShopOwners.Include(s => s.ApplicationUser);  
            return View(shopOwnerSubscriptionViewModelList);

            
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Neighbor)
                .Include(s => s.ShopOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // GET: Subscriptions/Create
        public IActionResult Create(Subscription subscription, int shopOwnerId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
            subscription.NeighborId = neighbor.Id;
            subscription.Neighbor = neighbor;
            subscription.ShopOwnerId = shopOwnerId;
            subscription.ShopOwner = _context.ShopOwners.Where(s => s.Id == shopOwnerId).FirstOrDefault();
            subscription.SubscriptionStatus = true;
            var shopOwner = _context.ShopOwners.Where(s => s.Id == subscription.ShopOwnerId).FirstOrDefault(); ;
            var address = _context.ShopOwners.Where(s => s.Id == shopOwnerId).Select(s => s.Address).FirstOrDefault();
            var city = _context.ShopOwners.Where(s => s.Id == shopOwnerId).Select(s => s.City).FirstOrDefault();
            var state = _context.ShopOwners.Where(s => s.Id == shopOwnerId).Select(s => s.State).FirstOrDefault();
            var zipCode = _context.ShopOwners.Where(s => s.Id == shopOwnerId).Select(s => s.ZipCode).FirstOrDefault();      

            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id");
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id");
            return View(subscription);
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NeighborId,ShopOwnerId,SubscriptionStatus")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {

                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", subscription.NeighborId);
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", subscription.ShopOwnerId);
            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", subscription.NeighborId);
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", subscription.ShopOwnerId);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NeighborId,ShopOwnerId,SubscriptionStatus")] Subscription subscription)
        {
            if (id != subscription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.Id))
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
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", subscription.NeighborId);
            ViewData["ShopOwnerId"] = new SelectList(_context.ShopOwners, "Id", "Id", subscription.ShopOwnerId);
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Neighbor)
                .Include(s => s.ShopOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.Id == id);
        }
    }
}
