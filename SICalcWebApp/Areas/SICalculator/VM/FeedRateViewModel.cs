using SICalcWebApp.Areas.SICalculator.Models;

namespace SICalcWebApp.Areas.SICalculator.VM
{
    public class FeedRateViewModel
    {
        public List<FC> FCs { get; set; }
        public List<TPDInfo> TPDs { get; set; }
        public int SelectedFC { get; set; }
        public List<int> SelectedTPDs { get; set; }

        public Dictionary<string, decimal> MaterialPercentages { get; set; }

        /*AverageFeedRate*/
        public decimal AverageFeedRate { get; set; }
        public decimal Moisture {  get; set; }

        public decimal CoalCost {  get; set; }
        public decimal Cfe { get; set; }
        public int NumberOfRunningKilns { get; set; }


        public bool IsAutoGeneration { get; set; } // New property to distinguish between manual and auto generation
        public List<string> SelectedMaterials { get; set; } // For auto-generation
        public decimal Increment { get; set; } // Increment value for auto-generation
        public List<MaterialCalculationResult> Results { get; set; } // Store results to be displayed in the view
    }
}
