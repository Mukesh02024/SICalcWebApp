using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Data;
using System.Data;

namespace SICalcWebApp.Areas.RiceMill.Services
{
    public class MillQualityService:IMillQualityService
    {
        private readonly ApplicationDbContext _context;

        public MillQualityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BatchRemainingStages>> GetRemainingBatchesAsync()
        {
            var batches = new List<BatchRemainingStages>();

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetRemainingBatches";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var remainingStages = new List<string>();

                            if (reader["S1"] != DBNull.Value && reader["S1"].ToString() != "") remainingStages.Add("S1");
                            if (reader["S2"] != DBNull.Value && reader["S2"].ToString() != "") remainingStages.Add("S2");
                            if (reader["S3"] != DBNull.Value && reader["S3"].ToString() != "") remainingStages.Add("S3");
                            if (reader["S4"] != DBNull.Value && reader["S4"].ToString() != "") remainingStages.Add("S4");

                            if (remainingStages.Count > 0)  // Only add batches with remaining stages
                            {
                                batches.Add(new BatchRemainingStages
                                {
                                    BatchID = reader["BatchID"].ToString(),
                                    RemainingStages = remainingStages
                                });
                            }
                        }
                    }
                }
            }
            return batches;
        }


        ////public async Task<List<BatchRemainingStages>> GetRemainingBatchesAsync()
        ////{
        ////    var batches = new List<BatchRemainingStages>();

        ////    using (var connection = _context.Database.GetDbConnection())
        ////    {
        ////        await connection.OpenAsync();

        ////        using (var command = connection.CreateCommand())
        ////        {
        ////            command.CommandText = "GetRemainingBatches";
        ////            command.CommandType = CommandType.StoredProcedure;

        ////            using (var reader = await command.ExecuteReaderAsync())
        ////            {
        ////                while (await reader.ReadAsync())
        ////                {
        ////                    var batch = new BatchRemainingStages
        ////                    {
        ////                        BatchID = reader["BatchID"].ToString(),
        ////                        RemainingStages = new List<string>()
        ////                    };

        ////                    if (!reader.IsDBNull(reader.GetOrdinal("S1"))) batch.RemainingStages.Add("S1");
        ////                    if (!reader.IsDBNull(reader.GetOrdinal("S2"))) batch.RemainingStages.Add("S2");
        ////                    if (!reader.IsDBNull(reader.GetOrdinal("S3"))) batch.RemainingStages.Add("S3");
        ////                    if (!reader.IsDBNull(reader.GetOrdinal("S4"))) batch.RemainingStages.Add("S4");

        ////                    batches.Add(batch);
        ////                }
        ////            }
        ////        }
        ////    }
        ////    return batches;
        ////}




        public async Task<bool> SubmitQualityAsync(MillQuality model)
        {
            try
            {
                _context.MillQualities.Add(model);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        //--------------this is sortex quality code---

        public async Task<List<BatchRemainingStages>> GetRemainingBatchesSortexAsync()
        {
            var batches = new List<BatchRemainingStages>();

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetRemainingBatchesForSortex";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            var remainingStages = new List<string>();
                           

                            if (reader["S1"] != DBNull.Value && reader["S1"].ToString() != "") remainingStages.Add("S1");
                            if (reader["S2"] != DBNull.Value && reader["S2"].ToString() != "") remainingStages.Add("S2");
                            if (reader["S3"] != DBNull.Value && reader["S3"].ToString() != "") remainingStages.Add("S3");
                            if (reader["S4"] != DBNull.Value && reader["S4"].ToString() != "") remainingStages.Add("S4");

                            if (remainingStages.Count > 0)  // Only add batches with remaining stages
                            {
                                batches.Add(new BatchRemainingStages
                                {
                                    BatchID = reader["BatchID"].ToString(),
                                    RemainingStages = remainingStages
                                });
                            }
                        }
                    }
                }


            }
            return batches;
        }




        public async Task<bool> SubmitQualitySortexAsync(MillQualitySortex model)
        {
            try
            {
                _context.MillQualitySortexes.Add(model);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }















    }


    public class BatchRemainingStages
    {
        public string BatchID { get; set; }
        public List<string> RemainingStages { get; set; }
    }
}
