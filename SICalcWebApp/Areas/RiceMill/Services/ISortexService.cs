using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface ISortexService
    {
        Task<List<string>> GetAvailableBatchesForSortexAsync();
        Task StartSortexProcessAsync(SortexProcess sortexProcess);

        Task<SortexProcess?> GetSortexProcessAsync(string batchId);




        Task PauseProcessAsync(string batchId, string pauseReason, DateTime? PauseTime);
        Task ResumeProcessAsync(string batchId, DateTime? ResumeTime);


        Task EndProcessAsync(string batchId, DateTime? EndTime);


        Task<SortexProcess> GetActiveProcessAsync();




        Task<List<string>> GetOccupiedSortexBunkersAsync();
    Task<List<string>> GetBatchIdsForSortexAsync(string sortexBunkerName);

    }
}
