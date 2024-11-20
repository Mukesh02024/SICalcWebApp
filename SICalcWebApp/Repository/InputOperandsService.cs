using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Areas.SICalculator.VM;
using SICalcWebApp.Data;

namespace SICalcWebApp.Repository
{
    public class InputOperandsService:IInputOperandsService
    {
        private readonly ApplicationDbContext _context;

        public InputOperandsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InputOperand>> GetAllInputOperandsAsync()
        {
            return await _context.InputOperands.Include(io => io.IronType).ToListAsync();
        }

        public async Task AddInputOperandsAsync(InputOperand inputOperands)
        {
            _context.InputOperands.Add(inputOperands);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateInputOperandAsync(InputOperand inputOperands)
        {
            _context.InputOperands.Update(inputOperands);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInputOperandAsync(int id)
        {
            var inputOperand = await _context.InputOperands.FindAsync(id);
            if (inputOperand != null)
            {
                _context.InputOperands.Remove(inputOperand);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<InputOperand> GetInputOperandByIdAsync(int id)  // Implementation of the new method
        {
            return await _context.InputOperands.Include(io => io.IronType)
                                               .FirstOrDefaultAsync(io => io.ProductID == id);
        }

    }
}
