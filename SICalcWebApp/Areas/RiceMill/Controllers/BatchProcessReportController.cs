using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SICalcWebApp.Areas.RiceMill.Services;
using System.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    public class BatchProcessReportController : Controller
    {
        private readonly IBatchProcessReportService _reportService;

        public BatchProcessReportController(IBatchProcessReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBatchProcessReport(string processType, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var reports = await _reportService.GetBatchProcessReportAsync(processType, fromDate, toDate);
       

                return Json(reports);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string processType, DateTime fromDate, DateTime toDate)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // ✅ Required for EPPlus

                var data = await _reportService.GetBatchProcessReportAsync(processType, fromDate, toDate);

                if (data == null || !data.Any())
                {
                    return BadRequest("No data available for export.");
                }

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Batch Report");

                    // Define headers dynamically based on processType
                    int col = 1;
                    worksheet.Cells[1, col++].Value = "Batch ID";
                    worksheet.Cells[1, col++].Value = "Process Type";
                    worksheet.Cells[1, col++].Value = "Paddy Type";
                    worksheet.Cells[1, col++].Value = "Handi Type";
                    worksheet.Cells[1, col++].Value = "Pressure";
                    worksheet.Cells[1, col++].Value = "Handi Staff";
                    worksheet.Cells[1, col++].Value = "Handi Start";
                    worksheet.Cells[1, col++].Value = "Handi End";
                    worksheet.Cells[1, col++].Value = "Handi Delay";
                    worksheet.Cells[1, col++].Value = "Handi Taken Time";

                    if (processType.Equals("USNA", StringComparison.OrdinalIgnoreCase))
                    {
                        // Add Dryer Process columns (Only for USNA)
                        worksheet.Cells[1, col++].Value = "Dryer Load Time";
                        worksheet.Cells[1, col++].Value = "Dryer Unload Time";
                        worksheet.Cells[1, col++].Value = "Ducti Pressure";
                        worksheet.Cells[1, col++].Value = "Dryer Unload Bunk";
                        worksheet.Cells[1, col++].Value = "Dryer Staff";
                        worksheet.Cells[1, col++].Value = "Dryer Delay";
                        worksheet.Cells[1, col++].Value = "Dryer Taken Time";
                        worksheet.Cells[1, col++].Value = "Dryer-Handi";
                    }

                    // Milling & Sortex Process (Included for both USNA & ARWA)
                    worksheet.Cells[1, col++].Value = "Mill Start Time";
                    worksheet.Cells[1, col++].Value = "Mill End Time";
                    worksheet.Cells[1, col++].Value = "Mill Bunker Name";
                    worksheet.Cells[1, col++].Value = "Sortex Unload Bunk";
                    worksheet.Cells[1, col++].Value = "Milling Staff";
                    worksheet.Cells[1, col++].Value = "Milling Delay";
                    worksheet.Cells[1, col++].Value = "Milling Taken Time";
                    worksheet.Cells[1, col++].Value = "Sortex Start Time";
                    worksheet.Cells[1, col++].Value = "Sortex End Time";
                    worksheet.Cells[1, col++].Value = "Sortex Load Bunk";
                    worksheet.Cells[1, col++].Value = "Sortex Staff";
                    worksheet.Cells[1, col++].Value = "Sortex Delay";
                    worksheet.Cells[1, col++].Value = "Sortex Taken Time";

                    int row = 2;

                    foreach (var reportObj in data)
                    {
                        var report = reportObj as dynamic;
                        if (report == null) continue;

                        col = 1;
                        worksheet.Cells[row, col++].Value = report.BatchId;
                        worksheet.Cells[row, col++].Value = report.ProcessType;
                        worksheet.Cells[row, col++].Value = report.PaddyType;
                        worksheet.Cells[row, col++].Value = report.HandiType;
                        worksheet.Cells[row, col++].Value = report.Pressure;
                        worksheet.Cells[row, col++].Value = report.HandiStaff;
                        worksheet.Cells[row, col++].Value = report.HandiStartTime?.ToString();
                        worksheet.Cells[row, col++].Value = report.HandiEndTime?.ToString();
                        worksheet.Cells[row, col++].Value = report.HandiDelay;
                        worksheet.Cells[row, col++].Value = report.HandiTakenTime;

                        if (processType.Equals("USNA", StringComparison.OrdinalIgnoreCase))
                        {
                            // Add Dryer Process Data (Only for USNA)
                            worksheet.Cells[row, col++].Value = report.DryerLoadTime?.ToString();
                            worksheet.Cells[row, col++].Value = report.DryerUnloadTime?.ToString();
                            worksheet.Cells[row, col++].Value = report.DuctiPressure;
                            worksheet.Cells[row, col++].Value = report.DryerUnloadBunk;
                            worksheet.Cells[row, col++].Value = report.DryerStaff;
                            worksheet.Cells[row, col++].Value = report.DryerDelay;
                            worksheet.Cells[row, col++].Value = report.DryerTakenTime;
                            worksheet.Cells[row, col++].Value = report.DryerHandiTimeDifference;
                        }

                        // Milling & Sortex Process Data (Included for both USNA & ARWA)
                        worksheet.Cells[row, col++].Value = report.MillStartTime?.ToString();
                        worksheet.Cells[row, col++].Value = report.MillEndTime?.ToString();
                        worksheet.Cells[row, col++].Value = report.MillBunkerName;
                        worksheet.Cells[row, col++].Value = report.SortexUnloadBunk;
                        worksheet.Cells[row, col++].Value = report.MillingStaff;
                        worksheet.Cells[row, col++].Value = report.MillingDelay;
                        worksheet.Cells[row, col++].Value = report.MillingTakenTime;
                        worksheet.Cells[row, col++].Value = report.SortexStartTime?.ToString();
                        worksheet.Cells[row, col++].Value = report.SortexEndTime?.ToString();
                        worksheet.Cells[row, col++].Value = report.SortexloadBunk;
                        worksheet.Cells[row, col++].Value = report.SortexStaff;
                        worksheet.Cells[row, col++].Value = report.SortexDelay;
                        worksheet.Cells[row, col++].Value = report.SortexTakenTime;

                        row++;
                    }

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    var fileName = $"BatchProcessReport_{processType}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExportToExcel Error: {ex.Message}");
                return StatusCode(500, "An error occurred while exporting the data.");
            }
        }







    }
}
