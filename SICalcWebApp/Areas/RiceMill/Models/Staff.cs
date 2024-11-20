using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class Staff
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StaffName { get; set; }
    }
}
