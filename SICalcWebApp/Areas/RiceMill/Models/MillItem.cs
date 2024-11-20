namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class MillItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemNumber { get; set; }
        public decimal Rate { get; set; }
        public decimal Capacity { get; set; }
        public int GroupId { get; set; }
        public GroupMill? GroupMill { get; set; }
    }
}
