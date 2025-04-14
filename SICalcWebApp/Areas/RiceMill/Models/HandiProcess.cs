using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class HandiProcess: IValidatableObject
    {
        public int HandiProcessId { get; set; }
        public string ?BatchId { get; set; }
        public string ProcessType { get; set; }
        public string PaddyType { get; set; }
        public string? HandiType { get; set; }


        public string? WaterType{ get; set; }
      
        public double? Pressure { get; set; }

        [Required(ErrorMessage = "Paddy Moisture is required.")]
        public double ?PaddyMoisture { get; set; } 

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
        // Handi Run Count (this will be the default to 8 if ProcessType is not "ARWA")

        [BindRequired]
        [Range(0.1, double.MaxValue, ErrorMessage = "PaddyWeight must be at least 0.1.")]
        public decimal? PaddyWeight { get; set; }

        // Unload Bunker Name (only visible if ProcessType is "ARWA")
        public string? UnloadBunkerName { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProcessType == "USNA")
            {
                if (string.IsNullOrWhiteSpace(WaterType))
                {
                    yield return new ValidationResult("WaterType is required.", new[] { nameof(WaterType) });
                }

                if (Pressure == null)
                {
                    yield return new ValidationResult("Pressure is required.", new[] { nameof(Pressure) });
                }

                if (PaddyMoisture == null)
                {
                    yield return new ValidationResult("Paddy Moisture is required.", new[] { nameof(PaddyMoisture) });
                }
            }
        }




    }
}
