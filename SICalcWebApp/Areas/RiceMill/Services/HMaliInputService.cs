using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.VM;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class HMaliInputService :IHMaliInputService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public HMaliInputService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<GroupMill>> GetGroupsAsync()
        {
            //return await _context.GroupMills.ToListAsync();
            if (!_cache.TryGetValue("GroupMillsCache", out IEnumerable<GroupMill> groups))
            {
                groups = await _context.GroupMills.ToListAsync();
                _cache.Set("GroupMillsCache", groups, TimeSpan.FromMinutes(30)); // Cache for 30 minutes
            }

            return groups;
        }

        public async Task<IEnumerable<MillItem>> GetItemsByGroupAsync(int groupId)
        {
            return await _context.MillItems
                .Where(i => i.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<HmaliInput> CreateHmaliInputAsync(HmaliInput hmaliInput)
        {
            _context.HmaliInputs.Add(hmaliInput);
            await _context.SaveChangesAsync();
            return hmaliInput;
        }

        public async Task<MillItem> GetItemDetailsAsync(int itemNumber)
        { // Convert itemNumber to string
            string itemNumberStr = itemNumber.ToString();

            //return await _context.MillItems
            //    .FirstOrDefaultAsync(i => i.ItemNumber == itemNumberStr);

            var item = await _context.MillItems.FirstOrDefaultAsync(i => i.ItemNumber== itemNumberStr);

            return item;  // Return the result, could be null if not found
        }



        public async Task<MillItem> GetItemDetailssAsync(int itemNumber)
        { // Convert itemNumber to string
            string itemNumberStr = itemNumber.ToString();

            //return await _context.MillItems
            //    .FirstOrDefaultAsync(i => i.ItemNumber == itemNumberStr);

            var item = await _context.MillItems.FirstOrDefaultAsync(i => i.ItemId == itemNumber);

            return item;  // Return the result, could be null if not found
        }



        public async Task<IEnumerable<HmaliInput>> SearchHmaliInputsAsync(DateTime? fromDate, DateTime? toDate, int? groupId, int? itemId)
        {




            var query = from h in _context.HmaliInputs
                        join g in _context.GroupMills on h.GroupId equals g.GroupId
                        where (!fromDate.HasValue || h.EntryDate >= fromDate.Value)
                           && (!toDate.HasValue || h.EntryDate <= toDate.Value)
                           && (!groupId.HasValue || h.GroupId == groupId.Value)
                           && (!itemId.HasValue || h.ItemNumber == itemId.Value)
                        select new HmaliInput
                        {
                            GroupId = h.GroupId,
                            GroupName = g.GroupName, // Get GroupName from Groups table
                            ItemName = h.ItemName,
                            Rate = h.Rate,
                            Quantity = h.Quantity,
                            Capacity = h.Capacity,
                            EntryDate = h.EntryDate
                        };

            return await query.ToListAsync();
        }

     
        public async Task<HmaliInput> GetHmaliInputByIdAsync(int hmaliId)
        {
            return await _context.HmaliInputs.FirstOrDefaultAsync(h => h.HmaliId == hmaliId);
        }
        public async Task UpdateHmaliInputAsync(HmaliInput hmaliInput)
        {
            _context.HmaliInputs.Update(hmaliInput);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHmaliInputAsync(int hmaliId)
        {
            var hmaliInput = await GetHmaliInputByIdAsync(hmaliId);
            _context.HmaliInputs.Remove(hmaliInput);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<HmaliInputViewModel>> GetAllHmaliInputsAsync()
        {

            var groupDictionary = await _context.GroupMills
    .ToDictionaryAsync(g => g.GroupId, g => g.GroupName);

            var inputs = await _context.HmaliInputs.ToListAsync();
            var viewModels = inputs.Select(input => new HmaliInputViewModel
            {
                HmaliId = input.HmaliId,
                ItemName = input.ItemName,
                Quantity = input.Quantity,
                Rate = input.Rate,
                TotalValue = input.TotalValue,
                EntryDate = input.EntryDate,
                //GroupId = input.GroupId,

                GroupName = groupDictionary.TryGetValue(input.GroupId, out var groupName)
            ? groupName
            : "Unknown"

            }).ToList();

            return viewModels;
        }




    }
}
