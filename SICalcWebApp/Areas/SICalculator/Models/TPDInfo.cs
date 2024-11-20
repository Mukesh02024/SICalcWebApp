using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.SICalculator.Models
{
    public class TPDInfo
    {
        public int TPDId { get; set; }
        [Required(ErrorMessage = "The TPD field is required.")]
        public int TPD { get; set; }
        public string KilnName { get; set; }
        public ICollection<FCInfo>? FCInfos { get; set; }
    }
}
