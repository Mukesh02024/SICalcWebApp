﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class MillingProcessService : IMillingProcessService
    {

        private readonly ApplicationDbContext _context;
        public MillingProcessService(ApplicationDbContext context)
        {
            _context = context;
                
        }
        public async Task<List<string>> GetAvailableBatchesForMillAsync()
        {
            // Fetch completed batch IDs from HandiProcess
            var handiCompletedBatchIds = await _context.DryerProcesses
                .Where(hp => hp.ProcessStatus == "Completed")
                .Select(hp => hp.BatchId)
                .ToListAsync();

            // Fetch batch IDs already in Milling Process
            var dryerBatchIds = await _context.MillingProcesses
                .Select(dp => dp.BatchId)
                .ToListAsync();

            // Return batch IDs completed in HandiProcess but not in DryerProcess
            return handiCompletedBatchIds.Except(dryerBatchIds).ToList();
        }


        public async Task StartMillProcessAsync(MillingProcess MillingProcess)
        {
            _context.MillingProcesses.Add(MillingProcess);
            await _context.SaveChangesAsync();
        }




        public async Task<MillingProcess?> GetMillProcessAsync(string batchId)
        {
            return await _context.MillingProcesses.FirstOrDefaultAsync(h => h.BatchId == batchId);
        }





        public async Task PauseProcessAsync(string batchId, string pauseReason)
        {
            var process = await _context.MillingProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                process.ProcessStatus = "Paused";
                process.PauseReason = pauseReason;
                process.PauseTime = DateTime.Now;

                _context.MillingProcesses.Update(process);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResumeProcessAsync(string batchId)
        {


            var process = await _context.MillingProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                process.ProcessStatus = "In Progress";
                process.ResumeTime = DateTime.Now;

                // Accumulate the current pause duration into TotalDelayTime
                if (process.PauseTime.HasValue)
                {
                    var currentDelay = process.ResumeTime.Value - process.PauseTime.Value;

                    // Add the current delay to the total
                    process.TotalDelayTime = (process.TotalDelayTime ?? TimeSpan.Zero) + currentDelay;

                    // Reset PauseTime and ResumeTime for the next pause-resume cycle
                    process.PauseTime = null;
                    process.ResumeTime = null;
                }

                _context.MillingProcesses.Update(process);
                await _context.SaveChangesAsync();
            }

        }



        public async Task EndProcessAsync(string batchId , string SortexBunker)
        {
            var process = await _context.MillingProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                // Check if the process is paused
                if (process.ProcessStatus == "Paused" && process.PauseTime.HasValue)
                {
                    // Calculate delay as the difference between EndTime (current time) and PauseTime
                    var additionalDelay = DateTime.Now - process.PauseTime.Value;

                    // Add the additional delay to the total delay time
                    process.TotalDelayTime = (process.TotalDelayTime ?? TimeSpan.Zero) + additionalDelay;

                    // Reset PauseTime since the process is ending
                    process.PauseTime = null;
                }

                // Update the end details after handling pause-related calculations
                process.EndTime = DateTime.Now;
                process.SortexBunkerName = SortexBunker;
                process.ProcessStatus = "Completed";


                // Fetch the selected bunker and update its status to "OCCUPIED"
                var bunker = await _context.SortexBunkers.FirstOrDefaultAsync(b => b.SortexBName == SortexBunker);
                if (bunker != null)
                {
                    bunker.Status = "OCCUPIED"; // Mark the bunker as occupied
                    _context.SortexBunkers.Update(bunker); // Update the bunker status
                }

                _context.MillingProcesses.Update(process);
                await _context.SaveChangesAsync();
            }

       
        }

        public async Task<MillingProcess> GetActiveProcessAsync()
        {
      

            return await _context.MillingProcesses
        .Where(h => h.ProcessStatus == "In Progress" || h.ProcessStatus == "Paused")
        .OrderByDescending(h => h.StartTime) // Optionally, get the most recent active process
        .FirstOrDefaultAsync();
        }







        // Method to fetch occupied bunkers
        public async Task<List<SelectListItem>> GetOccupiedBunkersAsync()
        {
            var bunkers = await _context.MillBunkers
                .Where(b => b.Status == "OCCUPIED")
                .Select(b => new SelectListItem
                {
                    Value = b.MillBName, // Assuming Name is what you want to display
                    Text = b.MillBName
                })
                .ToListAsync();

            return bunkers;
        }

        // Method to fetch completed batches from DryerProcess based on the selected occupied bunker
        public async Task<List<SelectListItem>> GetBatchesForOccupiedBunkerAsync(string occupiedBunkerName)
        {
            //var batches = await _context.DryerProcesses
            //    .Where(dp => dp.ProcessStatus == "Completed" && dp.UnloadBunkerName == occupiedBunkerName)
            //    .Select(dp => new SelectListItem
            //    {
            //        Value = dp.BatchId,
            //        Text = $"Batch {dp.BatchId}"
            //    })
            //    .ToListAsync();

            //return batches;

            var batches = await _context.DryerProcesses
    .Where(dp => dp.ProcessStatus == "Completed"
                && dp.UnloadBunkerName == occupiedBunkerName
                && !_context.MillingProcesses.Any(mp => mp.BatchId == dp.BatchId))  // Exclude batches present in MillingProcess
    .Select(dp => new SelectListItem
    {
        Value = dp.BatchId.ToString(),
        Text = $"Batch {dp.BatchId}"
    })
    .ToListAsync();

            return batches;
        }





        // Method to get a bunker by its name
        public async Task<MillBunker> GetBunkerByNameAsync(string bunkerName)
        {
            // Fetch the bunker with the given name
            return await _context.MillBunkers
                                 .FirstOrDefaultAsync(b => b.MillBName == bunkerName);
        }

        // Method to update the status of the bunker
        public async Task UpdateBunkerStatusAsync(MillBunker bunker)
        {
            // Update the bunker status and save changes
            _context.MillBunkers.Update(bunker);
            await _context.SaveChangesAsync();
        }





        public async Task<bool> IsAnyBunkerSortexEmptyAsync()
        {
            // Check if any bunker has the status "EMPTY"
            return await _context.SortexBunkers.AnyAsync(bunker => bunker.Status == "EMPTY");
        }

        public async Task<List<string>> GetEmptySortexBunkersAsync()
        {
            return await _context.SortexBunkers
                .Where(b => b.Status == "EMPTY")
                .Select(b => b.SortexBName)
                .ToListAsync();
        }




     


    }
}
