using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Areas.SICalculator.VM;

namespace SICalcWebApp.Repository
{
    public interface IInputOperandsService
    {
        Task<IEnumerable<InputOperand>> GetAllInputOperandsAsync();
        Task AddInputOperandsAsync(InputOperand inputOperands);

        Task UpdateInputOperandAsync(InputOperand inputOperands);
        Task DeleteInputOperandAsync(int id);
        Task<InputOperand> GetInputOperandByIdAsync(int id);
    }
}
