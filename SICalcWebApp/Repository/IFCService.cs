using SICalcWebApp.Areas.SICalculator.Models;

namespace SICalcWebApp.Repository
{
    public interface IFCService
    {
        Task CreateFCAsync(FC fc);
        Task<IEnumerable<FC>> GetAllFCsAsync();
        Task<FC> GetFCByIdAsync(int id); // Method to fetch an FC by Id
        Task UpdateFCAsync(FC fc);        // Method to update an existing FC
        Task DeleteFCAsync(int id);       // Method to delete an FC by Id


    }
}
