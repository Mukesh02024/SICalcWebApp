using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IDryerService
    {
        Task<List<string>> GetAvailableBatchesForDryerAsync();
        Task StartDryerProcessAsync(DryerProcess DryerProcess);

        Task<DryerProcess?> GetDryerProcessAsync(string batchId);




        Task PauseProcessAsync(string batchId, string pauseReason);
        Task ResumeProcessAsync(string batchId);
        Task EndProcessAsync(string batchId, string UnloadBunkers);


        Task<DryerProcess> GetActiveProcessAsync();

        Task<bool> IsAnyBunkerEmptyAsync();

        Task<List<string>> GetEmptyBunkersAsync();

    }
}
