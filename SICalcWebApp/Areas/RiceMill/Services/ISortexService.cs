using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface ISortexService
    {
        Task<List<string>> GetAvailableBatchesForSortexAsync();
        Task StartSortexProcessAsync(SortexProcess sortexProcess);

        Task<SortexProcess?> GetSortexProcessAsync(string batchId);




        Task PauseProcessAsync(string batchId, string pauseReason);
        Task ResumeProcessAsync(string batchId);


        Task EndProcessAsync(string batchId);


        Task<SortexProcess> GetActiveProcessAsync();




        Task<List<string>> GetOccupiedSortexBunkersAsync();
    Task<List<string>> GetBatchIdsForSortexAsync(string sortexBunkerName);

    }
}
