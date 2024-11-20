using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.SICalculator.Models
{
    public class FCInfo
    {


        public int FCInfoId { get; set; }
        [Required] // Check if this is causing the issue
        [Display(Name = "FC")]
        public int FCId { get; set; }
        public FC? FC { get; set; }

        [Required] // Check if this is causing the issue
        [Display(Name = "Klin")]
        public int TPDId { get; set; }
        public TPDInfo? TPDInfo { get; set; }
        public decimal FeedRate { get; set; }
    }
}
