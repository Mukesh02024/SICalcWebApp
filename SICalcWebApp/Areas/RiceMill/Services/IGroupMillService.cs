using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IGroupMillService
    {
        Task CreateGroupMillAsync(GroupMill groupMill);
        Task<IEnumerable<GroupMill>> GetAllGroupMillsAsync();

        Task<GroupMill> GetGroupMillByIdAsync(int groupId);


        Task UpdateGroupMillAsync(GroupMill groupMill); // New method for updating
        Task DeleteGroupMillAsync(int groupId); // New method for deleting
    }
}
