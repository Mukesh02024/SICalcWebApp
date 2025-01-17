using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class DryerService:IDryerService
    {
        private readonly ApplicationDbContext _context;
        public DryerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetAvailableBatchesForDryerAsync()
        {
            // Fetch completed batch IDs from HandiProcess
            var handiCompletedBatchIds = await _context.HandiProcesses
                .Where(hp => hp.ProcessStatus == "Completed" && hp.ProcessType =="USNA")
                .Select(hp => hp.BatchId)
                .ToListAsync();

            // Fetch batch IDs already in DryerProcess
            var dryerBatchIds = await _context.DryerProcesses
                .Select(dp => dp.BatchId)
                .ToListAsync();

            // Return batch IDs completed in HandiProcess but not in DryerProcess
            return handiCompletedBatchIds.Except(dryerBatchIds).ToList();
        }



        public async Task StartDryerProcessAsync(DryerProcess DryerProcess)
        {
            _context.DryerProcesses.Add(DryerProcess);
            await _context.SaveChangesAsync();
        }



        public async Task<DryerProcess?> GetDryerProcessAsync(string batchId)
        {
            return await _context.DryerProcesses.FirstOrDefaultAsync(h => h.BatchId == batchId);
        }






        public async Task PauseProcessAsync(string batchId, string pauseReason, DateTime? pauseTime)
        {
            var process = await _context.DryerProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                process.ProcessStatus = "Paused";
                process.PauseReason = pauseReason;
                process.PauseTime = pauseTime;

                _context.DryerProcesses.Update(process);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResumeProcessAsync(string batchId, DateTime? resumeTime)
        {
    

            var process = await _context.DryerProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                process.ProcessStatus = "In Progress";
                process.ResumeTime = resumeTime;

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

                _context.DryerProcesses.Update(process);
                await _context.SaveChangesAsync();
            }

        }

    
        public async Task EndProcessAsync(string batchId, string UnloadBunkers, DateTime? UnloadTime)
        {
            // Fetch the DryerProcess based on the BatchId
            var process = await _context.DryerProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                // Check if the process is paused and handle delay
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
                process.UnloadTime = UnloadTime;
                process.UnloadBunkerName = UnloadBunkers;
                process.ProcessStatus = "Completed";

                // Fetch the selected bunker and update its status to "OCCUPIED"
                var bunker = await _context.MillBunkers.FirstOrDefaultAsync(b => b.MillBName == UnloadBunkers);
                if (bunker != null)
                {
                    bunker.Status = "OCCUPIED"; // Mark the bunker as occupied
                    _context.MillBunkers.Update(bunker); // Update the bunker status
                }

                // Update the DryerProcess in the database
                _context.DryerProcesses.Update(process);

                // Save all changes to the database
                await _context.SaveChangesAsync();
            }
        }



        public async Task<DryerProcess> GetActiveProcessAsync()
        {
            //// Find the process with "In Progress" status
            //return await _context.HandiProcesses
            //    .Where(h => h.ProcessStatus == "In Progress")
            //    .OrderByDescending(h => h.StartTime) // Optionally, get the most recent active process
            //    .FirstOrDefaultAsync();

            return await _context.DryerProcesses
        .Where(h => h.ProcessStatus == "In Progress" || h.ProcessStatus == "Paused")
        .OrderByDescending(h => h.LoadTime) // Optionally, get the most recent active process
        .FirstOrDefaultAsync();
        }


        public async Task<bool> IsAnyBunkerEmptyAsync()
        {
            var emptyBunkers = await _context.MillBunkers
                .Where(b => b.Status == "EMPTY")
                .ToListAsync();

            return emptyBunkers.Any();
        }

        public async Task<List<string>> GetEmptyBunkersAsync()
        {
            return await _context.MillBunkers
                .Where(b => b.Status == "EMPTY")
                .Select(b => b.MillBName)
                .ToListAsync();
        }

    }
}
