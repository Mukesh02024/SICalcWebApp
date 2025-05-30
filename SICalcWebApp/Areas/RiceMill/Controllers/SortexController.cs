﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
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
            var model = new SortexProcess
            {
                StartTime = DateTime.Now

            };

            var occupiedSortexBunkers = await _sortexService.GetOccupiedSortexBunkersAsync();
            ViewBag.SortexBunkers = occupiedSortexBunkers;
           
    
            var masterData = await _machineProcessService.GetMasterDataAsync();
            ViewBag.StaffNames = masterData.StaffNames;


            return View(model); // Use Dryer-specific view model

        }


        [HttpPost]
        public async Task<JsonResult> GetBatchIds(string sortexBunkerName)
        {
            if (string.IsNullOrEmpty(sortexBunkerName))
            {
                return Json(new List<string>());
            }

            var batchIds = await _sortexService.GetBatchIdsForSortexAsync(sortexBunkerName);
            return Json(batchIds);
        }



        // POST: Start sortex  Process
        [HttpPost]
        public async Task<IActionResult> StartSortex(SortexProcess model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns in case of error
                var occupiedSortexBunkers = await _sortexService.GetOccupiedSortexBunkersAsync();
                ViewBag.SortexBunkers = occupiedSortexBunkers;
                var masterData = await _machineProcessService.GetMasterDataAsync();
                ViewBag.StaffNames = masterData.StaffNames;
               

                return View(model);
            }

            
            var sortexProcess = new SortexProcess
            {
                BatchId = model.BatchId,
                StaffName = model.StaffName,
                SortexBunkerName = model.SortexBunkerName,
                StartTime = model.StartTime,
                SaleType= model.SaleType,
                IsFRK = model.IsFRK, // ✅ Add this line to capture the checkbox value

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
        public async Task<IActionResult> PauseSortex(string batchId, string pauseReason,DateTime? PauseTime)
        {
            try
            {
                Console.WriteLine($"Received Batch ID: {batchId}, Pause Reason: {pauseReason}"); // Debugging

                if (!string.IsNullOrEmpty(batchId) && !string.IsNullOrEmpty(pauseReason))
                {
                    // Pause the process
                    await _sortexService.PauseProcessAsync(batchId, pauseReason,PauseTime);

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
        public async Task<IActionResult> ResumeSortex(string batchId,DateTime? ResumeTime)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _sortexService.ResumeProcessAsync(batchId,ResumeTime);
                return Json(new { success = true, message = "Process resumed successfully" });
            }
            catch (Exception ex)
            {
                // Log exception here
                return Json(new { success = false, message = "An error occurred while resuming the process." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> EndSortex(string batchId, DateTime? EndTime, decimal Endweight)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return Json(new { success = false, message = "Invalid Batch ID" });
            }

            try
            {
                await _sortexService.EndProcessAsync(batchId, EndTime,Endweight);
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
