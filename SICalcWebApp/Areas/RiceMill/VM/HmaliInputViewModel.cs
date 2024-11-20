namespace SICalcWebApp.Areas.RiceMill.VM
{
    public class HmaliInputViewModel
    {
        public int HmaliId { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime EntryDate { get; set; }

        // This is for display purposes only
        public int GroupId { get; set; }

        public string GroupName { get; set; }
    }
}
