using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{

    [Area("RiceMill")]
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
    public class MillItemController : Controller
    {
        private readonly IMillItemService _millItemService;
        private readonly IGroupMillService _groupMillService; // Add this line for the service


        public MillItemController(IMillItemService millItemService, IGroupMillService groupMillService)
        {
            _millItemService = millItemService;
            _groupMillService = groupMillService;
        }
        public async Task<IActionResult> Create()
        {

            var groups = await _groupMillService.GetAllGroupMillsAsync(); // Get all groups
            var groupSelectList = new List<SelectListItem>
    {
  
    };

            groupSelectList.AddRange(groups.Select(g => new SelectListItem
            {
                Value = g.GroupId.ToString(),
                Text = g.GroupName
            }));

            ViewBag.GroupId = groupSelectList; // Assign the list to ViewBag
            return View();


  
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MillItem millItem)
        {
            if (ModelState.IsValid)
            {
                // Fetch the GroupMill based on the GroupId
                millItem.GroupMill = await _groupMillService.GetGroupMillByIdAsync(millItem.GroupId);

                // Save the MillItem with the populated GroupMill object
                await _millItemService.CreateMillItemAsync(millItem);
                return RedirectToAction(nameof(Create));
            }

            // If model state is not valid, repopulate the GroupId dropdown and return the view
            var groups = await _groupMillService.GetAllGroupMillsAsync();
            ViewBag.GroupId = new SelectList(groups, "GroupId", "GroupName", millItem.GroupId);

            return View(millItem);
        }



        public async Task<IActionResult> Index()
        {
            var millItems = await _millItemService.GetAllMillItemsAsync();
            return View(millItems);
        }



        // EDIT: RiceMill/MillItem/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var millItem = await _millItemService.GetMillItemByIdAsync(id);
            if (millItem == null)
            {
                return NotFound();
            }

            var groups = await _groupMillService.GetAllGroupMillsAsync();
            ViewBag.GroupId = new SelectList(groups, "GroupId", "GroupName", millItem.GroupId);

            return View(millItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MillItem millItem)
        {
            if (id != millItem.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _millItemService.UpdateMillItemAsync(millItem);
                return RedirectToAction(nameof(Index));
            }

            var groups = await _groupMillService.GetAllGroupMillsAsync();
            ViewBag.GroupId = new SelectList(groups, "GroupId", "GroupName", millItem.GroupId);

            return View(millItem);
        }

       

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _millItemService.DeleteMillItemAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
