using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Repository
{
    public class IronTypeService:IIronTypeService
    {
        private readonly ApplicationDbContext _context;

        public IronTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddIronTypeAsync(IronType ironType)
        {
            _context.IronTypes.Add(ironType);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<IronType>> GetAllIronTypesAsync()
        {
            return await _context.IronTypes.ToListAsync();
        }




        public async Task UpdateIronTypeAsync(IronType ironType)
        {
            var existingIronType = await _context.IronTypes.FindAsync(ironType.Id);
            if (existingIronType != null)
            {
                existingIronType.IronTypeName = ironType.IronTypeName;
                // Update other properties as needed
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Iron type not found.");
            }
        }

        public async Task DeleteIronTypeAsync(int ironTypeId)
        {
            var ironType = await _context.IronTypes.FindAsync(ironTypeId);
            if (ironType != null)
            {
                _context.IronTypes.Remove(ironType);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Iron type not found.");
            }
        }
    }
}
