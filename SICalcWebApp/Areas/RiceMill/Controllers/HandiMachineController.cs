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
            var masterData = await _machineProcessService.GetMasterDataAsync();
            ViewBag.ProcessTypes = masterData.ProcessTypes;
            ViewBag.PaddyTypes = masterData.PaddyTypes;
            ViewBag.HandiTypes = masterData.HandiTypes;
            ViewBag.StaffNames = masterData.StaffNames;

            return View(new HandiProcess()); // Use Handi-specific view model
        }

        // POST: Start Handi Process

        // POST: Start Handi Process
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







            // Create the Handi process and save it to the HandiProcess table
            var handiProcess = new HandiProcess
            {
                BatchId = $"RICE-{_machineProcessService.GetNextBatchId()}",
                ProcessType = model.ProcessType,
                PaddyType = model.PaddyType,
                HandiType = model.HandiType,
                Temperature = model.Temperature,
                Pressure = model.Pressure,
                StaffName = model.StaffName,
                StartTime = DateTime.Now,
                ProcessStatus = "In Progress"
            };

            // Save Handi Process
            await _machineProcessService.StartHandiProcessAsync(handiProcess);

            // Pass BatchId to next machine for sharing
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
            if (process == null)
            {
                return RedirectToAction("Handi");
            }

            Console.WriteLine($"ProcessStatus in Dashboard: {process.ProcessStatus}"); // Debugging
            return View(process);
        }

        // Pause process for Handi machine
        [HttpPost]
        public async Task<IActionResult> PauseHandi(string batchId, string pauseReason)
        {
            try
            {
                Console.WriteLine($"Received Batch ID: {batchId}, Pause Reason: {pauseReason}"); // Debugging

                if (!string.IsNullOrEmpty(batchId) && !string.IsNullOrEmpty(pauseReason))
                {
                    // Pause the process
                    await _machineProcessService.PauseProcessAsync(batchId, pauseReason);

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
        public async Task<IActionResult> ResumeHandi(string batchId)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _machineProcessService.ResumeProcessAsync(batchId);
                return Json(new { success = true, message = "Process resumed successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while resuming the process." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EndHandi(string batchId)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _machineProcessService.EndProcessAsync(batchId);
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
