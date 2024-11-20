using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OfficeOpenXml;
using SICalcWebApp.Areas.SICalculator.Models;
using SICalcWebApp.Areas.SICalculator.VM;
using SICalcWebApp.Data;
using SICalcWebApp.Migrations;

namespace SICalcWebApp.Areas.SICalculator.Controllers
{
    [Area("SICalculator")]
    [Authorize(Roles = $"{SD.Role_Sponge_Admin},{SD.Role_Super_Admin}")]
    public class FeedRateAvgController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FeedRateAvgController(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            // Get all FCs to populate the dropdown
            var allFCs = await _context.FCs.ToListAsync();

            // Initially, we don't load any TPDs since they will be loaded based on FC selection
            var model = new FeedRateViewModel
            {
                FCs = allFCs,
                TPDs = new List<TPDInfo>(), // Empty TPDs list initially
                SelectedTPDs = new List<int>()
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetTPDsByFC(int fcId)
        {


            var tpdIdsWithFeedRatesForFC = await _context.FCInfos
        .Where(fci => fci.FCId == fcId)
        .Select(fci => fci.TPDId)
        .ToListAsync();

            // Project the data into a simpler structure
            var tpdList = await _context.TPDInfos
                .Where(tpd => tpdIdsWithFeedRatesForFC.Contains(tpd.TPDId))
                .Select(tpd => new
                {
                    TPDId = tpd.TPDId,
                    tpd.TPD,
                    kilnName = tpd.KilnName
                })
                .ToListAsync();

            return Json(tpdList);

        }


        [HttpPost]
        public async Task<IActionResult> GetAverageFeedRate(int selectedFC, int[] selectedTPDs)
        {
            try
            {
                // Debugging: Log the selected FC and TPDs
                Console.WriteLine($"Selected FC: {selectedFC}");
                Console.WriteLine("Selected TPDs: " + string.Join(", ", selectedTPDs));

                // Check if the selected FC exists
                var fcExists = await _context.FCs.AnyAsync(fc => fc.FCId == selectedFC);
                if (!fcExists)
                {
                    return Json(new { error = "Invalid FC selected." });
                }

                // Get the valid TPD IDs
                var validTpdIds = await _context.TPDInfos
                    .Where(tpd => selectedTPDs.Contains(tpd.TPDId))
                    .Select(tpd => tpd.TPDId)
                    .ToListAsync();

                if (!validTpdIds.Any())
                {
                    return Json(new { error = "Invalid TPDs selected." });
                }

                // Debugging: Log the valid TPD IDs
                Console.WriteLine("Valid TPD IDs: " + string.Join(", ", validTpdIds));

                // Execute the query
                var query = from fcInfo in _context.FCInfos
                            join fc in _context.FCs on fcInfo.FCId equals fc.FCId
                            join tpd in _context.TPDInfos on fcInfo.TPDId equals tpd.TPDId
                            where fc.FCId == selectedFC && validTpdIds.Contains(tpd.TPDId)
                            group fcInfo by new { fc.FCValue, fc.C_Fe } into g
                            select new
                            {
                                FC = g.Key.FCValue,
                                AverageFeedRate = g.Average(x => x.FeedRate),
                                CostPerFeedRate = g.Key.C_Fe
                            }; 

                var result = await query.FirstOrDefaultAsync();

                // Debugging: Log the result
                if (result != null)
                {
                    Console.WriteLine($"Result: FC = {result.FC}, AverageFeedRate = {result.AverageFeedRate}, CostPerFeedRate = {result.CostPerFeedRate}");
                }
                else
                {
                    Console.WriteLine("No results found.");
                }

                if (result == null)
                {
                    return Json(new { error = "No data found for the selected FC and TPDs." });
                }

                return Json(new
                {
                    FC = result.FC,
                    AverageFeedRate = result.AverageFeedRate,
                    CostPerFeedRate = result.CostPerFeedRate
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }


        public async Task<IActionResult> GetMaterials()
        {
            var materials = await _context.InputOperands
                .Select(m => new MaterialInputViewModel
                {
                    MaterialName = m.Sidename
                }).ToListAsync();

            return Json(materials);
        }





        [HttpPost]
        public IActionResult PerformCalculations([FromBody] FeedRateViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }

            var results = new List<MaterialCalculationResult>();
            int combinationId = 1;

            if (model.IsAutoGeneration)
            {
                // Auto-generate all possible combinations with a 5% increment
                var combinations = GenerateCombinations(model.SelectedMaterials, 5m);

                foreach (var combination in combinations)
                {
                    var combinationResults = PerformSingleCombinationCalculation(combination, model.AverageFeedRate, model.NumberOfRunningKilns, model.Cfe, model.Moisture, model.SelectedFC, model.CoalCost);

                    foreach (var result in combinationResults)
                    {
                        result.CombinationId = combinationId; // Assign a unique combination ID
                    }

                    results.AddRange(combinationResults);
                    combinationId++;
                }
            }
            else
            {
                // For manual input, only one combination to process
                var singleCombinationResult = PerformSingleCombinationCalculation(model.MaterialPercentages, model.AverageFeedRate, model.NumberOfRunningKilns, model.Cfe, model.Moisture, model.SelectedFC, model.CoalCost);

                foreach (var result in singleCombinationResult)
                {
                    result.CombinationId = combinationId; // Assign ID for single combination
                }

                results.AddRange(singleCombinationResult);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

            var singleRowData = _context.PriceOfMaterials.SingleOrDefault();

            // Cache the total count of registered kilns to avoid multiple async calls
            int countRegisteredKlin = _context.TPDInfos.Count();


            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Results");

                // Dynamically get all unique material names from the results
                var allMaterialNames = results
                    .Select(r => r.MaterialName)
                    .Distinct()
                    .ToList();

                // Add headers to the worksheet
                worksheet.Cells[1, 1].Value = "Combination Id";

                int materialColStart = 2; // Start writing material columns after CombinationId

                // Dynamically create material columns based on unique material names
                for (int i = 0; i < allMaterialNames.Count; i++)
                {
                    worksheet.Cells[1, materialColStart + i].Value = allMaterialNames[i]; // Material Name column headers
                }

                // Add headers for calculated totals
                worksheet.Cells[1, materialColStart + allMaterialNames.Count].Value = "Sponge Cost A+B+C+D+E";
                worksheet.Cells[1, materialColStart + allMaterialNames.Count + 1].Value = "Gangue in Sponge ";
                worksheet.Cells[1, materialColStart + allMaterialNames.Count + 2].Value = "Phos In Sponge ";
                worksheet.Cells[1, materialColStart + allMaterialNames.Count + 3].Value = "Total Production Sponge";

                // Start writing data from row 2
                int row = 2;

                // Group the results by CombinationId to process each combination
                foreach (var combinationGroup in results.GroupBy(r => r.CombinationId))
                {
                    // Write the combination ID in the first column
                    worksheet.Cells[row, 1].Value = combinationGroup.Key;

                    decimal totalSpongeCost = 0;
                    decimal totalGuenge = 0;
                    decimal totalProduction = 0;

                    decimal totalnetironcst = 0;

                    decimal totalironconve = 0;
                    decimal totalcoalreq = 0;
                    decimal sconsucoal = 0;
                    decimal btotalcoalcst = 0;
                    decimal CoalCost = 0;

                    decimal mgfexpence = 0;

                    decimal Dmgfexpence = 0;


                    decimal EfixedCost = 0;

                    decimal dolomitecst = 0;

                    decimal Gangeinsponge=0;
                    decimal PhosInsponge = 0;



                    decimal TGangeinsponge = 0;
                    decimal TPhosInsponge = 0;

                    // Create a dictionary to store the material percentages for this row
                    var materialPercentages = new Dictionary<string, decimal>();

                    // Initialize all materials with 0% for the current row (in case some materials are missing in a combination)
                    foreach (var materialName in allMaterialNames)
                    {
                        materialPercentages[materialName] = 0;
                    }

                    // Loop through each result in the combination and accumulate data
                    foreach (var result in combinationGroup)
                    {
                     
                        totalGuenge += result.GuengeInSpong;
                        totalProduction += result.TotalProductionSponge;
                        totalnetironcst += result.TotalNetIronCost;
                        totalcoalreq += result.TotalCoalRequired;
                        CoalCost=result.CoalCost;

                        Gangeinsponge += (result.GuengeInSpong * result.TotalProductionSponge);
                        PhosInsponge += (result.PhosInSpong * result.TotalProductionSponge);
                       
                          
                        

                        // Set the percentage for the corresponding material
                        materialPercentages[result.MaterialName] = result.NetKlinUsesPer;
                    }

                    // Write the material percentages in their respective columns
                    for (int i = 0; i < allMaterialNames.Count; i++)
                    {
                        worksheet.Cells[row, materialColStart + i].Value = materialPercentages[allMaterialNames[i]];
                    }
         

                    sconsucoal= totalcoalreq / totalProduction;
           

                    //Main value for print Sponge Cost A + B + C + D + E
                    if (singleRowData != null)
                    {
                      dolomitecst = singleRowData.DolomiteRate;

                        mgfexpence = (singleRowData.MgfExpence / countRegisteredKlin) * model.NumberOfRunningKilns;
                        EfixedCost = singleRowData.FixedCost / totalProduction;
                    }

                    btotalcoalcst = sconsucoal * CoalCost;
                    totalironconve = totalnetironcst / totalProduction;

                    Dmgfexpence = mgfexpence / totalProduction;

                    totalSpongeCost = dolomitecst + EfixedCost + btotalcoalcst + totalironconve + Dmgfexpence;

                    TGangeinsponge = Gangeinsponge / totalProduction;
                    TPhosInsponge = PhosInsponge / totalProduction;



                    // Write the totals for Sponge Cost ABCD, Guenge, and Total Production
                    worksheet.Cells[row, materialColStart + allMaterialNames.Count].Value = totalSpongeCost;
                    worksheet.Cells[row, materialColStart + allMaterialNames.Count + 1].Value = TGangeinsponge;
                    worksheet.Cells[row, materialColStart + allMaterialNames.Count + 2].Value = TPhosInsponge;
                    worksheet.Cells[row, materialColStart + allMaterialNames.Count + 3].Value = totalProduction;


                    // Move to the next row for the next combination
                    row++;
                }

                var excelBytes = package.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FeedRateResults.xlsx");
            }
        }
















        //[HttpPost]
        //public IActionResult PerformCalculations([FromBody] FeedRateViewModel model)
        //{


        //    if (model == null)
        //    {
        //        return BadRequest("Invalid model");
        //    }

        //    var results = new List<MaterialCalculationResult>();
        //    int combinationId = 1;

        //    if (model.IsAutoGeneration)
        //    {
        //        // Auto-generate all possible combinations with a 5% increment
        //        var combinations = GenerateCombinations(model.SelectedMaterials, 5m);

        //        foreach (var combination in combinations)
        //        {
        //            var combinationResults = PerformSingleCombinationCalculation(combination, model.AverageFeedRate, model.NumberOfRunningKilns, model.Cfe, model.Moisture, model.SelectedFC, model.CoalCost);

        //            foreach (var result in combinationResults)
        //            {
        //                result.CombinationId = combinationId; // Assign a unique combination ID
        //            }

        //            results.AddRange(combinationResults);
        //            combinationId++;
        //        }

        //        // Group results by CombinationId to render combination-wise
        //        var groupedResults = results
        //            .GroupBy(r => r.CombinationId)
        //            .Select(grp => new
        //            {
        //                CombinationId = grp.Key,
        //                CombinationResults = grp.OrderBy(r => r.MaterialName).ToList() // Optional: Order materials within each combination
        //            })
        //            .OrderBy(g => g.CombinationId) // Sort by CombinationId
        //            .ToList();

        //        return Json(new { success = true, message = "Data processed successfully", combinations = groupedResults });
        //    }

        //    else
        //    {
        //        // For manual input, only one combination to process
        //        var singleCombinationResult = PerformSingleCombinationCalculation(model.MaterialPercentages, model.AverageFeedRate, model.NumberOfRunningKilns, model.Cfe, model.Moisture, model.SelectedFC, model.CoalCost);

        //        foreach (var result in singleCombinationResult)
        //        {
        //            result.CombinationId = combinationId; // Assign ID for single combination
        //        }

        //        results.AddRange(singleCombinationResult);
        //    }

        //    model.Results = results;

        //    return Json(new { success = true, message = "Data processed successfully", results = results });
        //}

        private List<Dictionary<string, decimal>> GenerateCombinations(List<string> selectedMaterials, decimal increment)
        {
            var combinations = new List<Dictionary<string, decimal>>();
            GenerateCombinationsRecursive(selectedMaterials, increment, 100m, new Dictionary<string, decimal>(), combinations, 0);
            return combinations;
        }

        private void GenerateCombinationsRecursive(List<string> selectedMaterials, decimal increment, decimal remainingPercentage, Dictionary<string, decimal> currentCombination, List<Dictionary<string, decimal>> combinations, int index)
        {
            // If we have processed all materials, check if the total is 100%
            if (index == selectedMaterials.Count - 1)
            {
                // Assign remaining percentage to the last material
                var lastMaterial = selectedMaterials[index];
                currentCombination[lastMaterial] = remainingPercentage;

                // Add a copy of the valid combination to the result list
                combinations.Add(new Dictionary<string, decimal>(currentCombination));
                return;
            }

            // Loop over possible percentage values for the current material
            for (decimal percentage = 0; percentage <= remainingPercentage; percentage += increment)
            {
                // Set the current material's percentage
                var currentMaterial = selectedMaterials[index];
                currentCombination[currentMaterial] = percentage;

                // Recursively calculate combinations for the next material
                GenerateCombinationsRecursive(selectedMaterials, increment, remainingPercentage - percentage, currentCombination, combinations, index + 1);
            }
        }

        private  List<MaterialCalculationResult> PerformSingleCombinationCalculation(Dictionary<string, decimal> materialPercentages, decimal averageFeedRate, int numberOfRunningKilns, decimal CFe, decimal moisture, decimal Fc, decimal coalCost)
        {
            var results = new List<MaterialCalculationResult>();

            decimal totalIssueSum = 0;
            decimal netKlinSum = 0;


            // Cache InputOperands data to avoid querying the database multiple times
            var inputOperands = _context.InputOperands.ToDictionary(m => m.Sidename, m => m);

            // Cache the single row data from PriceOfMaterials
            var singleRowData = _context.PriceOfMaterials.SingleOrDefault();
            if (singleRowData == null)
            {
                throw new InvalidOperationException("The single row data could not be found.");
            }

            // Cache the total count of registered kilns to avoid multiple async calls
            int countRegisteredKlin = _context.TPDInfos.Count();


            foreach (var material in materialPercentages)
            {
                if (material.Value > 0)
                {
                    inputOperands.TryGetValue(material.Key, out var operand);
                    decimal totalFeedrate = numberOfRunningKilns * averageFeedRate * 24;
                    var inputPercentage = material.Value;

                    if (operand != null)
                    {
                        var totalIssue = totalFeedrate * (inputPercentage / (100 - operand.FineLoss) / (100 - operand.GroundLoss)) * 100;
                        totalIssueSum += totalIssue;

                        var result = new MaterialCalculationResult
                        {
                            MaterialName = operand.Sidename,
                            TotalIssue = totalIssue,
                            // Store other necessary fields...
                        };

                        results.Add(result);
                    }
                }
            }

            foreach (var result in results)
            {
                inputOperands.TryGetValue(result.MaterialName, out var operand);
                if (operand != null)
                {
                    var rmhIssue = totalIssueSum * (result.TotalIssue / 100);
                    var netKlins = rmhIssue - ((operand.GroundLoss * rmhIssue / 100)) - ((((rmhIssue - ((operand.GroundLoss * rmhIssue / 100)))) * operand.FineLoss / 100));
                    result.RmhIssue = rmhIssue;
                    result.NetKlin = netKlins;
                    netKlinSum += netKlins;

                    var TNetFeedKlin = result.NetKlin * (numberOfRunningKilns * averageFeedRate * 24) / 100;
                    // Perform other calculations...

                    result.TotalNetIronCost = 15200;
                }
            }

            decimal sumOfTotalSpong = 0;

            var additionalResults = new List<MaterialCalculationResult>();

           

            int CountRegisteredKlin = _context.TPDInfos.CountAsync().Result;

            if (singleRowData == null)
            {
                throw new InvalidOperationException("The single row data could not be found.");
            }

            foreach (var result in results)
            {
                var operand = _context.InputOperands.FirstOrDefault(m => m.Sidename == result.MaterialName);

                var FineRealization = ((100 - operand.GroundLoss) * operand.FineLoss) / 100;
                var UsableOre = 100 - (operand.GroundLoss + FineRealization);
                var PriceUsableIron = (operand.IronPrice - (operand.FinesRealisationValue * (FineRealization) / 100)) / UsableOre * 100;

                // Example additional calculation using netKlinSum
                var additionalCalculation = (result.NetKlin / netKlinSum) * 100;

                var TNetFeedKlin = additionalCalculation * (numberOfRunningKilns * averageFeedRate * 24) / 100;
                var TIronIssue = (TNetFeedKlin * 100 / (100 - operand.FineLoss)) * 100 / (100 - operand.GroundLoss);
                var TCostIron = TIronIssue * operand.IronPrice;
                var TNetIronCost = (TNetFeedKlin * PriceUsableIron);
                var TFet = TNetFeedKlin * operand.FeT * (1 - operand.Moisture / 100);

                var denominator = (Fc / 100) * (1 - (moisture / 100));

                if (denominator == 0)
                {
                    throw new InvalidOperationException("The denominator in the calculation is zero.");
                }

                var TCoalRequired = (TFet * CFe) / denominator;

                var TotalPSpong = TNetFeedKlin * operand.Yield;

                var SConCoal = TCoalRequired / TotalPSpong;

                //var TCoalRequired = (TFet * CFe) / ((Fc/100) *(1 - Moisture));

                decimal GetMgfExepence = (singleRowData.MgfExpence / CountRegisteredKlin) * numberOfRunningKilns;
                result.NetKlinUsesPer = additionalCalculation;
                result.TotalNetFeedKlin = TNetFeedKlin;
                result.TotalIronIssue = TIronIssue;
                result.TotalCostIron = TCostIron;
                result.TotalNetIronCost = TNetIronCost;
                result.TotalFet = TFet;
                result.TotalCoalRequired = TCoalRequired;
                result.SConsuCoal = SConCoal;
                result.TotalProductionSponge = TotalPSpong;


                result.Yeald = operand.Yield;
                result.TotalCoalCost = TCoalRequired * coalCost;
                result.ATotalCostIronplusCon = TNetIronCost / operand.Yield;
                result.BTotalCoalCost = SConCoal * coalCost;
                result.GuengeInSpong = operand.Gangue / operand.Yield;
                result.PhosInSpong = operand.Phos / operand.Yield;
                result.FeoInSpong = ((operand.FeT * 1.428m) - operand.FeMSponge) * 1.286m * 100;
                result.CoalCost = coalCost;


                result.FemPerinSpong = operand.FeMSponge;
                //result.PerProductionOfSpong = TotalPSpong;
                result.NetYeildOnSpong = operand.FeMSponge - (operand.YLoss + operand.TransferLoss);
                result.TotalLmPro = (operand.FeMSponge - (operand.YLoss + operand.TransferLoss)) * TotalPSpong;
                result.PhosLM = (operand.Phos / operand.Yield) / operand.FeMSponge;

                result.DolomiteCost = singleRowData.DolomiteRate;
                result.MgfExp = GetMgfExepence / TotalPSpong;
                result.FixedCost = singleRowData.FixedCost / TotalPSpong;
                result.SpongeCostABCDE = (singleRowData.DolomiteRate) + (GetMgfExepence / TotalPSpong) + (singleRowData.FixedCost / TotalPSpong) + (SConCoal * coalCost) + (TNetIronCost / operand.Yield);
                result.LmCost = ((singleRowData.DolomiteRate) + (GetMgfExepence / TotalPSpong) + (singleRowData.FixedCost / TotalPSpong) + (SConCoal * coalCost) + (TNetIronCost / operand.Yield)) / ((operand.FeMSponge - (operand.YLoss + operand.TransferLoss)));
                result.MgfOther = GetMgfExepence;
                result.FixedCostOther = singleRowData.FixedCost;
                sumOfTotalSpong += TotalPSpong;
            }

            foreach (var result in results)
            {
                result.PerProductionOfSpong = sumOfTotalSpong != 0 ? (result.TotalProductionSponge / sumOfTotalSpong) * 100 : 0;
            }



            return results;
        }






        //[HttpPost]
        //public IActionResult PerformCalculations([FromBody] FeedRateViewModel model)
        //{
        //    if (model == null)
        //    {
        //        return BadRequest("Invalid model");
        //    }

        //    var results = new List<MaterialCalculationResult>();
        //    int combinationId = 1;

        //    if (model.IsAutoGeneration)
        //    {
        //        // Auto-generate all possible combinations with a 5% increment
        //        var combinations = GenerateCombinations(model.SelectedMaterials, 5m);


        //        foreach (var combination in combinations)
        //        {
        //            var combinationResults = PerformSingleCombinationCalculation(combination, model.AverageFeedRate, model.NumberOfRunningKilns, model.Cfe, model.Moisture, model.SelectedFC, model.CoalCost);

        //            foreach (var result in combinationResults)
        //            {
        //                result.CombinationId = combinationId; // Assign a unique combination ID
        //            }

        //            results.AddRange(combinationResults);
        //            combinationId++;
        //        }


        //        results = results.Take(10).ToList();
        //        model.Results = results;

        //        Console.WriteLine($"Top 10 results count: {results.Count}");

        //        return Json(new { success = true, message = "Data processed successfully", results = results });


        //    }
        //    else
        //    {
        //        // For manual input, only one combination to process
        //        var singleCombinationResult = PerformSingleCombinationCalculation(model.MaterialPercentages, model.AverageFeedRate, model.NumberOfRunningKilns, model.Cfe, model.Moisture, model.SelectedFC, model.CoalCost);

        //        foreach (var result in singleCombinationResult)
        //        {
        //            result.CombinationId = combinationId; // Assign ID for single combination
        //        }

        //        results.AddRange(singleCombinationResult);
        //    }

        //    model.Results = results;

        //    return Json(new { success = true, message = "Data processed successfully", results = results });
        //}

        //private List<Dictionary<string, decimal>> GenerateCombinations(List<string> selectedMaterials, decimal increment)
        //{
        //    var combinations = new List<Dictionary<string, decimal>>();
        //    GenerateCombinationsRecursive(selectedMaterials, increment, 100m, new Dictionary<string, decimal>(), combinations, 0);
        //    return combinations;
        //}

        //private void GenerateCombinationsRecursive(List<string> selectedMaterials, decimal increment, decimal remainingPercentage, Dictionary<string, decimal> currentCombination, List<Dictionary<string, decimal>> combinations, int index)
        //{
        //    // If we have processed all materials, check if the total is 100%
        //    if (index == selectedMaterials.Count - 1)
        //    {
        //        // Assign remaining percentage to the last material
        //        var lastMaterial = selectedMaterials[index];
        //        currentCombination[lastMaterial] = remainingPercentage;

        //        // Add a copy of the valid combination to the result list
        //        combinations.Add(new Dictionary<string, decimal>(currentCombination));
        //        return;
        //    }

        //    // Loop over possible percentage values for the current material
        //    for (decimal percentage = 0; percentage <= remainingPercentage; percentage += increment)
        //    {
        //        // Set the current material's percentage
        //        var currentMaterial = selectedMaterials[index];
        //        currentCombination[currentMaterial] = percentage;

        //        // Recursively calculate combinations for the next material
        //        GenerateCombinationsRecursive(selectedMaterials, increment, remainingPercentage - percentage, currentCombination, combinations, index + 1);
        //    }
        //}

        //private List<MaterialCalculationResult> PerformSingleCombinationCalculation(Dictionary<string, decimal> materialPercentages, decimal averageFeedRate, int numberOfRunningKilns, decimal CFe, decimal moisture, decimal Fc, decimal coalCost)
        //{
        //    var results = new List<MaterialCalculationResult>();

        //    decimal totalIssueSum = 0;
        //    decimal netKlinSum = 0;

        //    foreach (var material in materialPercentages)
        //    {
        //        if (material.Value > 0)
        //        {
        //            var operand = _context.InputOperands.FirstOrDefault(m => m.Sidename == material.Key);
        //            decimal totalFeedrate = numberOfRunningKilns * averageFeedRate * 24;
        //            var inputPercentage = material.Value;

        //            if (operand != null)
        //            {
        //                var totalIssue = totalFeedrate * (inputPercentage / (100 - operand.FineLoss) / (100 - operand.GroundLoss)) * 100;
        //                totalIssueSum += totalIssue;

        //                var result = new MaterialCalculationResult
        //                {
        //                    MaterialName = operand.Sidename,
        //                    TotalIssue = totalIssue,
        //                    // Store other necessary fields...
        //                };

        //                results.Add(result);
        //            }
        //        }
        //    }

        //    foreach (var result in results)
        //    {
        //        var operand = _context.InputOperands.FirstOrDefault(m => m.Sidename == result.MaterialName);
        //        if (operand != null)
        //        {
        //            var rmhIssue = totalIssueSum * (result.TotalIssue / 100);
        //            var netKlins = rmhIssue - ((operand.GroundLoss * rmhIssue / 100)) - ((((rmhIssue - ((operand.GroundLoss * rmhIssue / 100)))) * operand.FineLoss / 100));
        //            result.RmhIssue = rmhIssue;
        //            result.NetKlin = netKlins;
        //            netKlinSum += netKlins;

        //            var TNetFeedKlin = result.NetKlin * (numberOfRunningKilns * averageFeedRate * 24) / 100;
        //            // Perform other calculations...

        //            result.TotalNetIronCost = 15200;
        //        }
        //    }

        //    decimal sumOfTotalSpong = 0;

        //    var additionalResults = new List<MaterialCalculationResult>();

        //    var singleRowData = _context.PriceOfMaterials.SingleOrDefault();

        //    int CountRegisteredKlin = _context.TPDInfos.CountAsync().Result;

        //    if (singleRowData == null)
        //    {
        //        throw new InvalidOperationException("The single row data could not be found.");
        //    }

        //    foreach (var result in results)
        //    {
        //        var operand = _context.InputOperands.FirstOrDefault(m => m.Sidename == result.MaterialName);

        //        var FineRealization = ((100 - operand.GroundLoss) * operand.FineLoss) / 100;
        //        var UsableOre = 100 - (operand.GroundLoss + FineRealization);
        //        var PriceUsableIron = (operand.IronPrice - (operand.FinesRealisationValue * (FineRealization) / 100)) / UsableOre * 100;

        //        // Example additional calculation using netKlinSum
        //        var additionalCalculation = (result.NetKlin / netKlinSum) * 100;

        //        var TNetFeedKlin = additionalCalculation * (numberOfRunningKilns * averageFeedRate * 24) / 100;
        //        var TIronIssue = (TNetFeedKlin * 100 / (100 - operand.FineLoss)) * 100 / (100 - operand.GroundLoss);
        //        var TCostIron = TIronIssue * operand.IronPrice;
        //        var TNetIronCost = (TNetFeedKlin * PriceUsableIron);
        //        var TFet = TNetFeedKlin * operand.FeT * (1 - operand.Moisture / 100);

        //        var denominator = (Fc / 100) * (1 - (moisture / 100));

        //        if (denominator == 0)
        //        {
        //            throw new InvalidOperationException("The denominator in the calculation is zero.");
        //        }

        //        var TCoalRequired = (TFet * CFe) / denominator;

        //        var TotalPSpong = TNetFeedKlin * operand.Yield;

        //        var SConCoal = TCoalRequired / TotalPSpong;

        //        //var TCoalRequired = (TFet * CFe) / ((Fc/100) *(1 - Moisture));

        //        decimal GetMgfExepence = (singleRowData.MgfExpence / CountRegisteredKlin) * numberOfRunningKilns;
        //        result.NetKlinUsesPer = additionalCalculation;
        //        result.TotalNetFeedKlin = TNetFeedKlin;
        //        result.TotalIronIssue = TIronIssue;
        //        result.TotalCostIron = TCostIron;
        //        result.TotalNetIronCost = TNetIronCost;
        //        result.TotalFet = TFet;
        //        result.TotalCoalRequired = TCoalRequired;
        //        result.SConsuCoal = SConCoal;
        //        result.TotalProductionSponge = TotalPSpong;


        //        result.Yeald = operand.Yield;
        //        result.TotalCoalCost = TCoalRequired * coalCost;
        //        result.ATotalCostIronplusCon = TNetIronCost / operand.Yield;
        //        result.BTotalCoalCost = SConCoal * coalCost;
        //        result.GuengeInSpong = operand.Gangue / operand.Yield;
        //        result.PhosInSpong = operand.Phos / operand.Yield;
        //        result.FeoInSpong = ((operand.FeT * 1.428m) - operand.FeMSponge) * 1.286m * 100;
        //        result.CoalCost = coalCost;


        //        result.FemPerinSpong = operand.FeMSponge;
        //        //result.PerProductionOfSpong = TotalPSpong;
        //        result.NetYeildOnSpong = operand.FeMSponge - (operand.YLoss + operand.TransferLoss);
        //        result.TotalLmPro = (operand.FeMSponge - (operand.YLoss + operand.TransferLoss)) * TotalPSpong;
        //        result.PhosLM = (operand.Phos / operand.Yield) / operand.FeMSponge;

        //        result.DolomiteCost = singleRowData.DolomiteRate;
        //        result.MgfExp = GetMgfExepence / TotalPSpong;
        //        result.FixedCost = singleRowData.FixedCost / TotalPSpong;
        //        result.SpongeCostABCDE = (singleRowData.DolomiteRate) + (GetMgfExepence / TotalPSpong) + (singleRowData.FixedCost / TotalPSpong) + (SConCoal * coalCost) + (TNetIronCost / operand.Yield);
        //        result.LmCost = ((singleRowData.DolomiteRate) + (GetMgfExepence / TotalPSpong) + (singleRowData.FixedCost / TotalPSpong) + (SConCoal * coalCost) + (TNetIronCost / operand.Yield)) / ((operand.FeMSponge - (operand.YLoss + operand.TransferLoss)));
        //        result.MgfOther = GetMgfExepence;
        //        result.FixedCostOther = singleRowData.FixedCost;
        //        sumOfTotalSpong += TotalPSpong;
        //    }

        //    foreach (var result in results)
        //    {
        //        result.PerProductionOfSpong = sumOfTotalSpong != 0 ? (result.TotalProductionSponge / sumOfTotalSpong) * 100 : 0;
        //    }



        //    return results;
        //}





        //[HttpPost]
        //public IActionResult PerformCalculations([FromBody] FeedRateViewModel model)
        //{
        //    if (model == null)
        //    {
        //        return BadRequest("Invalid model");
        //    }

        //    // Process the data
        //    var averageFeedRate = model.AverageFeedRate;
        //    var numberOfRunningKilns = model.NumberOfRunningKilns;
        //    var materialPercentages = model.MaterialPercentages;
        //    var CFe = model.Cfe;
        //    var Moisture=model.Moisture;
        //    decimal Fc = model.SelectedFC;
        //    //var CoalCost=model.CoalCost;

        //    var results = new List<MaterialCalculationResult>();
        //    var totalIssues = new Dictionary<string, decimal>();
        //    decimal totalIssueSum = 0;
        //    decimal netKlinSum = 0;

        //    // Calculate TotalIssue for each material
        //    foreach (var material in materialPercentages)
        //    {
        //        if (material.Value > 0)
        //        {
        //            var operand = _context.InputOperands.FirstOrDefault(m => m.Sidename == material.Key);
        //            decimal totalFeedrate = numberOfRunningKilns * averageFeedRate * 24; // Total feedrate
        //            var inputPercentage = material.Value; // Input %

        //            if (operand != null)
        //            {
        //                // Calculate TotalIssue for the material
        //                var totalIssue = totalFeedrate * (inputPercentage / (100 - operand.FineLoss) / (100 - operand.GroundLoss)) * 100;


        //                // Add to the total issue sum
        //                totalIssueSum += totalIssue;

        //                // Store the TotalIssue for this material
        //                totalIssues[operand.Sidename] = totalIssue;

        //                // Store the results for the material
        //                results.Add(new MaterialCalculationResult
        //                {
        //                    MaterialName = operand.Sidename,
        //                    TotalIssue = totalIssue,
        //                    InputPercentage = inputPercentage // Add this property to store the input percentage
        //                });
        //            }
        //        }
        //    }

        //    // Calculate RmhIssue for each material based on the totalIssueSum and input percentage
        //    foreach (var result in results)
        //    {
        //        var operand = _context.InputOperands.FirstOrDefault(m => m.Sidename == result.MaterialName);
        //        if (operand != null)
        //        {
        //            // Calculate RmhIssue using the totalIssueSum and the material's input percentage
        //            var rmhIssue = totalIssueSum * (result.InputPercentage / 100) ;
        //            var netKlins = ((rmhIssue - ((operand.GroundLoss * rmhIssue/100))) - ((((rmhIssue - ((operand.GroundLoss * rmhIssue/100)))) * operand.FineLoss/100)));

        //            // Update the result with RmhIssue
        //            result.RmhIssue = rmhIssue;
        //            result.NetKlin= netKlins;
        //            netKlinSum += netKlins;
        //        }
        //    }
        //    decimal sumOfTotalSpong = 0;

        //    var additionalResults = new List<MaterialCalculationResult>();

        //    var singleRowData = _context.PriceOfMaterials.SingleOrDefault();

        //    int CountRegisteredKlin = _context.TPDInfos.CountAsync().Result;

        //    if (singleRowData == null)
        //    {
        //        throw new InvalidOperationException("The single row data could not be found.");
        //    }

        //    foreach (var result in results)
        //    {
        //        var operand = _context.InputOperands.FirstOrDefault(m => m.Sidename == result.MaterialName);

        //        var FineRealization = ((100 - operand.GroundLoss) * operand.FineLoss) / 100;
        //        var UsableOre=100-(operand.GroundLoss+FineRealization);
        //        var PriceUsableIron=(operand.IronPrice-(operand.FinesRealisationValue*(FineRealization)/100))/UsableOre*100;

        //        // Example additional calculation using netKlinSum
        //        var additionalCalculation = (result.NetKlin/netKlinSum)*100;

        //        var TNetFeedKlin=additionalCalculation * (numberOfRunningKilns * averageFeedRate * 24)/100;
        //        var TIronIssue=(TNetFeedKlin*100/(100-operand.FineLoss))*100/(100-operand.GroundLoss);
        //        var TCostIron = TIronIssue * operand.IronPrice;
        //        var TNetIronCost = (TNetFeedKlin * PriceUsableIron);
        //        var TFet = TNetFeedKlin * operand.FeT * (1 - operand.Moisture / 100);

        //        var denominator = (Fc / 100) * (1 - (Moisture/100));

        //        if (denominator == 0)
        //        {
        //            throw new InvalidOperationException("The denominator in the calculation is zero.");
        //        }

        //        var TCoalRequired = (TFet * CFe) / denominator;

        //        var TotalPSpong = TNetFeedKlin * operand.Yield;

        //        var SConCoal = TCoalRequired / TotalPSpong;

        //        //var TCoalRequired = (TFet * CFe) / ((Fc/100) *(1 - Moisture));

        //        decimal GetMgfExepence=(singleRowData.MgfExpence/CountRegisteredKlin)*numberOfRunningKilns;
        //        result.NetKlinUsesPer= additionalCalculation;
        //        result.TotalNetFeedKlin= TNetFeedKlin;
        //        result.TotalIronIssue= TIronIssue;
        //        result.TotalCostIron= TCostIron;
        //        result.TotalNetIronCost = TNetIronCost;
        //        result.TotalFet=TFet;
        //        result.TotalCoalRequired = TCoalRequired;
        //        result.SConsuCoal= SConCoal;
        //        result.TotalProductionSponge=TotalPSpong;


        //        result.Yeald = operand.Yield;
        //        result.TotalCoalCost=TCoalRequired*model.CoalCost;
        //        result.ATotalCostIronplusCon = TNetIronCost / operand.Yield;
        //        result.BTotalCoalCost=SConCoal*model.CoalCost;
        //        result.GuengeInSpong=operand.Gangue/operand.Yield;
        //        result.PhosInSpong=operand.Phos/operand.Yield;
        //        result.FeoInSpong = ((operand.FeT * 1.428m) -operand.FeMSponge) * 1.286m*100;
        //        result.CoalCost=model.CoalCost;


        //        result.FemPerinSpong = operand.FeMSponge;
        //        //result.PerProductionOfSpong = TotalPSpong;
        //        result.NetYeildOnSpong = operand.FeMSponge-(operand.YLoss + operand.TransferLoss);
        //        result.TotalLmPro= (operand.FeMSponge - (operand.YLoss + operand.TransferLoss))* TotalPSpong;
        //        result.PhosLM = (operand.Phos / operand.Yield)/operand.FeMSponge;

        //        result.DolomiteCost = singleRowData.DolomiteRate;
        //        result.MgfExp = GetMgfExepence / TotalPSpong;
        //        result.FixedCost= singleRowData.FixedCost/ TotalPSpong;
        //        result.SpongeCostABCDE = (singleRowData.DolomiteRate) + (GetMgfExepence / TotalPSpong) + (singleRowData.FixedCost / TotalPSpong) + (SConCoal * model.CoalCost) + (TNetIronCost / operand.Yield);
        //        result.LmCost = ((singleRowData.DolomiteRate) + (GetMgfExepence / TotalPSpong) + (singleRowData.FixedCost / TotalPSpong) + (SConCoal * model.CoalCost) + (TNetIronCost / operand.Yield)) / ((operand.FeMSponge - (operand.YLoss + operand.TransferLoss)));
        //        result.MgfOther = GetMgfExepence;
        //        result.FixedCostOther = singleRowData.FixedCost;
        //        sumOfTotalSpong += TotalPSpong;
        //    }

        //    foreach (var result in results)
        //    {
        //        result.PerProductionOfSpong = sumOfTotalSpong != 0 ? (result.TotalProductionSponge / sumOfTotalSpong) * 100 : 0;
        //    }

        //    return Json(new { success = true, message = "Data processed successfully", results = results });
        //}

    }
}

