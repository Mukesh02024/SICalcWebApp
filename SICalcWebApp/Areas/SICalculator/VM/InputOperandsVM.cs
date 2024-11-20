using SICalcWebApp.Areas.SICalculator.Models;

namespace SICalcWebApp.Areas.SICalculator.VM
{
    public class InputOperandsVM
    {
        public InputOperand ?InputOperandss { get; set; }
        public IEnumerable<IronType>? IronTypes
        {
            get; set;
        }
    }
}
