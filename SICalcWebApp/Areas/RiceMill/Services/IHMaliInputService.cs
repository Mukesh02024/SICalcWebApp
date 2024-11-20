using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.VM;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IHMaliInputService
    {
        Task<IEnumerable<GroupMill>> GetGroupsAsync();
        Task<IEnumerable<MillItem>> GetItemsByGroupAsync(int groupId);

        Task<IEnumerable<HmaliInput>> SearchHmaliInputsAsync(DateTime? fromDate, DateTime? toDate, int? groupId, int? itemId);



        Task<HmaliInput> CreateHmaliInputAsync(HmaliInput hmaliInput);
        Task<MillItem> GetItemDetailsAsync(int itemNumber);
        Task<MillItem> GetItemDetailssAsync(int itemNumber);
        Task<HmaliInput> GetHmaliInputByIdAsync(int hmaliId);


        Task UpdateHmaliInputAsync(HmaliInput hmaliInput);
        Task DeleteHmaliInputAsync(int hmaliId);


        Task<IEnumerable<HmaliInputViewModel>> GetAllHmaliInputsAsync();
    }
}
