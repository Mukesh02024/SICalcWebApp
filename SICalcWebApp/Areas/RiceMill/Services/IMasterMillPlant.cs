using SICalcWebApp.Areas.RiceMill.Models;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public interface IMasterMillPlant
    {
        Task<List<Staff>> GetAllStaffAsync();
        Task<Staff> GetStaffByIdAsync(int id);
        Task<bool> AddStaffAsync(Staff staff);
        Task<bool> UpdateStaffAsync(Staff staff);
        Task<bool> DeleteStaffAsync(int id);

        // Paddy Type Methods
        Task<List<PaddyType>> GetAllPaddyTypesAsync();
        Task<PaddyType> GetPaddyTypeByIdAsync(int id);
        Task<bool> AddPaddyTypeAsync(PaddyType paddyType);
        Task<bool> UpdatePaddyTypeAsync(PaddyType paddyType);
        Task<bool> DeletePaddyTypeAsync(int id);
    }
}
