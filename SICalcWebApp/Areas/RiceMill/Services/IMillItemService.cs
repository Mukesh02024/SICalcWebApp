using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IMillItemService
    {

        Task<IEnumerable<MillItem>> GetAllMillItemsAsync();
        Task<MillItem> GetMillItemByIdAsync(int id);
        Task CreateMillItemAsync(MillItem millItem);

        Task UpdateMillItemAsync(MillItem millItem);
        Task DeleteMillItemAsync(int id);
    }
}
