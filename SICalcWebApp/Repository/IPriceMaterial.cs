using SICalcWebApp.Areas.SICalculator.Models;

namespace SICalcWebApp.Repository
{
    public interface IPriceMaterial
    {


        Task AddPriceofmaterialAsync(PriceOfMaterial priceOfMaterial);
        Task<IEnumerable<PriceOfMaterial>> GetAllPriceMaterialAsync();

        Task<PriceOfMaterial> GetPriceMaterialAsync(); // Fetch the single PriceOfMaterial
        Task UpdatePriceMaterialAsync(PriceOfMaterial priceOfMaterial); // Update the single PriceOfMaterial
    }
}
