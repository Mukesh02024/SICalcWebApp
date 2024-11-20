namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class GroupMill
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        //public ICollection<MillItem>? MillItems { get; set; }

        public ICollection<MillItem> MillItems { get; set; } = new List<MillItem>();


    }
}
