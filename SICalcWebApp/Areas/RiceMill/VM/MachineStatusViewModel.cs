namespace SICalcWebApp.Areas.RiceMill.VM
{
    public class MachineStatusViewModel
    {
        public string HandiBatchId { get; set; }
        public string HandiStatus { get; set; }
        public DateTime? HandiStartTime { get; set; }

        public string DryerBatchId { get; set; }
        public string DryerStatus { get; set; }
        public DateTime? DryerStartTime { get; set; }



        public string SortexBatchId { get; set; }
        public string SortexStatus { get; set; }
        public DateTime? SortexStartTime { get; set; }

        public string MillingBatchId { get; set; }
        public string MillingStatus { get; set; }
        public DateTime? MillingStartTime { get; set; }




        public int QualityStageCount { get; set; }
        public double AvgMachineBroken { get; set; }
        public double AvgManualBroken { get; set; }
        public double AvgMachineMoisture { get; set; }
        public double AvgManualMoisture { get; set; }
        public double AvgMoistureChottiMachine { get; set; }
        public double AvgMoistureChottiMachineManual { get; set; }
        public double AvgMillWeightment { get; set; }

        // Properties for Damage & Discolour (Only for USNA batches)
        public double AvgMachineDamage { get; set; }
        public double AvgManualDamage { get; set; }
        public double AvgMachineDiscolour { get; set; }
        public double AvgManualDiscolour { get; set; }


        //this is section for sortex
        public int QualityStageSortexCount { get; set; }
        public double AvgSortexMachineBroken { get; set; }
        public double AvgSortexManualBroken { get; set; }
        public double AvgSortexMachineMoisture { get; set; }
        public double AvgSortexManualMoisture { get; set; }
        public double AvgSortexMoistureChottiMachine { get; set; }
        public double AvgSortexMoistureChottiMachineManual { get; set; }


        // Properties for Damage & Discolour (Only for USNA batches)
        public double AvgSortexMachineDamage { get; set; }
        public double AvgSortexManualDamage { get; set; }
        public double AvgSortexMachineDiscolour { get; set; }
        public double AvgSortexManualDiscolour { get; set; }

        public double Avg30Second { get; set; }
        public double Avg10minutes { get; set; }


        public List<RemainingStock> StockList { get; set; } = new List<RemainingStock>();

        public List<BunkerStatusVM> MillBList { get; set; } = new List<BunkerStatusVM>();

    }

}
