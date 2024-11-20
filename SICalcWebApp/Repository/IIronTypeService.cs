using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;

namespace SICalcWebApp.Repository
{
    public interface IIronTypeService
    {
        Task AddIronTypeAsync(IronType ironType);
        Task<IEnumerable<IronType>> GetAllIronTypesAsync();

        Task UpdateIronTypeAsync(IronType ironType);
        Task DeleteIronTypeAsync(int ironTypeId);
    }
}
