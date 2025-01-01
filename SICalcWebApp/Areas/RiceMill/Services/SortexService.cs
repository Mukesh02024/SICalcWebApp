using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class SortexService:ISortexService
    {
        private readonly ApplicationDbContext _context;
        public SortexService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetAvailableBatchesForSortexAsync()
        {
            // Fetch completed batch IDs from HandiProcess
            var handiCompletedBatchIds = await _context.MillingProcesses
                .Where(hp => hp.ProcessStatus == "Completed")
                .Select(hp => hp.BatchId)
                .ToListAsync();

            // Fetch batch IDs already in Milling Process
            var dryerBatchIds = await _context.SortexProcesses
                .Select(dp => dp.BatchId)
                .ToListAsync();

            // Return batch IDs completed in HandiProcess but not in DryerProcess
            return handiCompletedBatchIds.Except(dryerBatchIds).ToList();
        }
        public async Task StartSortexProcessAsync(SortexProcess process)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Add the Sortex process to the database
                _context.SortexProcesses.Add(process);
                await _context.SaveChangesAsync();

                // Update the related Sortex bunker's status
                var bunker = await _context.SortexBunkers.FirstOrDefaultAsync(sb => sb.SortexBName == process.SortexBunkerName);
                if (bunker != null)
                {
                    bunker.Status = "EMPTY";
                    await _context.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Rollback transaction in case of error
                await transaction.RollbackAsync();
                throw;
            }
        }


        //public async Task StartSortexProcessAsync(SortexProcess sortexProcess)
        //{
        //    _context.SortexProcesses.Add(sortexProcess);
        //    await _context.SaveChangesAsync();
        //}




        public async Task<SortexProcess?> GetSortexProcessAsync(string batchId)
        {
            return await _context.SortexProcesses.FirstOrDefaultAsync(h => h.BatchId == batchId);
        }





        public async Task PauseProcessAsync(string batchId, string pauseReason)
        {
            var process = await _context.SortexProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                process.ProcessStatus = "Paused";
                process.PauseReason = pauseReason;
                process.PauseTime = DateTime.Now;

                _context.SortexProcesses.Update(process);
                await _context.SaveChangesAsync();
            }
        }
        public async Task ResumeProcessAsync(string batchId)
        {


            var process = await _context.SortexProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
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

                _context.SortexProcesses.Update(process);
                await _context.SaveChangesAsync();
            }

        }



        public async Task EndProcessAsync(string batchId)
        {
            var process = await _context.SortexProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
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
                process.ProcessStatus = "Completed";

                _context.SortexProcesses.Update(process);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SortexProcess> GetActiveProcessAsync()
        {
            //// Find the process with "In Progress" status
            //return await _context.HandiProcesses
            //    .Where(h => h.ProcessStatus == "In Progress")
            //    .OrderByDescending(h => h.StartTime) // Optionally, get the most recent active process
            //    .FirstOrDefaultAsync();

            return await _context.SortexProcesses
        .Where(h => h.ProcessStatus == "In Progress" || h.ProcessStatus == "Paused")
        .OrderByDescending(h => h.StartTime) // Optionally, get the most recent active process
        .FirstOrDefaultAsync();
        }




        public async Task<List<string>> GetBatchIdsForSortexAsync(string sortexBunkerName)
        {
            var completedBatchIds = await _context.MillingProcesses
                .Where(mp => mp.SortexBunkerName == sortexBunkerName && mp.ProcessStatus == "Completed")
                .Select(mp => mp.BatchId)
                .ToListAsync();

            var existingBatchIds = await _context.SortexProcesses
                .Select(sp => sp.BatchId)
                .ToListAsync();

            return completedBatchIds.Except(existingBatchIds).ToList();
        }

        public async Task<List<string>> GetOccupiedSortexBunkersAsync()
        {
            return await _context.SortexBunkers
                .Where(sb => sb.Status == "OCCUPIED")
                .Select(sb => sb.SortexBName)
                .ToListAsync();
        }




    }
}
