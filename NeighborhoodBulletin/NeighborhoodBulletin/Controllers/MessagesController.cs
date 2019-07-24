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
            var hashtags = _context.Hashtags.Where(h => h.NeighborId == neighbor.Id).Select(h => h.Text);
            MessageIndexViewModel messageIndexViewModel = new MessageIndexViewModel();
            var shopOwnersSubscribedTo = new List<ShopOwner>();
            var subscriptions = await _context.Subscriptions.Where(s => s.NeighborId == neighbor.Id).ToListAsync();
            var messageIds = new List<int?>();
            var messagesToPost = new List<Message>();
            var nonlocalMessagesToPost = new List<Message>();
            var messageHashtags = new List<MessageHashtag>();
            var messagesToUse = new List<Message>();
            var messagesOutsideZipCode = new List<Message>();
            var updatesOutsideZipCode = new List<Update>();


            messageHashtags = _context.MessageHashtags.ToList();

            //List<MessageIndexViewModel> messageIndexViewModels = new List<MessageIndexViewModel>();

            var messagesInZipCode = _context.Messages.Where(m => m.ZipCode == neighbor.ZipCode);//|| m => m.Zipcode == neighbor.OtherZipCodes)


            //check if hashtags match yours
            //then post

            foreach (var m in messagesInZipCode)
            {
                //MessageIndexViewModel messageIndexViewModelForMessage = new MessageIndexViewModel();
                //messageIndexViewModelForMessage.Message = m;
                //messageIndexViewModelForMessage.Message.Hashtags = m.Hashtags;
                var hashtagsInMessage = _context.MessageHashtags.Where(mH => mH.MessageId == m.Id).Select(mH => mH.Text);
                foreach (var h in hashtags)
                {
                    if (hashtagsInMessage.Contains(h))
                    {
                        if (messagesToPost.Contains(m))
                        {
                            continue;
                        }
                        else
                        {
                            messagesToPost.Add(m);
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }

            var outsideZipCodes = _context.ZipCodes.Where(z => z.NeighborId == neighbor.Id).ToList();

            foreach (var o in outsideZipCodes)
            {
                var messagesPerZipCode = _context.Messages.Where(m => m.ZipCode == o.NonLocalZipCode).ToList();
                foreach(var m in messagesPerZipCode)
                {
                    messagesOutsideZipCode.Add(m);
                }
                var updatesPerZipCode = _context.Updates.Where(u => u.ZipCode == o.NonLocalZipCode).ToList();
                foreach(var u in updatesPerZipCode)
                {
                    updatesOutsideZipCode.Add(u);
                }

            }

            foreach (var m in messagesOutsideZipCode)
            {
                var hashtagsInMessage = _context.MessageHashtags.Where(mH => mH.MessageId == m.Id).Select(mH => mH.Text).ToList();
                foreach (var h in hashtags)
                {
                    if (hashtagsInMessage.Contains(h))
                    {
                        if (nonlocalMessagesToPost.Contains(m))
                        {
                            continue;
                        }
                        else
                        {
                            nonlocalMessagesToPost.Add(m);
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            var nonlocalUpdatesToValidate = new List<Update>();
            foreach(var u in updatesOutsideZipCode)
            {
                foreach(var s in subscriptions)
                {
                    if (u.ShopOwnerId == s.ShopOwnerId && u.ZipCode == neighbor.ZipCode)
                    {
                        nonlocalUpdatesToValidate.Add(u);
                    }
                }
            }
            var nonlocalUpdatesToPost = ValidityCheck(nonlocalUpdatesToValidate);
            //    foreach (var mH in messageHashtags)
            //    {

            //        //if (m.Id == mH.MessageId)
            //        //{
            //        //    messagesToUse.Add(m);
            //        //}
            //    }

            //    messageHashtags = _context.MessageHashtags.Where(mH => mH.MessageId == m.Id).ToList();
            //}

            //foreach (var h in hashtags)
            //{
            //    foreach (var m in messageHashtags)
            //    {
            //        if (h.Text == m.Text)
            //        {
            //            //if (messageIds.Contains(m.Id))
            //            //{
            //            //    continue;
            //            //}
            //            //else
            //            //{
            //                messageIds.Add(m.MessageId);
            //            //}
            //        } 
            //    }
            //}
            //foreach (var messageId in messageIds)
            //{
            //    var message = _context.Messages.Where(m => m.Id == messageId).FirstOrDefault();
            //    messagesToPost.Add(message);

            //}

            var updates = _context.Updates.Where(u => u.ZipCode == neighbor.ZipCode).ToList();
            var updatesToValidate = new List<Update>();
            foreach (var u in updates)
            {
                foreach (var s in subscriptions)
                {
                    if (u.ShopOwnerId == s.ShopOwnerId)
                    {
                        updatesToValidate.Add(u);
                    }
                }
            }
            var updatesToPost = ValidityCheck(updatesToValidate);
            var shopOwners = _context.ShopOwners.ToList();
            foreach (var s in subscriptions)
            {
                foreach (var sO in shopOwners)
                {
                    if (s.ShopOwnerId == sO.Id)
                    {
                        shopOwnersSubscribedTo.Add(sO);
                    }
                }
            }
            var jArray = JArray.FromObject(shopOwnersSubscribedTo);
            var latLngs = new List<Dictionary<string, double>>();

            foreach (var s in shopOwnersSubscribedTo)
            {
                var locationDictionary = new Dictionary<string,double>();
                locationDictionary.Add("lat", s.Latitude);
                locationDictionary.Add("lng", s.Longitude);
                latLngs.Add(locationDictionary);
            }
            var locationsArray = latLngs.ToArray();
            messageIndexViewModel.Neighbor = neighbor;
            messageIndexViewModel.Messages = messagesToPost.OrderByDescending(m => m.DateTime).ToList();
            messageIndexViewModel.Updates = updatesToPost.OrderByDescending(m => m.StartDate).ToList();
            messageIndexViewModel.ShopOwners = shopOwnersSubscribedTo;
            messageIndexViewModel.LatLngs = locationsArray;
            messageIndexViewModel.ShopOwnersArray = jArray;
            messageIndexViewModel.Url = url;
            messageIndexViewModel.ZipCodes = outsideZipCodes;
            messageIndexViewModel.MessagesOutsideZipCode = nonlocalMessagesToPost.OrderByDescending(m => m.DateTime).ToList();
            messageIndexViewModel.UpdatesOutsideZipCode = nonlocalUpdatesToPost.OrderByDescending(u => u.StartDate).ToList();
            //messageIndexViewModel.Location = neighborLocation;
            return View(messageIndexViewModel);
        }

        public List<Update> ValidityCheck(List<Update> updates)
        {
            var newUpdates = new List<Update>();
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
                if (u.Valid)
                {
                    newUpdates.Add(u);
                }
            }
            return newUpdates;

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
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id");
            ViewData["ZipCode"] = new SelectList(_context.ZipCodes, "Id", "Id", neighbor.ZipCode);
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,ZipCode")] Message message)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
                message.NeighborId = neighbor.Id;
                message.NeighborZipCode = neighbor.ZipCode;
                message.Username = neighbor.Username;
                message.DateTime = DateTime.Now;
                _context.Add(message);
                var nonlocalZipCodes = _context.ZipCodes.Where(z => z.NeighborId == neighbor.Id).Select(z => z.NonLocalZipCode).ToList();
                if (message.ZipCode != neighbor.ZipCode && !nonlocalZipCodes.Contains(message.ZipCode))
                {
                    var zipCode = new ZipCode();
                    zipCode.NeighborId = neighbor.Id;
                    zipCode.NonLocalZipCode = message.ZipCode;
                    _context.Add(zipCode);
                }
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
            ViewData["NeighborId"] = new SelectList(_context.Neighbors, "Id", "Id", message.NeighborId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,ZipCode")] Message message)
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
                    message.NeighborZipCode = neighbor.ZipCode;
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
