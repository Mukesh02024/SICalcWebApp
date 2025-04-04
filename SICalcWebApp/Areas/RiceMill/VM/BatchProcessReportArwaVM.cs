namespace SICalcWebApp.Areas.RiceMill.VM
{
    public class BatchProcessReportArwaVM
    {

        public string BatchId { get; set; }
        public string ProcessType { get; set; }
        public string PaddyType { get; set; }
        // Handi Process
        public string HandiType { get; set; }
        public double Pressure { get; set; }
        public string HandiStaff { get; set; }
        public DateTime HandiStartTime { get; set; }
        public DateTime HandiEndTime { get; set; }

        public double HandiDelay { get; set; }  // Stores time in hours (e.g., 3.5 for 3 hours 30 mins)
        public double HandiTakenTime { get; set; }

        // Milling Process
        public DateTime MillStartTime { get; set; }
        public DateTime MillEndTime { get; set; }
        public string MillBunkerName { get; set; }
        public string SortexUnloadBunk { get; set; }
        public string MillingStaff { get; set; }

        public double MillingDelay { get; set; }
        public double MillingTakenTime { get; set; }

        // Sortex Process
        public DateTime SortexStartTime { get; set; }
        public DateTime SortexEndTime { get; set; }
        public string SortexloadBunk { get; set; }
        public string SortexStaff { get; set; }

        public double SortexDelay { get; set; }
        public double SortexTakenTime { get; set; }
    }
}
