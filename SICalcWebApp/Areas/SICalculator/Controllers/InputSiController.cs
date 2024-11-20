using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.SICalculator.VM;
using SICalcWebApp.Data;
using SICalcWebApp.Repository;

namespace SICalcWebApp.Areas.SICalculator.Controllers
{
    [Area("SICalculator")]
    [Authorize(Roles = $"{SD.Role_Sponge_Admin},{SD.Role_Super_Admin}")]
    public class InputSiController : Controller
    {
        private readonly IInputOperandsService _inputOperandsService;
        private readonly IIronTypeService _ironTypeService;
        public InputSiController(IInputOperandsService inputOperandsService, IIronTypeService ironTypeService)
        {
            _inputOperandsService = inputOperandsService;
            _ironTypeService = ironTypeService;
        }


        public async Task<IActionResult> OperandList()
        {
            var inputOperands = await _inputOperandsService.GetAllInputOperandsAsync();
            return View(inputOperands);
        }

        public async Task<IActionResult> CreateOperand()
        {
            var ironTypes = await _ironTypeService.GetAllIronTypesAsync();
            var viewModel = new InputOperandsVM
            {
                IronTypes = ironTypes
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOperand(InputOperandsVM viewModel)
        {
            if (ModelState.IsValid)
            {
                await _inputOperandsService.AddInputOperandsAsync(viewModel.InputOperandss);
                return RedirectToAction(nameof(OperandList));
            }

            viewModel.IronTypes = await _ironTypeService.GetAllIronTypesAsync(); // Reload the dropdown if model validation fails
            return View(viewModel);
        }


        // GET: Edit Operand
        public async Task<IActionResult> EditOperand(int id)
        {
            var inputOperand = await _inputOperandsService.GetInputOperandByIdAsync(id);
            if (inputOperand == null)
            {
                return NotFound();
            }

            var ironTypes = await _ironTypeService.GetAllIronTypesAsync();
            var viewModel = new InputOperandsVM
            {
                InputOperandss = inputOperand,
                IronTypes = ironTypes
            };

            return View(viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOperand(int id, InputOperandsVM viewModel)
        {
            if (id != viewModel.InputOperandss.ProductID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _inputOperandsService.UpdateInputOperandAsync(viewModel.InputOperandss);
                TempData["Message"] = "Material Updated succesfully";
                return RedirectToAction(nameof(OperandList));
             
            }

            viewModel.IronTypes = await _ironTypeService.GetAllIronTypesAsync(); // Reload the dropdown if model validation fails
            return View(viewModel);
        }


        // POST: Delete Operand
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOperand(int id)
        {
            if (id == 0)
            {
                return BadRequest();

                
            }
            else
            {
                await _inputOperandsService.DeleteInputOperandAsync(id);
                TempData["Message"] = "Material Deleted succesfully";
                return RedirectToAction(nameof(OperandList));
            }
        }
    }
}
