using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class FrankingAccount
    {

        public int? FAId { get; set; }

        public int EntityId { get; set; }

        public string Period { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }

        public double? FrankedDividendPaid { get; set; }

        public double? FrankedDividendReceived { get; set; }

        public double? TaxRefunds { get; set; }

        public double? TaxPayments { get; set; }

        public double? Balance { get; set; }
        public string? Process { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedOn { get; set; }
    }
}
