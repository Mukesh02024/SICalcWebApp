using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Data;
using SICalcWebApp.Repository;

namespace SICalcWebApp.Areas.SICalculator.Controllers
{
    [Area("SICalculator")]
    [Authorize(Roles = $"{SD.Role_Sponge_Admin},{SD.Role_Super_Admin}")]
    public class FCController : Controller
    {
        private readonly IFCService _fcService;

        public FCController(IFCService fcService)
        {
            _fcService = fcService;
        }

        // GET: FC
        public async Task<IActionResult> FcList()
        {
            return View(await _fcService.GetAllFCsAsync());
        }

        // GET: FC/Create
        public IActionResult FcCreate()
        {
            return View();
        }

        // POST: FC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FcCreate([Bind("FCValue,C_Fe")] FC fc)
        {
            if (ModelState.IsValid)
            {
                await _fcService.CreateFCAsync(fc);
                TempData["Message"] = "Fc Data Inserted succesfully";
                return RedirectToAction(nameof(FcList));
            }
            return View(fc);
        }

        // GET: FC/Edit/5
        public async Task<IActionResult> FcEdit(int id)
        {
            var fc = await _fcService.GetFCByIdAsync(id);
            if (fc == null)
            {
                return NotFound();
            }
            return View(fc);
        }

        // POST: FC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FcEdit(int id, [Bind("FCId,FCValue,C_Fe")] FC fc)
        {
            if (id != fc.FCId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _fcService.UpdateFCAsync(fc);
                    TempData["Message"] = "Fc Data Updated succesfully";
                    return RedirectToAction(nameof(FcList));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency issue if necessary
                }
            }
            return View(fc);
        }


        [HttpPost, ActionName("FcDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FcDeleteConfirmed(int id)
        {
            await _fcService.DeleteFCAsync(id);
            TempData["Message"] = "Fc Data Deleted succesfully";
            return RedirectToAction(nameof(FcList));
        }




    }
}
