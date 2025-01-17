using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IMachineProcessService
    {
        Task StartHandiProcessAsync(HandiProcess handiProcess);
        Task<HandiProcess?> GetHandiProcessAsync(string batchId);

        Task UpdateHandiProcessAsync(HandiProcess handiProcess);
        Task<MasterDataViewModel> GetMasterDataAsync();
        int GetNextBatchId(string batchPrefix);
        Task PauseProcessAsync(string batchId, string pauseReason, DateTime? pauseTime);
        Task ResumeProcessAsync(string batchId, DateTime? resumeTime);
        Task EndProcessAsync(string batchId, DateTime? endTime);

        Task<HandiProcess> GetActiveProcessAsync();

        Task<bool> IsDryerFreeAsync();

        Task<bool> AreAllCompletedBatchesInDryerProcessAsync();


        bool CompleteArwaProcess(string batchId, string unloadBunker, DateTime? endTime);

    }
}
