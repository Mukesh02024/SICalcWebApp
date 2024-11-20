using SICalcWebApp.Areas.SICalculator.Models;

namespace SICalcWebApp.Repository
{
    public interface IFCInfoService
    {
        Task<IEnumerable<FCInfo>> GetAllFCInfosAsync();
        Task AddFCInfoAsync(FCInfo fCInfo);
        IEnumerable<FC> GetAllFCs();
        IEnumerable<TPDInfo> GetAllTPDInfos();

        //new add

        Task<IEnumerable<FC>> GetAllFCsAsync();
        Task<IEnumerable<TPDInfo>> GetAllTPDInfosAsync();


        // Methods to update and delete FCInfo
        Task UpdateFCInfoAsync(FCInfo fCInfo);  // Method to update an existing FCInfo
        Task DeleteFCInfoAsync(int id);  // Method to delete an FCInfo by its ID

    }
}
