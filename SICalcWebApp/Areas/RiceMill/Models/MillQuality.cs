using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SICalcWebApp.Areas.RiceMill.Models
{
    public class MillQuality
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string BatchID { get; set; }

        [Required]
        [MaxLength(2)]
        public string Stage { get; set; } // S1, S2, S3, S4

        // Machine & Manual Values
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Machine_Damage { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal ?Manual_Damage { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal ?Machine_Discolour { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Manual_Discolour { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Machine_Broken { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Manual_Broken { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Machine_FRK { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Manual_FRK { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Machine_Moisture { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Manual_Moisture { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Moisture_Chotti_Machine { get; set; }


        [Column(TypeName = "decimal(5,2)")]
        public decimal Moisture_Chotti_Machine_Manual { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Mill_Wightment { get; set; }

        public DateTime ReportedAt { get; set; } = DateTime.Now;


        public string? Other1{ get; set; }
   
        public string? Other2 { get; set; }
 

        public string? Other3{ get; set; }
    }
}
