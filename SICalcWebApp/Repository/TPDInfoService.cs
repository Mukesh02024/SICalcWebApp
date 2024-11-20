using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Repository
{
    public class TPDInfoService : ITPDInfoService
    {
        private readonly ApplicationDbContext _context;

        public TPDInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTPDInfoAsync(TPDInfo tpdInfo)
        {
            _context.Add(tpdInfo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TPDInfo>> GetAllTPDInfosAsync()
        {
            return await _context.TPDInfos.ToListAsync();
        }



        public async Task<TPDInfo> GetTPDInfoByIdAsync(int id)
        {
            return await _context.TPDInfos.FindAsync(id);
        }

        public async Task UpdateTPDInfoAsync(TPDInfo tpdInfo)
        {
            _context.Update(tpdInfo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTPDInfoAsync(int id)
        {
            var tpdInfo = await _context.TPDInfos.FindAsync(id);
            if (tpdInfo != null)
            {
                _context.TPDInfos.Remove(tpdInfo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
