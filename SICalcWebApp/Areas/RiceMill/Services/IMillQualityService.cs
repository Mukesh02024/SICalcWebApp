using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IMillQualityService
    {
        Task<List<BatchRemainingStages>> GetRemainingBatchesAsync();

        Task<bool> SubmitQualityAsync(MillQuality model);

        Task<List<BatchRemainingStages>> GetRemainingBatchesSortexAsync();

        Task<bool> SubmitQualitySortexAsync(MillQualitySortex model);
    }
}
