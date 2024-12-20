using Microsoft.AspNetCore.Mvc.Rendering;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class MasterDataViewModel
    {
        //public List<SelectListItem> ProcessTypes { get; set; }
        //public List<SelectListItem> PaddyTypes { get; set; }
        //public List<SelectListItem> HandiTypes { get; set; }
        //public List<SelectListItem> StaffNames { get; set; }




        public List<string> ProcessTypes { get; set; }
        public List<string> PaddyTypes { get; set; }
        public List<string> HandiTypes { get; set; }
        public List<string> StaffNames { get; set; }
        public List<string> MillBunkers { get; set; }

        public List<string> SortexBunker { get; set; }
    }
}
