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
    public class PriceOfMController : Controller
    {
        public readonly IPriceMaterial _priceMaterial;

        public PriceOfMController(IPriceMaterial priceMaterial)
        {
            _priceMaterial = priceMaterial;
                
        }
        public async Task<IActionResult> PriceList()
        {
            var PriceValue = await _priceMaterial.GetAllPriceMaterialAsync();
            return View(PriceValue);
        }

        public IActionResult CreatePrice()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrice(PriceOfMaterial priceOfMaterial)
        {
            if (ModelState.IsValid)
            {
                await _priceMaterial.AddPriceofmaterialAsync(priceOfMaterial);
                return RedirectToAction(nameof(PriceList));
            }
            return View(priceOfMaterial);
        }




        // GET: PriceMaterial/Edit
        public async Task<IActionResult> EditPrice()
        {
            var priceMaterial = await _priceMaterial.GetPriceMaterialAsync();
            if (priceMaterial == null)
            {
                return NotFound();
            }
            return View(priceMaterial);
        }

        // POST: PriceMaterial/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPrice(PriceOfMaterial priceMaterial)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _priceMaterial.UpdatePriceMaterialAsync(priceMaterial);
                    TempData["Message"] = "Price details Updated";
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction(nameof(PriceList)); // Redirect back to the edit view after successful update
            }
            return View(priceMaterial);
        }
    }
}
