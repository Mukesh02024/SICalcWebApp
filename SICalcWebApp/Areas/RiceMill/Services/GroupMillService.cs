using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Data;
using SICalcWebApp.Migrations;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class GroupMillService:IGroupMillService
    {

        private readonly ApplicationDbContext _context;

        public GroupMillService(ApplicationDbContext context)
        {
          _context = context;
           
        }

        public async Task<IEnumerable<GroupMill>> GetAllGroupMillsAsync()
        {
            return await _context.GroupMills.ToListAsync();
        }


        public async Task CreateGroupMillAsync(GroupMill groupMill)
        {
            _context.GroupMills.Add(groupMill);
            await _context.SaveChangesAsync();
        }

        public async Task<GroupMill> GetGroupMillByIdAsync(int groupId)
        {
            return await _context.GroupMills.FirstOrDefaultAsync(g => g.GroupId == groupId);
        }


        public async Task UpdateGroupMillAsync(GroupMill groupMill)
        {
            _context.Update(groupMill);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGroupMillAsync(int groupId)
        {
            var groupMill = await _context.GroupMills.FindAsync(groupId);
            if (groupMill != null)
            {
                _context.GroupMills.Remove(groupMill);
                await _context.SaveChangesAsync();
            }
        }


    }
}
