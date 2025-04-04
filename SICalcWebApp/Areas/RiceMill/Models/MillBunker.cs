namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class MillBunker
    {
        public int Id { get; set; }
        public string MillBName { get; set; }

        // Make Status nullable or use a default value when adding
        public string Status { get; set; } = "EMPTY";  // Default value when creating new records

        // Set LastUpdated to nullable if you want to handle it automatically
        public DateTime LastUpdated { get; set; } = DateTime.Now;  // Default value set when adding a new record

        public string? BatchId { get; set; }  // Nullable to allow for empty state
    }
}
