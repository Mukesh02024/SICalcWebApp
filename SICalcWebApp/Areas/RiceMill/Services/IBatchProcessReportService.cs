using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IBatchProcessReportService
    {
        Task<List<object>> GetBatchProcessReportAsync(string processType, DateTime fromDate, DateTime toDate);
    }
}
