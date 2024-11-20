using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.SICalculator.VM
{
    public class MaterialInputViewModel
    {

        public string MaterialName { get; set; }

        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
        public double UsagePercentage { get; set; }
    }
}
