using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IMachineProcessService
    {
        Task StartHandiProcessAsync(HandiProcess handiProcess);
        Task<HandiProcess?> GetHandiProcessAsync(string batchId);

        Task UpdateHandiProcessAsync(HandiProcess handiProcess);
        Task<MasterDataViewModel> GetMasterDataAsync();
        int GetNextBatchId();
        Task PauseProcessAsync(string batchId, string pauseReason);
        Task ResumeProcessAsync(string batchId);
        Task EndProcessAsync(string batchId);

        Task<HandiProcess> GetActiveProcessAsync();

    }
}
