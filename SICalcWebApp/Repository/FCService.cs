using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Repository
{
    public class FCService : IFCService
    {
        private readonly ApplicationDbContext _context;

        public FCService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FC>> GetAllFCsAsync()
        {
            return await _context.FCs.ToListAsync();
        }

        public async Task CreateFCAsync(FC fc)
        {
            _context.Add(fc);
            await _context.SaveChangesAsync();
        }


        public async Task<FC> GetFCByIdAsync(int id)
        {
            return await _context.FCs.FindAsync(id);
        }

        public async Task UpdateFCAsync(FC fc)
        {
            _context.Update(fc);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFCAsync(int id)
        {
            var fc = await _context.FCs.FindAsync(id);
            if (fc != null)
            {
                _context.FCs.Remove(fc);
                await _context.SaveChangesAsync();
            }
        }
    }
}
