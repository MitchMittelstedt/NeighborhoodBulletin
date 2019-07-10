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
    public class MessageHashtagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageHashtagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MessageHashtags
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MessageHashtags.Include(m => m.Message);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MessageHashtags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageHashtag = await _context.MessageHashtags
                .Include(m => m.Message)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (messageHashtag == null)
            {
                return NotFound();
            }

            return View(messageHashtag);
        }

        // GET: MessageHashtags/Create
        public IActionResult Create(Message newMessage, int messageId)
        {
            newMessage.Id = messageId;
            ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id", newMessage.Id);
            return View();
        }

        // POST: MessageHashtags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,MessageId")] MessageHashtag messageHashtag)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var neighbor = _context.Neighbors.Where(n => n.ApplicationUserId == userId).FirstOrDefault();
                var messageHashtagTexts = messageHashtag.Text.Split(",").ToList();
                var hashtags = _context.Hashtags.Where(h => h.NeighborId == neighbor.Id).ToList();
                var hashtagTexts = _context.Hashtags.Select(h => h.Text).ToList();
                var hashtagListForMessage = new List<string>();
                foreach (var m in messageHashtagTexts)
                {

                    MessageHashtag newMessageHashtag = new MessageHashtag();
                    newMessageHashtag.MessageId = messageHashtag.MessageId;

                    hashtagListForMessage.Add(m);
                    newMessageHashtag.Text = m;
                    _context.Add(newMessageHashtag);
                    await _context.SaveChangesAsync();
                    if (hashtagTexts.Contains(m))
                    {
                        continue;
                    }
                    else
                    {
                        Hashtag newHashtag = new Hashtag();
                        newHashtag.NeighborId = neighbor.Id;
                        newHashtag.Text = m;
                        _context.Add(newHashtag);
                        await _context.SaveChangesAsync();
                    }


                }
                Message message = _context.Messages.Where(m => m.Id == messageHashtag.MessageId).FirstOrDefault();
                message.Hashtags = hashtagListForMessage;
                return RedirectToAction("Index", "Messages");
            }
            ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id", messageHashtag.MessageId);
            return View(messageHashtag);
        }

        // GET: MessageHashtags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageHashtag = await _context.MessageHashtags.FindAsync(id);
            if (messageHashtag == null)
            {
                return NotFound();
            }
            ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id", messageHashtag.MessageId);
            return View(messageHashtag);
        }

        // POST: MessageHashtags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,MessageId")] MessageHashtag messageHashtag)
        {
            if (id != messageHashtag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageHashtag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageHashtagExists(messageHashtag.Id))
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
            ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id", messageHashtag.MessageId);
            return View(messageHashtag);
        }

        // GET: MessageHashtags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageHashtag = await _context.MessageHashtags
                .Include(m => m.Message)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (messageHashtag == null)
            {
                return NotFound();
            }

            return View(messageHashtag);
        }

        // POST: MessageHashtags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var messageHashtag = await _context.MessageHashtags.FindAsync(id);
            _context.MessageHashtags.Remove(messageHashtag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageHashtagExists(int id)
        {
            return _context.MessageHashtags.Any(e => e.Id == id);
        }
    }
}
