namespace EY.ITP.API.Models.Entities
{
    public class PAAdjustmentsSave
    {
        public int? PAId { get; set; }
        public int EntityId { get; set; }

        public string Period { get; set; }

        public string YearIncurred { get; set; }

        public double? OpeningBalanceAdjustment { get; set; }

        public double? Additions { get; set; }

        public double? Disposals { get; set; }

        public double? Other { get; set; }

        public string? Comments { get; set; }

        public string? Process { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
