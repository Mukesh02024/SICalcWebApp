namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class FeedingModuleF
    {

        public int Id { get; set; }

        public string FeedingBunkerName { get; set; } // Store the name as a string

        public int NumberOfBags { get; set; }

        public string PaddyTypeName { get; set; } // Store the type of paddy as a string

        public string StaffName { get; set; } // Store the staff name as a string

        public DateTime FeedingDate { get; set; } = DateTime.Now; // Default to today's date
    }
}
