using SICalcWebApp.Areas.SICalculator.Models;

namespace SICalcWebApp.Repository
{
    public interface ITPDInfoService
    {
        Task AddTPDInfoAsync(TPDInfo tpdInfo);
        Task<IEnumerable<TPDInfo>> GetAllTPDInfosAsync();

        Task<TPDInfo> GetTPDInfoByIdAsync(int id);
        Task UpdateTPDInfoAsync(TPDInfo tpdInfo);
        Task DeleteTPDInfoAsync(int id);
    }
}
