using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class HandiProcess
    {
        public int HandiProcessId { get; set; }
        public string ?BatchId { get; set; }
        public string ProcessType { get; set; }
        public string PaddyType { get; set; }
        public string HandiType { get; set; }

        [Required(ErrorMessage = "Temperature is required.")]
        public double? Temperature { get; set; }

        [Required(ErrorMessage = "Pressure is required.")]
        public double? Pressure { get; set; }
        public string StaffName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? ProcessStatus { get; set; }
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
