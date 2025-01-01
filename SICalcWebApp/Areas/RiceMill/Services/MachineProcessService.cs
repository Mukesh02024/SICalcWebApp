using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class MachineProcessService:IMachineProcessService
    {

        private readonly ApplicationDbContext _context;

        public MachineProcessService(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task StartHandiProcessAsync(HandiProcess handiProcess)
        {
            _context.HandiProcesses.Add(handiProcess);
            await _context.SaveChangesAsync();
        }



        public async Task<HandiProcess?> GetHandiProcessAsync(string batchId)
        {
            return await _context.HandiProcesses.FirstOrDefaultAsync(h => h.BatchId == batchId);
        }

        public async Task UpdateHandiProcessAsync(HandiProcess handiProcess)
        {
            _context.HandiProcesses.Update(handiProcess);
            await _context.SaveChangesAsync();
        }




        public async Task<HandiProcess> GetActiveProcessAsync()
        {
            //// Find the process with "In Progress" status
            //return await _context.HandiProcesses
            //    .Where(h => h.ProcessStatus == "In Progress")
            //    .OrderByDescending(h => h.StartTime) // Optionally, get the most recent active process
            //    .FirstOrDefaultAsync();

            return await _context.HandiProcesses
        .Where(h => h.ProcessStatus == "In Progress" || h.ProcessStatus == "Paused")
        .OrderByDescending(h => h.StartTime) // Optionally, get the most recent active process
        .FirstOrDefaultAsync();
        }








        public async Task PauseProcessAsync(string batchId, string pauseReason)
        {
            var process = await _context.HandiProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            if (process != null)
            {
                process.ProcessStatus = "Paused";
                process.PauseReason = pauseReason;
                process.PauseTime = DateTime.Now;
      
                _context.HandiProcesses.Update(process);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResumeProcessAsync(string batchId)
        {
            //var process = await _context.HandiProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
            //if (process != null)
            //{
            //    process.ProcessStatus = "In Progress";
            //    process.ResumeTime = DateTime.Now;

            //    // Recalculate and update the TotalDelayTime when resumed
            //    if (process.PauseTime.HasValue && process.ResumeTime.HasValue)
            //    {
            //        process.TotalDelayTime = (process.ResumeTime.Value - process.PauseTime.Value);
            //    }
            //    await _context.SaveChangesAsync();
            //}

            var process = await _context.HandiProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
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

                _context.HandiProcesses.Update(process);
                await _context.SaveChangesAsync();
            }



        }

        public async Task EndProcessAsync(string batchId)
        {
            var process = await _context.HandiProcesses.FirstOrDefaultAsync(p => p.BatchId == batchId);
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
                process.ProcessStatus = "Completed";

                _context.HandiProcesses.Update(process);
                await _context.SaveChangesAsync();
            }



        }



        // Fetch master data for dropdowns
        public async Task<MasterDataViewModel> GetMasterDataAsync()
        {
            var processTypes = await _context.ProcessMethods
             .Select(pt => pt.MethodName) // Fetch text values directly
                .ToListAsync();

            var paddyTypes = await _context.PaddyTypes
                .Select(pt => pt.PaddyTypeName)
                .ToListAsync();

            var handiTypes = await _context.TypeOfHandis
                .Select(ht => ht.TypeHandiName)
                .ToListAsync();

            var staffNames = await _context.Staffs
                .Select(s => s.StaffName)
                .ToListAsync();

            var MillBunkers = await _context.MillBunkers
              .Select(s => s.MillBName)
              .ToListAsync();

            var SortexBunkers = await _context.SortexBunkers
              .Select(s => s.SortexBName)
              .ToListAsync();

            return new MasterDataViewModel
            {
                ProcessTypes = processTypes, // Update to plain lists
                PaddyTypes = paddyTypes,
                HandiTypes = handiTypes,
                StaffNames = staffNames,
                MillBunkers= MillBunkers,
                SortexBunker=SortexBunkers

            };




            //var processTypes = await _context.ProcessMethods
            //    .Select(pt => new SelectListItem
            //    {
            //        Value = pt.Id.ToString(),
            //        Text = pt.MethodName,
            //    }).ToListAsync();

            //var paddyTypes = await _context.PaddyTypes
            //    .Select(pt => new SelectListItem
            //    {
            //        Value = pt.Id.ToString(),
            //        Text = pt.PaddyTypeName
            //    }).ToListAsync();

            //var handiTypes = await _context.TypeOfHandis
            //    .Select(ht => new SelectListItem
            //    {
            //        Value = ht.Id.ToString(),
            //        Text = ht.TypeHandiName
            //    }).ToListAsync();

            //var staffNames = await _context.Staffs
            //    .Select(s => new SelectListItem
            //    {
            //        Value = s.Id.ToString(),
            //        Text = s.StaffName
            //    }).ToListAsync();

            //return new MasterDataViewModel
            //{
            //    ProcessTypes = processTypes,
            //    PaddyTypes = paddyTypes,
            //    HandiTypes = handiTypes,
            //    StaffNames = staffNames
            //};
        }

   
        public int GetNextBatchId()
        {
            var lastBatch = _context.HandiProcesses.OrderByDescending(p => p.HandiProcessId).FirstOrDefault();

            int nextBatchId = 1000; // Default starting value

            if (lastBatch != null)
            {
                // Extract the numeric part from the BatchId (after 'RICE-')
                var lastBatchNumber = lastBatch.BatchId.Replace("RICE-", "");

                if (int.TryParse(lastBatchNumber, out int batchNumber))
                {
                    nextBatchId = batchNumber + 1;
                }
            }

            return nextBatchId;
        }


        public async Task<bool> IsDryerFreeAsync()
        {
            // Check if there are any batches in the dryer that are "Paused" or "In Progress"
            var ongoingBatches = await _context.DryerProcesses
                .Where(p => p.ProcessStatus == "Paused" || p.ProcessStatus == "In Progress")
                .ToListAsync();

            return !ongoingBatches.Any();  // Return true if no ongoing batches, meaning dryer is free
        }



        public async Task<bool> AreAllCompletedBatchesInDryerProcessAsync()
        {
            // Step 1: Get all the completed batches from the Handi process table
            var completedBatches = await _context.HandiProcesses
                .Where(p => p.ProcessStatus == "Completed") // Get all completed batches
                .Select(p => p.BatchId)  // Select only BatchIds
                .ToListAsync();

            // Step 2: Check if all the completed batches from Handi are present in the Dryer process table
            foreach (var batchId in completedBatches)
            {
                var batchInDryerProcess = await _context.DryerProcesses
                    .Where(d => d.BatchId == batchId)  // Check if the batch is in the Dryer process
                    .FirstOrDefaultAsync();

                if (batchInDryerProcess == null)
                {
                    // If any batch is missing, return false
                    return false;
                }
            }

            // Step 3: If all completed batches are found in the Dryer process table, return true
            return true;
        }



    }
}
