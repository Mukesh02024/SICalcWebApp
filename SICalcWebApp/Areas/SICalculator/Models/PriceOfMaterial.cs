using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.SICalculator.Models
{
    public class PriceOfMaterial
    {
        [Key]
        public int PriceInputId { get; set; }
        public decimal DolomiteRate {  get; set; }

        public decimal MgfExpence { get; set; }

        public decimal FixedCost { get; set; }

    }
}
