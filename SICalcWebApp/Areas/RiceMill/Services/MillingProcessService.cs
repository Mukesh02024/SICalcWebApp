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


    }
}
