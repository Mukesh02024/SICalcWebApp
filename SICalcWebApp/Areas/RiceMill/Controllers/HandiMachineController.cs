using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Areas.RiceMill.VM;
using SICalcWebApp.Data;
using System.Diagnostics;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
    public class HandiMachineController : Controller
    {
        private readonly IMachineProcessService _machineProcessService;
        private readonly ApplicationDbContext _context;
        public HandiMachineController(IMachineProcessService machineProcessService, ApplicationDbContext context)
        {
            _machineProcessService = machineProcessService;
            _context = context;
        }

        // GET: Handi Form
        public async Task<IActionResult> Handi()
        {


            var activeProcess = await _machineProcessService.GetActiveProcessAsync();

            if (activeProcess != null)
            {
                // If there's an active process, redirect to the Dashboard to view the active process
                return RedirectToAction("Dashboard", new { batchId = activeProcess.BatchId });
            }
            var model = new HandiProcess
            {
                StartTime = DateTime.Now // Set default value
            };

            var masterData = await _machineProcessService.GetMasterDataAsync();
            ViewBag.ProcessTypes = masterData.ProcessTypes;
            ViewBag.PaddyTypes = masterData.PaddyTypes;
            ViewBag.HandiTypes = masterData.HandiTypes;
            ViewBag.StaffNames = masterData.StaffNames;
            ViewBag.WaterTypes = new List<string> { "New Water", "Old Water" };

            return View(model); // Pass the model to the view

            //return View(new HandiProcess()); // Use Handi-specific view model
        }



        [HttpPost]
        public async Task<IActionResult> StartHandi(HandiProcess model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // Reload dropdowns in case of error
                var masterData = await _machineProcessService.GetMasterDataAsync();
                ViewBag.ProcessTypes = masterData.ProcessTypes;
                ViewBag.PaddyTypes = masterData.PaddyTypes;
                ViewBag.HandiTypes = masterData.HandiTypes;
                ViewBag.StaffNames = masterData.StaffNames;

                return View(model);
            }

            var ongoingProcess = await _machineProcessService.GetActiveProcessAsync();
            if (ongoingProcess != null && (ongoingProcess.ProcessStatus == "In Progress" || ongoingProcess.ProcessStatus == "Paused"))
            {
                return Json(new { success = false, message = "A process is already in progress or paused. Please wait until it completes." });
            }

            // Handle HandiRunCount logic (default to 8 if null)
            //var handiRunCount = model.HandiRunCount ?? 8;

            // Determine the Batch ID prefix based on the ProcessType
            string batchPrefix = model.ProcessType?.ToUpper() switch
            {
                "ARWA" => "ARWA",
                "USNA" => "USNA",
                _ => "GENERAL" // Default prefix for unsupported process types
            };

            // Create the Handi process and save it to the HandiProcess table
            var handiProcess = new HandiProcess
            {
                BatchId = $"{batchPrefix}-{_machineProcessService.GetNextBatchId(batchPrefix)}",
                ProcessType = model.ProcessType,
                PaddyType = model.PaddyType,
                HandiType = model.HandiType,
                WaterType = model.WaterType,
                Pressure = model.Pressure,
                StaffName = model.StaffName,
                StartTime = model.StartTime,
                PaddyWeight=model.PaddyWeight,
                ProcessStatus = "In Progress"
            };

            // Save Handi Process
            await _machineProcessService.StartHandiProcessAsync(handiProcess);

            // Pass BatchId to the next machine for sharing
            TempData["BatchId"] = handiProcess.BatchId;

            return RedirectToAction("Dashboard", new { batchId = handiProcess.BatchId });
        }



        public async Task<IActionResult> Dashboard(string batchId)
        {
            if (string.IsNullOrEmpty(batchId))
            {
                return RedirectToAction("Handi");
            }

            var process = await _machineProcessService.GetHandiProcessAsync(batchId);
            var emptyBunkers = _context.MillBunkers
               .Where(bunker => bunker.Status == "EMPTY")
               .Select(bunker => bunker.MillBName)  // Select only the Name property
               .ToList();

            // Pass the list to the view using ViewBag
            ViewBag.EmptyBunkers = emptyBunkers;



            if (process == null)
            {
                return RedirectToAction("Handi");
            }

            Console.WriteLine($"ProcessStatus in Dashboard: {process.ProcessStatus}"); // Debugging



            return View(process);
        }

        // Pause process for Handi machine
        [HttpPost]
        public async Task<IActionResult> PauseHandi(string batchId, string pauseReason, DateTime? pauseTime)
        {
            try
            {
                Console.WriteLine($"Received Batch ID: {batchId}, Pause Reason: {pauseReason}, Pause Time: {pauseTime}"); // Debugging

                if (!string.IsNullOrEmpty(batchId) && !string.IsNullOrEmpty(pauseReason) && pauseTime.HasValue)
                {
                    // Pause the process
                    await _machineProcessService.PauseProcessAsync(batchId, pauseReason, pauseTime.Value);

                    return Json(new { success = true, message = "Process Paused" });
                }

                return Json(new { success = false, message = "Invalid Batch ID or Pause Reason" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PauseHandi: {ex.Message}"); // Debugging
                return Json(new { success = false, message = "An error occurred while pausing the process." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> ResumeHandi(string batchId, DateTime? resumeTime)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _machineProcessService.ResumeProcessAsync(batchId, resumeTime);
                return Json(new { success = true, message = "Process resumed successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while resuming the process." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> EndHandi(string batchId, DateTime? endTime)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                // Check if any batch in the dryer is in "Paused" or "In Progress" status
                bool isDryerFree = await _machineProcessService.IsDryerFreeAsync();
                if (!isDryerFree)
                {
                    // Return a failure message if the dryer is not free
                    return Json(new { success = false, message = "Dryer not free" });
                }

                bool areAllBatchesInDryerProcess = await _machineProcessService.AreAllCompletedBatchesInDryerProcessAsync();
                if (!areAllBatchesInDryerProcess)
                {
                    return Json(new { success = false, message = "Dryer unavailable or pending batch in process." });
                }




                // Proceed with ending the Handi process if dryer is free
                await _machineProcessService.EndProcessAsync(batchId,endTime);
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
        public IActionResult CompleteArwaProcess(string batchId, string unloadBunker, DateTime? endTime)
        {
            // Call CompleteArwaProcess and store its result in processCompleted
            bool processCompleted = _machineProcessService.CompleteArwaProcess(batchId, unloadBunker, endTime);

            // Check if the process was completed successfully
            if (!processCompleted)
            {
                return Json(new { success = false, message = "Process or bunker update failed." });
            }

            return Json(new { success = true });
        }




    }
}
