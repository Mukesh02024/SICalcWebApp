using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
  
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
    public class GroupMillController : Controller
    {
        private readonly IGroupMillService _groupMillService;

        public GroupMillController(IGroupMillService groupMillService)
        {
            _groupMillService = groupMillService;
        }
        // GET: RiceMill/GroupMill/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RiceMill/GroupMill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupMill groupMill)
        {
            if (ModelState.IsValid)
            {
                await _groupMillService.CreateGroupMillAsync(groupMill);
                return RedirectToAction(nameof(Create));
            }
            return View(groupMill);
        }






        // GET: RiceMill/GroupMill/List
        public async Task<IActionResult> List()
        {
            var groupMills = await _groupMillService.GetAllGroupMillsAsync();
            return View(groupMills);
        }

        // GET: RiceMill/GroupMill/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var groupMill = await _groupMillService.GetGroupMillByIdAsync(id);
            if (groupMill == null)
            {
                return NotFound();
            }
            return View(groupMill);
        }

        // POST: RiceMill/GroupMill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GroupMill groupMill)
        {
            if (id != groupMill.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the group mill using the service
                    await _groupMillService.UpdateGroupMillAsync(groupMill);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GroupMillExists(groupMill.GroupId))
                    {
                        return NotFound();
                    }
                    throw; // Rethrow the exception if the entity still exists
                }
                return RedirectToAction(nameof(List));
            }
            return View(groupMill);
        }

       

        // POST: RiceMill/GroupMill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupMill = await _groupMillService.GetGroupMillByIdAsync(id);
            if (groupMill != null)
            {
                await _groupMillService.DeleteGroupMillAsync(id);
            }
            return RedirectToAction(nameof(List));
        }

        private async Task<bool> GroupMillExists(int id)
        {
            return await _groupMillService.GetGroupMillByIdAsync(id) != null;
        }



    }
}
