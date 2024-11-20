using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class HmaliInput
    {

        public HmaliInput()
        {
            EntryDate = DateTime.Now; // Set default to current date
        }
        public int HmaliId { get; set; }
        public int GroupId { get; set; }
        public int ItemNumber { get; set; }
        [NotMapped]
        public string ?GroupName { get; set; } // Newly added property

        public string ItemName { get; set; }  // Read-only textbox in UI
        public decimal Rate { get; set; }     // Read-only textbox in UI
        public decimal Capacity { get; set; } // Read-only textbox in UI
        public int Quantity { get; set; }     // Input from user

        // No need to store TotalValue in the database
        [NotMapped] // This attribute tells EF not to map this property to the database
        public decimal TotalValue { get; set; } // Automatically calculated (Rate * Quantity)


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EntryDate { get; set; } // Input from user
    }
}
