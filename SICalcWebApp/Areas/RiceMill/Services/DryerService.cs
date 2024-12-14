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
                .Where(hp => hp.ProcessStatus == "Completed")
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






        public async Task PauseProcessAsync(string batchId, string pauseReason)
        {
            var process = await _context.DryerProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                process.ProcessStatus = "Paused";
                process.PauseReason = pauseReason;
                process.PauseTime = DateTime.Now;

                _context.DryerProcesses.Update(process);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResumeProcessAsync(string batchId)
        {
    

            var process = await _context.DryerProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
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

                _context.DryerProcesses.Update(process);
                await _context.SaveChangesAsync();
            }

        }

        public async Task EndProcessAsync(string batchId)
        {
            var process = await _context.DryerProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                process.UnloadTime = DateTime.Now;
                process.ProcessStatus = "Completed";
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




    }
}
