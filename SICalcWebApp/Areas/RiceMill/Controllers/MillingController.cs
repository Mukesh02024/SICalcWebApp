using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Areas.RiceMill.VM;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
    public class MillingController : Controller
    {
        private readonly IMachineProcessService _machineProcessService;
        private readonly ApplicationDbContext _context;
        private readonly IMillingProcessService _millingProcessService;
        public MillingController(IMachineProcessService machineProcessService, ApplicationDbContext context, IMillingProcessService millingProcessService)
        {
            _machineProcessService = machineProcessService;
            _context = context;
            _millingProcessService=millingProcessService;
            
        }
  
        public async Task<IActionResult> StartMilling()
        {
            var activeProcess = await _millingProcessService.GetActiveProcessAsync();

            if (activeProcess != null)
            {
                // If there's an active process, redirect to the Dashboard to view the active process
                return RedirectToAction("Dashboard", new { batchId = activeProcess.BatchId });
            }


            var model = new MillingProcessViewModel
            {
                Bunkers = await _millingProcessService.GetOccupiedBunkersAsync(),
                Batches = new List<SelectListItem>(), // Initialize with an empty list
                Staffs = await GetStaffList(),
          
           
            };

            return View(model);
        }


        // POST: Start Milling  Process

        [HttpPost]
        public async Task<IActionResult> StartMilling(MillingProcessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns if there are validation errors
                model.Bunkers = await _millingProcessService.GetOccupiedBunkersAsync();
                model.Staffs = await GetStaffList();
                return View(model);
            }

            // Create the MillingProcess object
            var millingProcess = new MillingProcess
            {
                BatchId = model.BatchId,
                StaffName = model.StaffName,
                MillBunkerName = model.MillBunkerName,
                StartTime = model.StartTime,
                ProcessStatus = "In Progress"
            };

            // Start the milling process
            await _millingProcessService.StartMillProcessAsync(millingProcess);

            // Mark the selected bunker as 'EMPTY'
            var bunker = await _millingProcessService.GetBunkerByNameAsync(model.MillBunkerName);
            if (bunker != null)
            {
                bunker.Status = "EMPTY"; // Mark the bunker as empty after starting milling
                await _millingProcessService.UpdateBunkerStatusAsync(bunker);
            }

            // Pass the BatchId to the next machine (if necessary)
            TempData["BatchId"] = millingProcess.BatchId;

            return RedirectToAction("Dashboard", new { batchId = millingProcess.BatchId });
        }



        // GET: Dryer Dashboard
        public async Task<IActionResult> Dashboard(string batchId)
        {
            if (string.IsNullOrEmpty(batchId))
            {
                return RedirectToAction("MillingMachine");
            }

            var process = await _millingProcessService.GetMillProcessAsync(batchId);
            if (process == null)
            {
                return RedirectToAction("MillingMachine");
            }

            var masterData = await _machineProcessService.GetMasterDataAsync();
            ViewBag.SortexList = masterData.SortexBunker;


            var emptyBunkers = await _millingProcessService.GetEmptySortexBunkersAsync();
            ViewBag.SortexList = emptyBunkers; // Pass the empty bunkers to the view
            return View(process);
        }



        [HttpPost]
        public async Task<IActionResult> PauseMill(string batchId, string pauseReason,DateTime? PauseTime)
        {
            try
            {
                Console.WriteLine($"Received Batch ID: {batchId}, Pause Reason: {pauseReason}"); // Debugging

                if (!string.IsNullOrEmpty(batchId) && !string.IsNullOrEmpty(pauseReason))
                {
                    // Pause the process
                    await _millingProcessService.PauseProcessAsync(batchId, pauseReason, PauseTime);

                    return Json(new { success = true, message = "Process Paused" });
                }

                return Json(new { success = false, message = "Invalid Batch ID or Pause Reason" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PauseDryer: {ex.Message}"); // Debugging
                return Json(new { success = false, message = "An error occurred while pausing the process." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> ResumeMill(string batchId,DateTime? ResumeTime)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _millingProcessService.ResumeProcessAsync(batchId, ResumeTime);
                return Json(new { success = true, message = "Process resumed successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while resuming the process." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> EndMill(string batchId, string SortexBunker,DateTime? EndTime)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _millingProcessService.EndProcessAsync(batchId, SortexBunker, EndTime);
                TempData["BatchId"] = batchId; // Retain this for redirection later if needed
                return Json(new { success = true, message = "Process ended successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while ending the process." });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetBatchesForBunker(string occupiedBunkerName)
        {
            // Ensure that occupiedBunkerName is passed correctly
            if (string.IsNullOrEmpty(occupiedBunkerName))
            {
                return Json(new List<SelectListItem>());
            }

            var batches = await _millingProcessService.GetBatchesForOccupiedBunkerAsync(occupiedBunkerName);
            return Json(batches);  // Return as JSON for use in the view
        }





        [HttpPost]
        public async Task<IActionResult> CheckBunkerStatus()
        {
            try
            {
                // Check if any bunker is empty using the service
                bool isAnyBunkerEmpty = await _millingProcessService.IsAnyBunkerSortexEmptyAsync();
                if (!isAnyBunkerEmpty)
                {
                    return Json(new { success = false, message = "YOU CANNOT UNLOAD PADDY IN  SORTEX BUNKER BECAUSE ALL BUNKERS ARE OCCUPIED" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "An error occurred while checking bunker status." });
            }
        }





        private async Task<List<SelectListItem>> GetStaffList()
        {
            return await _context.Staffs
                .Select(s => new SelectListItem
                {
                    Value = s.StaffName,
                    Text = s.StaffName
                })
                .ToListAsync();
        }



    }
}
