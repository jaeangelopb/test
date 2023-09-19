using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_FAR_List")]
public class FARListResponse
{
    public int EntityId { get; set; }

    public string EntityCode { get; set; }

    public string EntityName { get; set; }

    public string EntityType { get; set; }

    public string AssetClass { get; set; }

    public string AssetDescription { get; set; }

    public decimal? AccountingOpeningBalanceRolledForward { get; set; }

    public decimal? AccountingOpeningBalanceAdjustment { get; set; }

    public decimal? AccountingPriorYearAdjustments { get; set; }

    public decimal? AccountingAdditions { get; set; }

    public decimal? AccountingDepreciation { get; set; }

    public decimal? AccountingDisposals { get; set; }

    public decimal? AccountingIntercompanyTransfers { get; set; }

    public decimal? AccountingOther { get; set; }

    public decimal? AccountingClosingBalance { get; set; }

    public decimal? AccountingProceeds { get; set; }

    public decimal? TaxOpeningBalanceRolledForward { get; set; }

    public decimal? TaxOpeningBalanceAdjustment { get; set; }

    public decimal? TaxPriorYearAdjustments { get; set; }

    public decimal? TaxAdditions { get; set; }

    public decimal? TaxDepreciation { get; set; }

    public decimal? TaxDisposals { get; set; }

    public decimal? TaxIntercompanyTransfers { get; set; }

    public decimal? TaxOther { get; set; }

    public decimal? TaxClosingBalance { get; set; }

    public decimal? TaxProceeds { get; set; }

    public string SourceSystem { get; set; }

    public string Comments { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }
}
