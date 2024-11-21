using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class MasterMillPlant:IMasterMillPlant
    {

        private readonly ApplicationDbContext _context;

        public MasterMillPlant(ApplicationDbContext context)
        {
            _context = context;
        }

        // Staff Methods
        public async Task<List<Staff>> GetAllStaffAsync()
        {
            return await _context.Staffs.ToListAsync();
        }

        public async Task<Staff> GetStaffByIdAsync(int id)
        {
            return await _context.Staffs.FindAsync(id);
        }

        public async Task<bool> AddStaffAsync(Staff staff)
        {
            _context.Staffs.Add(staff);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStaffAsync(Staff staff)
        {
            _context.Staffs.Update(staff);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return false;

            _context.Staffs.Remove(staff);
            return await _context.SaveChangesAsync() > 0;
        }

        // Paddy Type Methods
        public async Task<List<PaddyType>> GetAllPaddyTypesAsync()
        {
            return await _context.PaddyTypes.ToListAsync();
        }

        public async Task<PaddyType> GetPaddyTypeByIdAsync(int id)
        {
            return await _context.PaddyTypes.FindAsync(id);
        }

        public async Task<bool> AddPaddyTypeAsync(PaddyType paddyType)
        {
            _context.PaddyTypes.Add(paddyType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePaddyTypeAsync(PaddyType paddyType)
        {
            _context.PaddyTypes.Update(paddyType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePaddyTypeAsync(int id)
        {
            var paddyType = await _context.PaddyTypes.FindAsync(id);
            if (paddyType == null) return false;

            _context.PaddyTypes.Remove(paddyType);
            return await _context.SaveChangesAsync() > 0;
        }













        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(int id) where T : class
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
