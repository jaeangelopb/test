using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class ConduitForeignIncome
    {

        public int? CFIId { get; set; }

        public int EntityId { get; set; }

        public string Period { get; set; }
        public DateTime? Date { get; set; }

        public string? Description { get; set; }

        public double? ForeignDividendReceivedInLocalCurrency { get; set; }

        public double? ExchangeRate { get; set; }

        public double? ForeignDividendPaidInAUD { get; set; }
        public double? Balance { get; set; }
        public string? Process { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedOn { get; set; }
    }
}
