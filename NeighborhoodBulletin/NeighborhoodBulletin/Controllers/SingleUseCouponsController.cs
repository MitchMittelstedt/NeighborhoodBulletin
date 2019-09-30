using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain;
using System.Security.Claims;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;
using Leadtools.Codecs;
using Leadtools.Barcode;
using Leadtools;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace NeighborhoodBulletin.Controllers
{
    public class SingleUseCouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IHostingEnvironment _host;

        public SingleUseCouponsController(ApplicationDbContext context, IHostingEnvironment host)
        {
            _context = context;
            _host = host;
            barcodeEngineInstance = new BarcodeEngine();
            // Requires a license file that unlocks 1D barcode read functionality. 
            string MY_LICENSE_FILE = @"C:\LEADTOOLS 20\Common\License\leadtools.lic";
            string MY_DEVELOPER_KEY = System.IO.File.ReadAllText(@"C:\LEADTOOLS 20\Common\License\leadtools.lic.key");
            RasterSupport.SetLicense(MY_LICENSE_FILE, MY_DEVELOPER_KEY);


        }

        private BarcodeEngine barcodeEngineInstance;
        private RasterImage theImage;
        private string imageFileName;

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
        public async Task<IActionResult> Create(IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var shopOwner = _context.ShopOwners.Where(s => s.ApplicationUserId == userId).FirstOrDefault();
                var filename = shopOwner.Id + Path.GetFileName(file.FileName);
                string contentrootpath = _host.ContentRootPath + "/App_Data/uploads";
                var path = Path.Combine(contentrootpath, filename);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                var neighborIds = _context.Neighbors.Select(n => n.Id).ToList();

                using (RasterCodecs codecs = new RasterCodecs())
                {
                    RasterImage newImage = codecs.Load(filename, 0, CodecsLoadByteOrder.BgrOrGray, 1, 1);

                    //Dispose of the image
                    if (newImage != null)
                    {
                        theImage.Dispose();
                    }

                    theImage = newImage;
                    imageFileName = filename;
                }

                BarcodeData[] dataArray = barcodeEngineInstance.Reader.ReadBarcodes(theImage, LeadRect.Empty, 0, null);

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} barcode(s) found", dataArray.Length);
                sb.AppendLine();

                for (int i = 0; i < dataArray.Length; i++)
                {
                    BarcodeData data = dataArray[i];

                    sb.AppendFormat("Symbology: {0}, Location: {1}, Data: {2}", data.Symbology.ToString(), data.Bounds.ToString(), data.Value);
                    sb.AppendLine();

                    if(neighborIds.Contains(sb[2]))
                    {
                        var neighborId = sb[2];
                        var subscription = _context.Subscriptions.Where(s => s.NeighborId == neighborId).FirstOrDefault();
                        subscription.UsageCount++;
                        //subscription.TotalSpent += 
                    }
                }
                //if (neighborIds.Contains(singleUseCoupon.Value))
                //{
                //    var neighborId = singleUseCoupon.Value;
                //    var subscription = _context.Subscriptions.Where(s => s.NeighborId == neighborId).FirstOrDefault();
                //    subscription.UsageCount++;
                //    subscription.TotalSpent += singleUseCoupon.LastSpent;
                //}
                //else
                //{
                //    throw new Exception();
                //}
                //var singleUseCouponValues = _context.SingleUseCoupons.Select(s => s.Value).ToList();
                //if (singleUseCouponValues.Contains(singleUseCoupon.Value))
                //{
                //    throw new Exception();
                //}
                //else
                //{
                //    _context.Add(singleUseCoupon);
                //    await _context.SaveChangesAsync();
                //}
                //return RedirectToAction(nameof(Index));
            }
            return View();
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
