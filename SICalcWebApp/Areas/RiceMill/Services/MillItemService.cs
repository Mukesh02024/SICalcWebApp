using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class MillItemService:IMillItemService
    {
        private readonly ApplicationDbContext _context;

        public MillItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateMillItemAsync(MillItem millItem)
        {
            _context.MillItems.Add(millItem);
            await _context.SaveChangesAsync();
        }



        public async Task<IEnumerable<MillItem>> GetAllMillItemsAsync()
        {
            return await _context.MillItems.Include(m => m.GroupMill).ToListAsync();
        }

        public async Task<MillItem> GetMillItemByIdAsync(int id)
        {
            return await _context.MillItems.Include(m => m.GroupMill)
                                           .FirstOrDefaultAsync(m => m.ItemId == id);
        }



        public async Task UpdateMillItemAsync(MillItem millItem)
        {
            _context.MillItems.Update(millItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMillItemAsync(int id)
        {
            var millItem = await GetMillItemByIdAsync(id);
            if (millItem != null)
            {
                _context.MillItems.Remove(millItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
