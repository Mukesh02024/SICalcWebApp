using Microsoft.AspNetCore.Mvc.Rendering;
using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IMillingProcessService
    {
        Task<List<string>> GetAvailableBatchesForMillAsync();
        Task StartMillProcessAsync(MillingProcess millingProcess);

        Task<MillingProcess?> GetMillProcessAsync(string batchId);




        Task PauseProcessAsync(string batchId, string pauseReason);
        Task ResumeProcessAsync(string batchId);


        Task EndProcessAsync(string batchId,string SortexBunker);


        Task<MillingProcess> GetActiveProcessAsync();

        Task<List<SelectListItem>> GetOccupiedBunkersAsync();
        Task<List<SelectListItem>> GetBatchesForOccupiedBunkerAsync(string occupiedBunkerName);



        Task<MillBunker> GetBunkerByNameAsync(string bunkerName);
        Task UpdateBunkerStatusAsync(MillBunker bunker);

        Task<bool> IsAnyBunkerSortexEmptyAsync();
        Task<List<string>> GetEmptySortexBunkersAsync();


    

    }
}
