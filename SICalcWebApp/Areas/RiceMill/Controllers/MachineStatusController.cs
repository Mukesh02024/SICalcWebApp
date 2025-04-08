using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SICalcWebApp.Areas.RiceMill.VM;
using SICalcWebApp.Data;
using System.Data;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    [Authorize(Roles = $"{SD.Role_Mill_Admin},{SD.Role_Super_Admin}")]
    public class MachineStatusController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public MachineStatusController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("SICS");
        }

        public IActionResult Index()
        {
            var viewModel = GetMachineStatus();
            return View(viewModel);
        }

        public IActionResult Refresh()
        {
            var viewModel = GetMachineStatus();



            return PartialView("~/Areas/RiceMill/Views/BatchProcessReport/_MachineStatusPartial.cshtml", viewModel);
        }

        private MachineStatusViewModel GetMachineStatus()
        {

            var viewModel = new MachineStatusViewModel();

            var latestHandi = _context.HandiProcesses
                .Where(p => p.ProcessStatus == "In Progress" || p.ProcessStatus == "Paused")
                .OrderByDescending(p => p.StartTime)
                .FirstOrDefault();

            var latestDryer = _context.DryerProcesses
                .Where(p => p.ProcessStatus == "In Progress" || p.ProcessStatus == "Paused")
                .OrderByDescending(p => p.LoadTime)
                .FirstOrDefault();

            var latestMilling = _context.MillingProcesses
                .Where(p => p.ProcessStatus == "In Progress" || p.ProcessStatus == "Paused")
                .OrderByDescending(p => p.StartTime)
                .FirstOrDefault();

            var latestSortex = _context.SortexProcesses
                .Where(p => p.ProcessStatus == "In Progress" || p.ProcessStatus == "Paused")
                .OrderByDescending(p => p.StartTime)
                .FirstOrDefault();

 

            viewModel.HandiBatchId = latestHandi?.BatchId;
            viewModel.HandiStatus = latestHandi?.ProcessStatus;
            viewModel.HandiStartTime = latestHandi?.StartTime;

            viewModel.DryerBatchId = latestDryer?.BatchId ?? "N/A";
            viewModel.DryerStatus = latestDryer?.ProcessStatus ?? "Not Started";
            viewModel.DryerStartTime = latestDryer?.LoadTime;

            viewModel.MillingBatchId = latestMilling?.BatchId;
            viewModel.MillingStatus = latestMilling?.ProcessStatus;
            viewModel.MillingStartTime = latestMilling?.StartTime;

            viewModel.SortexBatchId = latestSortex?.BatchId;
            viewModel.SortexStatus = latestSortex?.ProcessStatus;
            viewModel.SortexStartTime = latestSortex?.StartTime;

            viewModel.StockList = new List<RemainingStock>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("GetRemainingPaddyStock1", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            viewModel.StockList.Add(new RemainingStock
                            {
                                PaddyTypeName = reader["PaddyTypeName"].ToString(),
                                RemainingQuantity= Convert.ToInt32(reader["RemainingQuantity"])
                            });
                        }
                    }
                }
            }



            // this is for mil bunker status 
            viewModel.MillBList = new List<BunkerStatusVM>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("GetBunkerStatus",connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            viewModel.MillBList.Add(new BunkerStatusVM
                            {
                                MillBname = reader["BunkerName"].ToString(),
                                Status = reader["BunkerStatus"].ToString()
                            });
                        }
                    }
                }
            }




            if (latestMilling?.BatchId != null)
            {
                string batchId = latestMilling.BatchId;

                var isUSNA = batchId.Contains("USNA");
                var isARWA = batchId.Contains("ARWA");

                var qualityStats = _context.MillQualities
                    .Where(q => q.BatchID == batchId)
                    .GroupBy(q => 1) // Grouping to perform aggregation in a single query
                    .Select(g => new
                    {
                        QualityStageCount = g.Count(),
                        AvgMachineBroken = g.Average(q => (double?)q.Machine_Broken) ?? 0,
                        AvgManualBroken = g.Average(q => (double?)q.Manual_Broken) ?? 0,
                        AvgMachineMoisture = g.Average(q => (double?)q.Machine_Moisture) ?? 0,
                        AvgManualMoisture = g.Average(q => (double?)q.Manual_Moisture) ?? 0,
                        AvgMoistureChottiMachine = g.Average(q => (double?)q.Moisture_Chotti_Machine) ?? 0,
                        AvgMoistureChottiMachineManual = g.Average(q => (double?)q.Moisture_Chotti_Machine_Manual) ?? 0,
                        AvgMillWeightment = g.Average(q => (double?)q.Mill_Wightment) ?? 0,

                        // Damage and Discolour (Only for USNA batches)
                        AvgMachineDamage = isUSNA ? g.Average(q => (double?)q.Machine_Damage) ?? 0 : (double?)null,
                        AvgManualDamage = isUSNA ? g.Average(q => (double?)q.Manual_Damage) ?? 0 : (double?)null,
                        AvgMachineDiscolour = isUSNA ? g.Average(q => (double?)q.Machine_Discolour) ?? 0 : (double?)null,
                        AvgManualDiscolour = isUSNA ? g.Average(q => (double?)q.Manual_Discolour) ?? 0 : (double?)null
                    })
                    .FirstOrDefault();
                if (qualityStats != null)
                {
                    viewModel.QualityStageCount = qualityStats.QualityStageCount;
                    viewModel.AvgMachineBroken = Math.Round(qualityStats.AvgMachineBroken ,2);
                    viewModel.AvgManualBroken = Math.Round(qualityStats.AvgManualBroken,2) ;
                    viewModel.AvgMachineMoisture = Math.Round(qualityStats.AvgMachineMoisture,2);
                    viewModel.AvgManualMoisture = Math.Round(qualityStats.AvgManualMoisture,2) ;
                    viewModel.AvgMoistureChottiMachine = Math.Round(qualityStats.AvgMoistureChottiMachine ,2);
                    viewModel.AvgMoistureChottiMachineManual = Math.Round(qualityStats.AvgMoistureChottiMachineManual,2) ;
                    viewModel.AvgMillWeightment = Math.Round(qualityStats.AvgMillWeightment,2) ;

                    if (isUSNA)
                    {
                        viewModel.AvgMachineDamage = Math.Round(qualityStats.AvgMachineDamage ?? 0.0, 2);
                        viewModel.AvgManualDamage = Math.Round(qualityStats.AvgManualDamage ?? 0.0,2);
                        viewModel.AvgMachineDiscolour = Math.Round(qualityStats.AvgMachineDiscolour ?? 0.0,2);
                        viewModel.AvgManualDiscolour = Math.Round(qualityStats.AvgManualDiscolour ?? 0.0,2);
                    }
                }



            }

            if (latestSortex?.BatchId != null)
            {
                string batchId = latestSortex.BatchId;

                var isUSNA = batchId.Contains("USNA");
                var isARWA = batchId.Contains("ARWA");

                var qualityStats = _context.MillQualitySortexes
                    .Where(q => q.BatchID == batchId)
                    .GroupBy(q => 1) // Grouping to perform aggregation in a single query
                    .Select(g => new
                    {
                        QualityStageCount = g.Count(),
                        AvgMachineBroken = g.Average(q => (double?)q.Machine_Broken) ?? 0,
                        AvgManualBroken = g.Average(q => (double?)q.Manual_Broken) ?? 0,
                        AvgMachineMoisture = g.Average(q => (double?)q.Machine_Moisture) ?? 0,
                        AvgManualMoisture = g.Average(q => (double?)q.Manual_Moisture) ?? 0,
                        AvgMoistureChottiMachine = g.Average(q => (double?)q.Moisture_Chotti_Machine) ?? 0,
                        AvgMoistureChottiMachineManual = g.Average(q => (double?)q.Moisture_Chotti_Machine_Manual) ?? 0,
                       

                        // Damage and Discolour (Only for USNA batches)
                        AvgMachineDamage = isUSNA ? g.Average(q => (double?)q.Machine_Damage) ?? 0 : (double?)null,
                        AvgManualDamage = isUSNA ? g.Average(q => (double?)q.Manual_Damage) ?? 0 : (double?)null,
                        AvgMachineDiscolour = isUSNA ? g.Average(q => (double?)q.Machine_Discolour) ?? 0 : (double?)null,
                        AvgManualDiscolour = isUSNA ? g.Average(q => (double?)q.Manual_Discolour) ?? 0 : (double?)null,
                        sec30 = isUSNA ? g.Average(q => (double?)q.Sec30Final) ?? 0 : (double?)null,
                        min10 = isUSNA ? g.Average(q => (double?)q.Min10Rejection) ?? 0 : (double?)null
                    })
                    .FirstOrDefault();
                if (qualityStats != null)
                {
                    viewModel.QualityStageSortexCount = qualityStats.QualityStageCount;
                    viewModel.AvgSortexMachineBroken = Math.Round(qualityStats.AvgMachineBroken,2);
                    viewModel.AvgSortexManualBroken = Math.Round(qualityStats.AvgManualBroken,2);
                    viewModel.AvgSortexMachineMoisture = Math.Round(qualityStats.AvgMachineMoisture,2);
                    viewModel.AvgSortexManualMoisture = Math.Round(qualityStats.AvgManualMoisture,2);
                    viewModel.AvgSortexMoistureChottiMachine = Math.Round(qualityStats.AvgMoistureChottiMachine,2);
                    viewModel.AvgSortexMoistureChottiMachineManual = Math.Round(qualityStats.AvgMoistureChottiMachineManual,2);
              

                    if (isUSNA)
                    {
                        viewModel.AvgSortexMachineDamage = Math.Round(qualityStats.AvgMachineDamage ?? 0.0,2);
                        viewModel.AvgSortexManualDamage = Math.Round(qualityStats.AvgManualDamage ?? 0.0,2);
                        viewModel.AvgSortexMachineDiscolour = Math.Round(qualityStats.AvgMachineDiscolour ?? 0.0,2);
                        viewModel.AvgSortexManualDiscolour = Math.Round(qualityStats.AvgManualDiscolour ?? 0.0,2);

                        viewModel.Avg30Second = Math.Round(qualityStats.sec30 ?? 0.0,2);
                        viewModel.Avg10minutes = Math.Round(qualityStats.min10 ?? 0.0, 2);
                    }
                }



            }



            return viewModel;
        }
    }
}