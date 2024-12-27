using Microsoft.AspNetCore.Mvc.Rendering;
using SICalcWebApp.Areas.RiceMill.Models;
using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.RiceMill.VM
{
    public class HmaliSearchViewModel
    {
        [Required(ErrorMessage = "From Date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "From Date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }


        public int? GroupId { get; set; }
        public int? ItemNumber { get; set; } // Optional
        public List<SelectListItem> Groups { get; set; }
        public List<SelectListItem> Items { get; set; }
        public List<HmaliInput> SearchResults { get; set; } // List of search results

        public decimal GrandTotal { get; set; } // Grand Total of all Total Costs
                                                // Added Dictionary to store GroupName and Total Costs
                                                //public Dictionary<string, decimal> GroupedSummary { get; set; }

        public decimal GrandTotalQuantity { get; set; }

        public Dictionary<string, Dictionary<string, decimal>> GroupedSummary { get; set; }

        //public Dictionary<string, decimal> GroupedQuantities { get; set; } // Add this property for quantity sums

        public Dictionary<string, Dictionary<string, decimal>> GroupedQuantities { get; set; }  // Ensure it's this type


    }
}
