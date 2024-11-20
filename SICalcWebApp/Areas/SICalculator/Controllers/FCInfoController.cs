using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Areas.SICalculator.VM;
using SICalcWebApp.Data;
using SICalcWebApp.Repository;

namespace SICalcWebApp.Areas.SICalculator.Controllers
{
    [Area("SICalculator")]
    [Authorize(Roles = $"{SD.Role_Sponge_Admin},{SD.Role_Super_Admin}")]
    public class FCInfoController : Controller
    {
        private readonly IFCInfoService _fCInfoService;
        private readonly ApplicationDbContext _context;

        public FCInfoController(IFCInfoService fCInfoService, ApplicationDbContext context)
        {
            _fCInfoService = fCInfoService;
            _context = context;
        }

        // GET: FCInfo
        public async Task<IActionResult> FCInfoList()
        {
            return View(await _fCInfoService.GetAllFCInfosAsync());
        }

        // GET: FCInfo/Create
        public async Task<IActionResult> FCInfoCreate()
        {
            // Retrieve all FCs
            var fcs = await _fCInfoService.GetAllFCsAsync();

            // Retrieve all TPDs
            var tpds = await _fCInfoService.GetAllTPDInfosAsync();

            // Retrieve existing FCInfo entries
            var existingFCInfos = await _fCInfoService.GetAllFCInfosAsync();

            // Filter TPDs where not all FCs have feed rate entries
            var availableTPDs = tpds.Where(tpd =>
                fcs.Any(fc => !existingFCInfos.Any(info => info.FCId == fc.FCId && info.TPDId == tpd.TPDId))
            ).ToList();

            // Prepare ViewBag items
            ViewBag.FCId = new SelectList(fcs, "FCId", "FCValue");
            ViewBag.TPDId = new SelectList(availableTPDs, "TPDId", "KilnName");

            return View();


        }

        // POST: FCInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FCInfoCreate([Bind("FCId,TPDId,FeedRate")] FCInfo fCInfo)
        {
            Console.WriteLine($"FCId: {fCInfo.FCId}, TPDId: {fCInfo.TPDId}, FeedRate: {fCInfo.FeedRate}");

            if (ModelState.IsValid)
            {
                await _fCInfoService.AddFCInfoAsync(fCInfo);
                return RedirectToAction(nameof(FCInfoList));
            }


            foreach (var state in ModelState)
            {
                Console.WriteLine($"Key: {state.Key}");
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            var fcs = _fCInfoService.GetAllFCs();
            var tpds = _fCInfoService.GetAllTPDInfos();

            ViewBag.FCId = new SelectList(fcs, "FCId", "FCValue");
            ViewBag.TPDId = new SelectList(tpds, "TPDId", "TPD");

            return View(fCInfo);
        }
        [HttpPost]
        public async Task<IActionResult> GetFCsByTPD(int tpdId)
        {
            // Retrieve all FCs and TPDs
            var fcs = await _fCInfoService.GetAllFCsAsync();

            // Retrieve existing FCInfo entries
            var existingFCInfos = await _fCInfoService.GetAllFCInfosAsync();

            // Filter FCs based on the selected TPD
            var fcIdsWithFeedRate = existingFCInfos
                .Where(info => info.TPDId == tpdId)
                .Select(info => info.FCId)
                .Distinct();

            var availableFCs = fcs
                .Where(fc => !fcIdsWithFeedRate.Contains(fc.FCId))
                .Select(fc => new
                {
                    Value = fc.FCId,
                    Text = fc.FCValue
                })
                .ToList();

            // Return JSON data for AJAX
            return Json(availableFCs);
        }



        // GET: FCInfo/Edit/5
        public async Task<IActionResult> FCInfoEdit(int id)
        {
            var fCInfo = await _context.FCInfos.FindAsync(id);
            if (fCInfo == null)
            {
                return NotFound();
            }

            var fcs = await _fCInfoService.GetAllFCsAsync();
            var tpds = await _fCInfoService.GetAllTPDInfosAsync();

            ViewBag.FCId = new SelectList(fcs, "FCId", "FCValue", fCInfo.FCId);
            ViewBag.TPDId = new SelectList(tpds, "TPDId", "KilnName", fCInfo.TPDId);

            return View(fCInfo);
        }

        // POST: FCInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FCInfoEdit(int id, [Bind("FCInfoId,FCId,TPDId,FeedRate")] FCInfo fCInfo)
        {
            if (id != fCInfo.FCInfoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _fCInfoService.UpdateFCInfoAsync(fCInfo);

                    TempData["Message"] = "Feedrate Data Updated";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FCInfoExists(fCInfo.FCInfoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(FCInfoList));
            }

            var fcs = await _fCInfoService.GetAllFCsAsync();
            var tpds = await _fCInfoService.GetAllTPDInfosAsync();

            ViewBag.FCId = new SelectList(fcs, "FCId", "FCValue", fCInfo.FCId);
            ViewBag.TPDId = new SelectList(tpds, "TPDId", "KilnName", fCInfo.TPDId);

            return View(fCInfo);
        }
        private bool FCInfoExists(int id)
        {
            return _context.FCInfos.Any(e => e.FCInfoId == id);
        }


        // POST: FCInfo/Delete/5
        [HttpPost, ActionName("FCInfoDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FCInfoDeleteConfirmed(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            else
            {

            await _fCInfoService.DeleteFCInfoAsync(id);
            TempData["Message"] = "Feedrate Data Deleted";
            return RedirectToAction(nameof(FCInfoList));

            }
        }

    }
}
