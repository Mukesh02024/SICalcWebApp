using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.SICalculator.Models
{
    public class IronType
    {
        [Key]
        public int Id {  get; set; }
        [Required]
        public string ? IronTypeName { get; set; }

    }
}
