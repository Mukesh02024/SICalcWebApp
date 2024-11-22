using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Repository;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
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
                return RedirectToAction(nameof(Index)); // Redirect to the list or appropriate page
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

    }
}
