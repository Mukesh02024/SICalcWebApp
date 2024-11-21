using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    public class PlantOperateController : Controller
    {
        private readonly IMasterMillPlant _service;

        public PlantOperateController(IMasterMillPlant service)
        {
            _service = service;
        }

        // Staff Methods
        public async Task<IActionResult> StaffList()
        {
            var staff = await _service.GetAllStaffAsync();
            return View(staff);
        }

        public IActionResult AddStaff()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStaff(Staff staff)
        {
            if (ModelState.IsValid)
            {
                await _service.AddStaffAsync(staff);
                return RedirectToAction(nameof(StaffList));
            }
            return View(staff);
        }

        public async Task<IActionResult> EditStaff(int id)
        {
            var staff = await _service.GetStaffByIdAsync(id);
            if (staff == null) return NotFound();
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(Staff staff)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateStaffAsync(staff);
                return RedirectToAction(nameof(StaffList));
            }
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            try
            {
                // Call your service to delete the staff record
                await _service.DeleteStaffAsync(id);

                // Return success response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Paddy Type Methods
        public async Task<IActionResult> PaddyTypeList()
        {
            var paddyTypes = await _service.GetAllPaddyTypesAsync();
            return View(paddyTypes);
        }

        public IActionResult AddPaddyType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPaddyType(PaddyType paddyType)
        {
            if (ModelState.IsValid)
            {
                await _service.AddPaddyTypeAsync(paddyType);
                return RedirectToAction(nameof(PaddyTypeList));
            }
            return View(paddyType);
        }

        public async Task<IActionResult> EditPaddyType(int id)
        {
            var paddyType = await _service.GetPaddyTypeByIdAsync(id);
            if (paddyType == null) return NotFound();
            return View(paddyType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPaddyType(PaddyType paddyType)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdatePaddyTypeAsync(paddyType);
                return RedirectToAction(nameof(PaddyTypeList));
            }
            return View(paddyType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePaddyType(int id)
        {
            //await _service.DeletePaddyTypeAsync(id);
            //return RedirectToAction(nameof(PaddyTypeList));


            try
            {
                // Call your service to delete the staff record
                await _service.DeletePaddyTypeAsync(id);

                // Return success response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Json(new { success = false, message = ex.Message });
            }
        }













        public IActionResult AddFeedBunker()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedBunker(FeedingBunker bunker)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(bunker);
                return RedirectToAction(nameof(AddFeedBunker));
            }
            return View(bunker);
        }













    }
}
