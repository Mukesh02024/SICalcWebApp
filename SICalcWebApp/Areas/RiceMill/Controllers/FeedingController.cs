using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;
using SICalcWebApp.Repository;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
    public class FeedingController : Controller
    {
        private readonly IMasterMillPlant _service;
        public IActionResult Index()
        {
            return View();
        }

        public FeedingController(IMasterMillPlant service)
        {
            _service = service;
        }
        public async Task<IActionResult> ListFeed()
        {
            var feedingModules = await _service.GetAllAsync<FeedingModuleF>();  // Assuming the model is FeedingModuleF
            return View(feedingModules);
        }

        public async Task<IActionResult> AddFeed()
        {

           var FeedingBunkers = await _service.GetAllAsync<FeedingBunker>();
            ViewBag.FeedingBunkers = FeedingBunkers.Select(fb => fb.FeedingBName).ToList();


            var PaddyTypes = await _service.GetAllAsync<PaddyType>();
            ViewBag.PaddyTypes= PaddyTypes.Select(fb => fb.PaddyTypeName).ToList();


             var Staff = await _service.GetAllAsync<Staff>();

            ViewBag.Staff = Staff.Select(fb => fb.StaffName).ToList();


            return View(new FeedingModuleF());
        }



        [HttpPost]
        public async Task<IActionResult> AddFeed(FeedingModuleF module)
        {
            if (ModelState.IsValid)
            {
                // Save the module data to the database
                await _service.AddAsync(module);
                return RedirectToAction(nameof(ListFeed)); // Redirect to the list or appropriate page
            }

            // Reload dropdown data in case of validation failure
            var FeedingBunkers = await _service.GetAllAsync<FeedingBunker>();
            ViewBag.FeedingBunkers = FeedingBunkers.Select(fb => fb.FeedingBName).ToList();

            var PaddyTypes = await _service.GetAllAsync<PaddyType>();
            ViewBag.PaddyTypes = PaddyTypes.Select(pt => pt.PaddyTypeName).ToList();

            var Staff = await _service.GetAllAsync<Staff>();
            ViewBag.Staff = Staff.Select(s => s.StaffName).ToList();

            return View(module); // Return the view with validation errors displayed
        }





        [HttpGet]
        public async Task<IActionResult> EditFeed(int id)
        {
            var feedingModule = await _service.GetByIdAsync<FeedingModuleF>(id);
            if (feedingModule == null)
            {
                return NotFound();
            }

            var FeedingBunkers = await _service.GetAllAsync<FeedingBunker>();
            ViewBag.FeedingBunkers = FeedingBunkers;

            // Populate dropdowns
       

            var PaddyTypes = await _service.GetAllAsync<PaddyType>();
            ViewBag.PaddyTypes = PaddyTypes;

            var Staff = await _service.GetAllAsync<Staff>();
            ViewBag.Staff = Staff;

            return View(feedingModule);
        }

        [HttpPost]
        public async Task<IActionResult> EditFeed(FeedingModuleF module)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(module);  // Assuming the service handles the update
                return RedirectToAction(nameof(ListFeed));  // Redirect back to the list
            }

            // Reload dropdown data in case of validation failure
            var FeedingBunkers = await _service.GetAllAsync<FeedingBunker>();
            ViewBag.FeedingBunkers = FeedingBunkers.Select(fb => fb.FeedingBName).ToList();

            var PaddyTypes = await _service.GetAllAsync<PaddyType>();
            ViewBag.PaddyTypes = PaddyTypes.Select(pt => pt.PaddyTypeName).ToList();

            var Staff = await _service.GetAllAsync<Staff>();
            ViewBag.Staff = Staff.Select(s => s.StaffName).ToList();

            return View(module); // Return the view with validation errors displayed
        }



        [HttpPost]
        public async Task<IActionResult> DeleteFeed(int id)
        {
            await _service.DeleteAsync<FeedingModuleF>(id);  // Delete the record from the database
            return RedirectToAction(nameof(ListFeed));  // Redirect back to the list
        }


    }
}
