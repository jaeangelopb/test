using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class FixedAssetRegister
{
    public int Farid { get; set; }

    public int EntityId { get; set; }

    public string AssetClass { get; set; }

    public string AssetDescription { get; set; }

    public decimal OpeningBalanceRolledForward { get; set; }

    public decimal OpeningBalanceAdjustment { get; set; }

    public decimal PriorYearAdjustments { get; set; }

    public decimal Additions { get; set; }

    public decimal Depreciation { get; set; }

    public decimal Disposals { get; set; }

    public decimal IntercompanyTransfers { get; set; }

    public decimal Other { get; set; }

    public decimal ClosingBalance { get; set; }

    public decimal Proceeds { get; set; }

    public bool IsAccounting { get; set; }

    public string SourceSystem { get; set; }

    public string Comments { get; set; }

    public bool Active { get; set; }

    public bool Delete { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual Entity Entity { get; set; }
}
