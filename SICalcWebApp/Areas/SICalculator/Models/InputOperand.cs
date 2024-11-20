using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SICalcWebApp.Areas.SICalculator.Models
{
    public class InputOperand
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        public string? Sidename { get; set; }

        public int IronTypeId { get; set; }

        [ForeignKey("IronTypeId")]
        public IronType? IronType { get; set; }
        [Required]
        public decimal FeT { get; set; }
        [Required]
        public decimal Moisture { get; set; }
        [Required]
        public decimal Gangue { get; set; }
        [Required]
        public decimal Phos { get; set; }
        [Required]
        public decimal Yield { get; set; }
        [Required]
        public decimal IronPrice { get; set; }
        [Required]
        public decimal GroundLoss { get; set; }
        [Required]
        public decimal FineLoss { get; set; }

        [Required]
        public decimal FinesRealisationValue { get; set; }
        [Required]
        public decimal FeMSponge { get; set; }
    
        [Required]
        public decimal TransferLoss { get; set; }
        [Required]
        public decimal YLoss { get; set; }

    }
}
