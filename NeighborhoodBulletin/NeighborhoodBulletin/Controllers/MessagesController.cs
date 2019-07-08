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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IdentityModel.Tokens.Jwt;

namespace NeighborhoodBulletin.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
         {

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
            var url = $"https://maps.googleapis.com/maps/api/js?key={APIKey.SecretKey}&callback=initMap";
            var hashtags = _context.Hashtags.Where(h => h.NeighborId == neighbor.Id);
            MessageIndexViewModel messageIndexViewModel = new MessageIndexViewModel();
            var shopOwnersSubscribedTo = new List<ShopOwner>();
            var subscriptions = await _context.Subscriptions.Where(s => s.NeighborId == neighbor.Id).ToListAsync();
            var messageIds = new List<int?>();
            var messages = _context.Messages.Where(m => m.ZipCode == neighbor.ZipCode);
            var messagesToPost = new List<Message>();
            var messageHashtags = new List<MessageHashtag>();
            foreach (var m in messages)
            {
                messageHashtags = _context.MessageHashtags.Where(mH => mH.MessageId == m.Id).ToList();
            }

            foreach (var h in hashtags)
            {
                foreach (var m in messageHashtags)
                {
                    if (h.Text == m.Text)
                    {
                        if (messageIds.Contains(m.Id))
                        {
                            continue;
                        }
                        else
                        {
                            messageIds.Add(m.Id);
                        }
                    } 
                }
            }
            foreach (var messageId in messageIds)
            {
                var message = _context.Messages.Where(m => m.Id == messageId).FirstOrDefault();
                messagesToPost.Add(message);

            }
            //foreach(var s in subscriptions)
            //{
            //    shopOwnersSubscribedTo.Add(s.ShopOwner);
            //}
            //var latLngs = new List<Dictionary<string, double>>();
            //foreach (var s in shopOwnersSubscribedTo)
            //{
            //    latLngs.Add(s.LatLng);
            //}
            //var locationsArray = latLngs.ToArray();
            messageIndexViewModel.Messages = messagesToPost;
            messageIndexViewModel.Updates = await _context.Updates.Where(u => u.ZipCode == neighbor.ZipCode).OrderByDescending(m => m.DateTime).ToListAsync();
            messageIndexViewModel.ShopOwners = shopOwnersSubscribedTo;
            messageIndexViewModel.Neighbor = neighbor;
            //messageIndexViewModel.LatLngs = locationsArray;
            messageIndexViewModel.Url = url;
            //messageIndexViewModel.Location = neighborLocation;
            return View(messageIndexViewModel);
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Neighbor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text")] Message message)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
                message.NeighborId = neighbor.Id;
                message.ZipCode = neighbor.ZipCode;
                message.Username = neighbor.Username;
                message.DateTime = DateTime.Now;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "MessageHashtags", new { newMessage = message, messageId = message.Id, });
            }
            ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id", message.Id);
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", message.NeighborId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["ZipCode"] = new SelectList(_context.Neighbors, "Id", "ZipCode", message.ZipCode);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
                    message.NeighborId = neighbor.Id;
                    message.ZipCode = neighbor.ZipCode;
                    message.Username = neighbor.Username;
                    message.DateTime = DateTime.Now;
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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

            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", message.NeighborId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Neighbor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
