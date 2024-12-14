using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class DryerProcess
    {

        public int DryerProcessId { get; set; }

        [Required(ErrorMessage = "Batch ID is required.")]
        public string BatchId { get; set; }  // The Batch ID will be selected from Handi Process (completed processes)


        public DateTime? LoadTime { get; set; } // Current time (already set)

        [Required(ErrorMessage = "Ducti Pressure is required.")]
        public double? DuctiPressure { get; set; }

   
        public DateTime? UnloadTime { get; set; }

        [Required(ErrorMessage = "Unload Bunker Name is required.")]
        public string UnloadBunkerName { get; set; } // Dropdown from Master Data

        [Required(ErrorMessage = "Staff Name is required.")]
        public string StaffName { get; set; } // Dropdown from Master Data

        public string? ProcessStatus { get; set; } // Track process status (e.g., "In Progress", "Completed")

        // Tracking delay times
        public string? PauseReason { get; set; } // Nullable, used if paused
        public DateTime? PauseTime { get; set; } // Track when the process was paused
        public DateTime? ResumeTime { get; set; } // Track when the process was resumed

        public TimeSpan? TotalDelayTime { get; set; }

        // Computed property to calculate total delay time
        public TimeSpan? CalculatedDelayTime =>
             PauseTime.HasValue && ResumeTime.HasValue
             ? ResumeTime - PauseTime
             : null;

    }
}
