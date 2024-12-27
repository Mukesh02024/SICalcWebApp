using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    public class SortexController : Controller
    {
        private readonly IMachineProcessService _machineProcessService;
        private readonly ApplicationDbContext _context;
        private readonly ISortexService _sortexService;
        public SortexController(IMachineProcessService machineProcessService, ApplicationDbContext context, ISortexService sortexService)
        {
            _machineProcessService = machineProcessService;
            _context = context;
            _sortexService=sortexService;

        }
        // GET: Dryer Initial Form
        public async Task<IActionResult> SortexMachine()
        {
            var activeProcess = await _sortexService.GetActiveProcessAsync();
            if (activeProcess != null)
            {
                // If there's an active process, redirect to the Dashboard to view the active process
                return RedirectToAction("Dashboard", new { batchId = activeProcess.BatchId });
            }
            var availableBatches = await _sortexService.GetAvailableBatchesForSortexAsync();
            var masterData = await _machineProcessService.GetMasterDataAsync();
            ViewBag.StaffNames = masterData.StaffNames;
            ViewBag.SortextBunker = masterData.SortexBunker;
            ViewBag.CompletedBatches = availableBatches;

            return View(new SortexProcess()); // Use Dryer-specific view model

        }


        // POST: Start sortex  Process
        [HttpPost]
        public async Task<IActionResult> StartSortex(SortexProcess model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns in case of error
                var availableBatches = await _sortexService.GetAvailableBatchesForSortexAsync();
                var masterData = await _machineProcessService.GetMasterDataAsync();
                ViewBag.StaffNames = masterData.StaffNames;
                ViewBag.SortextBunker = masterData.SortexBunker;
                ViewBag.CompletedBatches = availableBatches;

                return View(model);
            }

            //var ongoingProcess = await _machineProcessService.GetActiveProcessAsync();
            //if (ongoingProcess != null && (ongoingProcess.ProcessStatus == "In Progress" || ongoingProcess.ProcessStatus == "Paused"))
            //{
            //    return Json(new { success = false, message = "A process is already in progress or paused. Please wait until it completes." });
            //}

            // Create the Dryer process and save it to the DryerProcess table
            var sortexProcess = new SortexProcess
            {
                BatchId = model.BatchId,
                StaffName = model.StaffName,
                SortexBunkerName = model.SortexBunkerName,
                StartTime = DateTime.Now,
                ProcessStatus = "In Progress"
            };

            // Save Dryer Process
            await _sortexService.StartSortexProcessAsync(sortexProcess);

            // Pass BatchId to next machine for sharing
            TempData["BatchId"] = sortexProcess.BatchId;

            return RedirectToAction("Dashboard", new { batchId = sortexProcess.BatchId });
        }




        // GET: Dryer Dashboard
        public async Task<IActionResult> Dashboard(string batchId)
        {
            if (string.IsNullOrEmpty(batchId))
            {
                return RedirectToAction("SortexMachine");
            }

            var process = await _sortexService.GetSortexProcessAsync(batchId);
            if (process == null)
            {
                return RedirectToAction("SortexMachine");
            }

            //var masterData = await _machineProcessService.GetMasterDataAsync();
            //ViewBag.SortexList = masterData.SortexBunker;
            return View(process);
        }










        [HttpPost]
        public async Task<IActionResult> PauseSortex(string batchId, string pauseReason)
        {
            try
            {
                Console.WriteLine($"Received Batch ID: {batchId}, Pause Reason: {pauseReason}"); // Debugging

                if (!string.IsNullOrEmpty(batchId) && !string.IsNullOrEmpty(pauseReason))
                {
                    // Pause the process
                    await _sortexService.PauseProcessAsync(batchId, pauseReason);

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
        public async Task<IActionResult> ResumeSortex(string batchId)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _sortexService.ResumeProcessAsync(batchId);
                return Json(new { success = true, message = "Process resumed successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while resuming the process." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> EndSortex(string batchId)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _sortexService.EndProcessAsync(batchId);
                TempData["BatchId"] = batchId; // Retain this for redirection later if needed
                return Json(new { success = true, message = "Process ended successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while ending the process." });
            }
        }

    }
}
