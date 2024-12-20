using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
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
        // GET: Dryer Initial Form
        public async Task<IActionResult> MillingMachine()
        {
            //var activeProcess = await _dryerService.GetActiveProcessAsync();
            //if (activeProcess != null)
            //{
            //    // If there's an active process, redirect to the Dashboard to view the active process
            //    return RedirectToAction("Dashboard", new { batchId = activeProcess.BatchId });
            //}
            var availableBatches = await _millingProcessService.GetAvailableBatchesForMillAsync();
            var masterData = await _machineProcessService.GetMasterDataAsync();
            ViewBag.StaffNames = masterData.StaffNames;
            ViewBag.MillBunkers = masterData.MillBunkers;
            ViewBag.CompletedBatches = availableBatches;

            return View(new MillingProcess()); // Use Dryer-specific view model
        }



        // POST: Start Milling  Process
        [HttpPost]
        public async Task<IActionResult> StartMilling (MillingProcess model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns in case of error
                var availableBatches = await _millingProcessService.GetAvailableBatchesForMillAsync();
                var masterData = await _machineProcessService.GetMasterDataAsync();
                ViewBag.StaffNames = masterData.StaffNames;
                ViewBag.MillBunkers = masterData.MillBunkers;
                ViewBag.CompletedBatches = availableBatches;

                return View(model);
            }

            //var ongoingProcess = await _machineProcessService.GetActiveProcessAsync();
            //if (ongoingProcess != null && (ongoingProcess.ProcessStatus == "In Progress" || ongoingProcess.ProcessStatus == "Paused"))
            //{
            //    return Json(new { success = false, message = "A process is already in progress or paused. Please wait until it completes." });
            //}

            // Create the Dryer process and save it to the DryerProcess table
            var MillProcess = new MillingProcess
            {
                BatchId = model.BatchId,
                StaffName = model.StaffName,
                MillBunkerName = model.MillBunkerName,
                StartTime = DateTime.Now,
                ProcessStatus = "In Progress"
            };

            // Save Dryer Process
            await _millingProcessService.StartMillProcessAsync(MillProcess);

            // Pass BatchId to next machine for sharing
            TempData["BatchId"] = MillProcess.BatchId;

            return RedirectToAction("Dashboard", new { batchId = MillProcess.BatchId });
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
            return View(process);
        }










        [HttpPost]
        public async Task<IActionResult> PauseMill(string batchId, string pauseReason)
        {
            try
            {
                Console.WriteLine($"Received Batch ID: {batchId}, Pause Reason: {pauseReason}"); // Debugging

                if (!string.IsNullOrEmpty(batchId) && !string.IsNullOrEmpty(pauseReason))
                {
                    // Pause the process
                    await _millingProcessService.PauseProcessAsync(batchId, pauseReason);

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
        public async Task<IActionResult> ResumeMill(string batchId)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _millingProcessService.ResumeProcessAsync(batchId);
                return Json(new { success = true, message = "Process resumed successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while resuming the process." });
            }
        }


    }
}
