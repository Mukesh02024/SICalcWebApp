using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class PaddyType
    {

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string PaddyTypeName { get; set; }
    }
}
