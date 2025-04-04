using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class SortexProcess
    {
        public int SortexProcessId { get; set; }

        [Required(ErrorMessage = "batch Id is required.")]
        public string BatchId { get; set; } // Foreign key to Batch table

        public DateTime? StartTime { get; set; } // Current time (already set)

        public DateTime? EndTime { get; set; }

       
        public decimal? EndWeight { get; set; }

        [Required(ErrorMessage = "Sortex Bunker Name is required.")]
        public string SortexBunkerName { get; set; } // Dropdown from Master Data

        [Required(ErrorMessage = "Staff Name is required.")]
        public string StaffName { get; set; } // Dropdown from Master Data

        public string? ProcessStatus { get; set; } // Track process status (e.g., "In Progress", "Completed")

        // Tracking delay times
        public string? PauseReason { get; set; } // Nullable, used if paused
        public DateTime? PauseTime { get; set; } // Track when the process was paused
        public DateTime? ResumeTime { get; set; } // Track when the process was resumed

        [Required(ErrorMessage = "SaleType is required.")]
        public string SaleType { get; set; } // Dropdown from Master Data
        public TimeSpan? TotalDelayTime { get; set; }

        // Computed property to calculate total delay time
        public TimeSpan? CalculatedDelayTime =>
             PauseTime.HasValue && ResumeTime.HasValue
             ? ResumeTime - PauseTime
             : null;
    }
}
