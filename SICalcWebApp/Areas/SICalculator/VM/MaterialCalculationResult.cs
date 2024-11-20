using Microsoft.Identity.Client;
using System.Drawing;

namespace SICalcWebApp.Areas.SICalculator.VM
{
    public class MaterialCalculationResult
    {
        public string? MaterialName { get; set; }
        public decimal TotalIssue { get; set; }

        public decimal TotalIssueP { get; set; }
        public decimal RmhIssue { get; set; }

        public decimal NetKlin { get; set; }

        public decimal NetKlinUsesPer { get; set; }
        public decimal TotalNetFeedKlin {  get; set; }
        public decimal TotalIronIssue { get; set; }
        public decimal TotalCostIron { get; set; }

        public decimal TotalNetIronCost {  get; set; }
        public decimal TotalFet {  get; set; }
        public decimal TotalCoalRequired {  get; set; }
        public decimal SConsuCoal {  get; set; }

        public decimal InputPercentage {  get; set; }   
        public decimal TotalCoalCost {  get; set; }
        public decimal Yeald {  get; set; }

        public decimal TotalProductionSponge {  get; set; }

        public decimal ATotalCostIronplusCon {  get; set; }
        
        public decimal BTotalCoalCost { get; set; }

        public decimal GuengeInSpong {  get; set; }

        public decimal PhosInSpong { get; set; }

        public decimal FeoInSpong { get; set; }

        public decimal CoalCost {  get; set; }

        public decimal FemPerinSpong { get; set; }
        public decimal PerProductionOfSpong { get; set; }
        public decimal NetYeildOnSpong { get; set; }
        public decimal TotalLmPro { get; set; }
        public decimal PhosLM { get; set; }
        public decimal DolomiteCost {  get; set; }
        public decimal MgfExp { get; set; }
        public decimal FixedCost {  get; set; }
        public decimal LmCost {  get; set; }
        public decimal SpongeCostABCDE {  get; set; }

        public decimal MgfOther {  get; set; }

        public decimal FixedCostOther { get; set; }

        public int CombinationId { get; set; } // For tracking which combination this result belongs to
    }

}

