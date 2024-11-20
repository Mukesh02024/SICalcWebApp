using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Repository
{
    public class PriceMaterialSer:IPriceMaterial
    {
        private readonly ApplicationDbContext _context;


        public PriceMaterialSer(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPriceofmaterialAsync(PriceOfMaterial priceMaterial)
        {
            _context.PriceOfMaterials.Add(priceMaterial);

            await _context.SaveChangesAsync();
        }


      

        public async Task<IEnumerable<PriceOfMaterial>> GetAllPriceMaterialAsync()
        {
            return await _context.PriceOfMaterials.ToListAsync();
        }


        public async Task<PriceOfMaterial> GetPriceMaterialAsync()
        {
            return await _context.PriceOfMaterials.FirstOrDefaultAsync(); // Fetch the single record
        }

        public async Task UpdatePriceMaterialAsync(PriceOfMaterial priceMaterial)
        {
            _context.PriceOfMaterials.Update(priceMaterial);
            await _context.SaveChangesAsync();
        }
    }
}
