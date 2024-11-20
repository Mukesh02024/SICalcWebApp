namespace SICalcWebApp.Areas.SICalculator.Models
{
    public class FC
    {
        public int FCId { get; set; }
        public int FCValue { get; set; }
        public decimal C_Fe { get; set; }

        public ICollection<FCInfo>? FCInfos { get; set; }
    }
}
