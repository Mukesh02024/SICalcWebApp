using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Data;
using SICalcWebApp.Repository;

namespace SICalcWebApp.Areas.SICalculator.Controllers
{
    [Area("SICalculator")]
    [Authorize(Roles = $"{SD.Role_Sponge_Admin},{SD.Role_Super_Admin}")]
    public class IronTypeController : Controller
    {
        private readonly IIronTypeService _ironTypeService;

        public IronTypeController(IIronTypeService ironTypeService)
        {
            _ironTypeService = ironTypeService;
        }
      

        public async Task<IActionResult> IronList()
        {
            var ironTypes = await _ironTypeService.GetAllIronTypesAsync();
            return View(ironTypes);
        }

        public IActionResult CreateIron()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIron(IronType ironType)
        {
            if (ModelState.IsValid)
            {
                await _ironTypeService.AddIronTypeAsync(ironType);
                return RedirectToAction(nameof(IronList));
            }
            return View(ironType);
        }


        public async Task<IActionResult> EditIron(int id)
        {
            var ironType = await _ironTypeService.GetAllIronTypesAsync();
            var ironTypeToEdit = ironType.FirstOrDefault(it => it.Id == id);
            if (ironTypeToEdit == null)
            {
                return NotFound();
            }
            return View(ironTypeToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIron(IronType ironType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ironTypeService.UpdateIronTypeAsync(ironType);
                    return RedirectToAction(nameof(IronList));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(ironType);
        }



        [HttpPost, ActionName("DeleteIron")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIronConfirmed(int id)
        {
            try
            {
                await _ironTypeService.DeleteIronTypeAsync(id);
                return RedirectToAction(nameof(IronList));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(IronList), new { id });
            }
        }



    }
}
