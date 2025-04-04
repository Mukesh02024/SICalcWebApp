using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.VM;
using SICalcWebApp.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class BatchProcessReportService:IBatchProcessReportService
    {
        private readonly ApplicationDbContext _context;
        public BatchProcessReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<object>> GetBatchProcessReportAsync(string processType, DateTime fromDate, DateTime toDate)
        {
            if (string.IsNullOrEmpty(processType))
            {
                throw new ArgumentException("Process type is required.");
            }

            if (processType.ToUpper() == "USNA")
            {
                return await _context.BatchProcessReports
                    .FromSqlRaw("EXEC GetBatchProcessReport_USNA @p0, @p1", fromDate, toDate)
                    .ToListAsync<object>();
            }
            else if (processType.ToUpper() == "ARWA")
            {
                return await _context.BatchProcessReportArwaVMs
                    .FromSqlRaw("EXEC GetBatchProcessReport_ARWA @p0, @p1", fromDate, toDate)
                    .ToListAsync<object>();

            }

            else
            {
                throw new ArgumentException("Invalid process type.");
            }
        }

    }
}
