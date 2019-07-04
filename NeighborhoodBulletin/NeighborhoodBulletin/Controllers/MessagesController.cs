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
            //var jsonObject = new WebClient().DownloadString(url);
            //javascript 
            //Location neighborLocation = new Location();


            //dynamic jObject = JObject.Parse(jsonObject);
            //var lat = jObject.results[0].geometry.location.lat;
            //var lng = jObject.results[0].geometry.location.lng;
            //neighborLocation.lat = lat;
            //neighborLocation.lng = lng;
            //neighborLocation = JsonConvert.DeserializeObject<Location>(jsonObject); in 
            //var lat = location["lat"];
            //var lng = location["lng"];
            //var stuff = _download_serialized_json_data(url);
            MessageIndexViewModel messageIndexViewModel = new MessageIndexViewModel();
            messageIndexViewModel.Messages = await _context.Messages.Where(m => m.ZipCode == neighbor.ZipCode).OrderByDescending(m => m.DateTime).ToListAsync();
            messageIndexViewModel.Updates = await _context.Updates.Where(u => u.ZipCode == neighbor.ZipCode).OrderByDescending(m => m.DateTime).ToListAsync();
            messageIndexViewModel.Neighbor = neighbor;
            messageIndexViewModel.Url = url;
            //messageIndexViewModel.Location = neighborLocation;
            //make zipcode connected to the center of the google map that pops up when the neighbor logs in. get latlong of zipcode.


            //var message = _context.Messages.WHere(m => m.Neighbor.ZipCode == neighbor.ZipCode);
            //var message = _context.Messages.Where(m => m.ZipCode == neighbor.ZipCode); 
            //var applicationDbContext = _context.Messages.Include(m => m.Neighbor);
            return View(messageIndexViewModel);
        }


        private static T _download_serialized_json_data<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }

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
                return RedirectToAction(nameof(Index));
            }
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
