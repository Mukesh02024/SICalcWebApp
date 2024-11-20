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
    public class TPDInfoController : Controller
    {

        private readonly ITPDInfoService _tpdInfoService;

        public TPDInfoController(ITPDInfoService tpdInfoService)
        {
            _tpdInfoService = tpdInfoService;
        }

        // GET: TPDInfo/TdpList

        public async Task<IActionResult> TdpList()
        {
            return View(await _tpdInfoService.GetAllTPDInfosAsync());
        }

        public IActionResult TpdCreate()
        {
            return View();
        }

        // POST: TPDInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TpdCreate([Bind("TPD,KilnName")] TPDInfo tpdInfo)
        {
            if (ModelState.IsValid)
            {
                await _tpdInfoService.AddTPDInfoAsync(tpdInfo);
                return RedirectToAction(nameof(TdpList));
            }
            return View(tpdInfo);

        }


        // GET: TPDInfo/Edit/5
        public async Task<IActionResult> TpdEdit(int id)
        {
            var tpdInfo = await _tpdInfoService.GetTPDInfoByIdAsync(id);
            if (tpdInfo == null)
            {
                return NotFound();
            }
            return View(tpdInfo);
        }

        // POST: TPDInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TpdEdit(int id, [Bind("TPDId,TPD,KilnName")] TPDInfo tpdInfo)
        {
            if (id != tpdInfo.TPDId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _tpdInfoService.UpdateTPDInfoAsync(tpdInfo);
                    TempData["Message"] = "Kiln Data Updated";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _tpdInfoService.GetTPDInfoByIdAsync(tpdInfo.TPDId) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(TdpList));
            }
            return View(tpdInfo);
        }


        // POST: TPDInfo/Delete/5
        [HttpPost, ActionName("TpdDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TpdDeleteConfirmed(int id)
        {
            if (id == 0)
            {
                return BadRequest();


            }
            else
            {
                await _tpdInfoService.DeleteTPDInfoAsync(id);
                TempData["Message"] = "Kiln Data Deleted";
                return RedirectToAction(nameof(TdpList));
            }
          
        }
    }
}
