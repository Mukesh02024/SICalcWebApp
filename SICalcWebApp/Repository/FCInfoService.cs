using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Repository
{
    public class FCInfoService:IFCInfoService
    {
        private readonly ApplicationDbContext _context;

        public FCInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FCInfo>> GetAllFCInfosAsync()
        {
            return await _context.FCInfos.Include(f => f.FC).Include(f => f.TPDInfo).ToListAsync();
        }
        public async Task AddFCInfoAsync(FCInfo fCInfo)
        {
            _context.Add(fCInfo);
            await _context.SaveChangesAsync();
        }


        public IEnumerable<FC> GetAllFCs()
        {
            return _context.FCs.ToList();
        }

        public IEnumerable<TPDInfo> GetAllTPDInfos()
        {
            return _context.TPDInfos.AsNoTracking().ToList();
        }


        //new add
        public async Task<IEnumerable<FC>> GetAllFCsAsync()
        {
            return await _context.FCs.ToListAsync();
        }

        public async Task<IEnumerable<TPDInfo>> GetAllTPDInfosAsync()
        {
            return await _context.TPDInfos.ToListAsync();
        }





        public async Task UpdateFCInfoAsync(FCInfo fCInfo)
        {
            _context.FCInfos.Update(fCInfo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFCInfoAsync(int id)
        {
            var fCInfo = await _context.FCInfos.FindAsync(id);
            if (fCInfo != null)
            {
                _context.FCInfos.Remove(fCInfo);
                await _context.SaveChangesAsync();
            }
        }






    }
}
