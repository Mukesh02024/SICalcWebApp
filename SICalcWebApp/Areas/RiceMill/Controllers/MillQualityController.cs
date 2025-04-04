using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
    public class MillQualityController : Controller
    {
        private readonly IMillQualityService _millQualityService;

        public MillQualityController(IMillQualityService millQualityService)
        {
            _millQualityService = millQualityService;
        }

        public async Task<IActionResult> Index()
        {
            var allBatches = await _millQualityService.GetRemainingBatchesAsync();
            return View(allBatches);
        }


        [HttpPost]
        public async Task<IActionResult> SubmitQuality(MillQuality model)
        {
            if (ModelState.IsValid)
            {
                var success = await _millQualityService.SubmitQualityAsync(model);
                return Json(new { success });
            }
            return Json(new { success = false });
        }


        public async Task<IActionResult> SortexQuality()
        {
            var allBatches = await _millQualityService.GetRemainingBatchesSortexAsync();
            return View(allBatches);
        }


        [HttpPost]
        public async Task<IActionResult> SubmitSortexQuality(MillQualitySortex model)
        {
            if (ModelState.IsValid)
            {
                var success = await _millQualityService.SubmitQualitySortexAsync(model);
                return Json(new { success });
            }
            return Json(new { success = false });
        }
    }
}
