using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
    public class DryerController : Controller
    {
        private readonly IMachineProcessService _machineProcessService;
        private readonly ApplicationDbContext _context;
        private readonly IDryerService _dryerService;
        public DryerController(IMachineProcessService machineProcessService, ApplicationDbContext context, IDryerService dryerService)
        {
            _machineProcessService = machineProcessService;
            _context = context;
            _dryerService = dryerService;
        }

        // GET: Dryer Initial Form
        public async Task<IActionResult> DryerMachine()
        {
            var activeProcess = await _dryerService.GetActiveProcessAsync();
            var model = new DryerProcess
            {
                LoadTime = DateTime.Now // Set default value
            };
            if (activeProcess != null)
            {
                // If there's an active process, redirect to the Dashboard to view the active process
                return RedirectToAction("Dashboard", new { batchId = activeProcess.BatchId });
            }
            var availableBatches = await _dryerService.GetAvailableBatchesForDryerAsync();
            var masterData = await _machineProcessService.GetMasterDataAsync();
            ViewBag.StaffNames = masterData.StaffNames;
            //ViewBag.UnloadingBunkers = masterData.MillBunkers;
            ViewBag.CompletedBatches = availableBatches;

            return View(model); // Use Dryer-specific view model
        }

        // POST: Start Dryer Process
        [HttpPost]
        public async Task<IActionResult> StartDryer(DryerProcess model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns in case of error
                var availableBatches = await _dryerService.GetAvailableBatchesForDryerAsync();
                var masterData = await _machineProcessService.GetMasterDataAsync();
                ViewBag.StaffNames = masterData.StaffNames;
                //ViewBag.UnloadingBunkers = masterData.MillBunkers;
                ViewBag.CompletedBatches = availableBatches;

                return View(model);
            }


            var dryerProcess = new DryerProcess
            {
                BatchId = model.BatchId,
                StaffName = model.StaffName,
                //UnloadBunkerName = model.UnloadBunkerName,
                DuctiPressure = model.DuctiPressure,
                LoadTime = model.LoadTime,
                ProcessStatus = "In Progress"
            };

            // Save Dryer Process
            await _dryerService.StartDryerProcessAsync(dryerProcess);

            // Pass BatchId to next machine for sharing
            TempData["BatchId"] = dryerProcess.BatchId;

            return RedirectToAction("Dashboard", new { batchId = dryerProcess.BatchId });
        }

        // GET: Dryer Dashboard
        public async Task<IActionResult> Dashboard(string batchId)
        {
            if (string.IsNullOrEmpty(batchId))
            {
                return RedirectToAction("DryerMachine");
            }

            var process = await _dryerService.GetDryerProcessAsync(batchId);
            if (process == null)
            {
                return RedirectToAction("Dryer");
            }


            //TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"); // Replace with your desired time zone
            //if (process.LoadTime.HasValue)
            //{
            //    process.LoadTime = TimeZoneInfo.ConvertTimeFromUtc(process.LoadTime.Value, localTimeZone);
            //}
            //if (process.UnloadTime.HasValue)
            //{
            //    process.UnloadTime = TimeZoneInfo.ConvertTimeFromUtc(process.UnloadTime.Value, localTimeZone);
            //}


            var emptyBunkers = await _dryerService.GetEmptyBunkersAsync();
            ViewBag.SortexList = emptyBunkers; // Pass the empty bunkers to the view
           
            return View(process);
           
        }



        [HttpPost]
        public async Task<IActionResult> PauseDryer(string batchId, string pauseReason,DateTime? pauseTime)
        {
            try
            {
                Console.WriteLine($"Received Batch ID: {batchId}, Pause Reason: {pauseReason}"); // Debugging

                if (!string.IsNullOrEmpty(batchId) && !string.IsNullOrEmpty(pauseReason))
                {
                    // Pause the process
                    await _dryerService.PauseProcessAsync(batchId, pauseReason, pauseTime);

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
        public async Task<IActionResult> ResumeDryer(string batchId,DateTime? resumeTime)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _dryerService.ResumeProcessAsync(batchId, resumeTime);
                return Json(new { success = true, message = "Process resumed successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while resuming the process." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EndDryer(string batchId,string unloadBunkerName,DateTime? UnloadTime)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _dryerService.EndProcessAsync(batchId, unloadBunkerName, UnloadTime);
                TempData["BatchId"] = batchId; // Retain this for redirection later if needed
                return Json(new { success = true, message = "Process ended successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while ending the process." });
            }
        }








        [HttpPost]
        public async Task<IActionResult> CheckBunkerStatus()
        {
            try
            {
                bool isAnyBunkerEmpty = await _dryerService.IsAnyBunkerEmptyAsync();
                if (!isAnyBunkerEmpty)
                {
                    return Json(new { success = false, message = "YOU CANNOT UNLOAD PADDY FOR MILLING BECAUSE ALL BUNKERS ARE OCCUPIED" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while checking bunker status." });
            }
        }


    }
}
