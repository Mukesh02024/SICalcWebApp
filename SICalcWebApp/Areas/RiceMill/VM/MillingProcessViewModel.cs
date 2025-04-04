using Microsoft.AspNetCore.Mvc.Rendering;

namespace SICalcWebApp.Areas.RiceMill.VM
{
    public class MillingProcessViewModel
    {
        public string MillBunkerName { get; set; }
        public string BatchId { get; set; }
        public string StaffName { get; set; }
        public string SortexBunkerName { get; set; } // Stores the selected Sortex bunker


        public string SaleType { get; set; }
        public DateTime? StartTime { get; set; } // Current time (already set)
        public List<SelectListItem> Bunkers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Batches { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Staffs { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> SortexBunkerList { get; set; } = new List<SelectListItem>(); // Stores dropdown options

    }
}
