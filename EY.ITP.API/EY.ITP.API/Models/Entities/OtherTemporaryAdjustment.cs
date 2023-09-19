using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class OtherTemporaryAdjustment
{
    public int Otaid { get; set; }

    public int EntityId { get; set; }

    public int Tacid { get; set; }

    public string TaxYear { get; set; }

    public DateTime Period { get; set; }

    public decimal OpeningBalanceRolledForward { get; set; }

    public decimal OpeningBalanceAdjustment { get; set; }

    public decimal UnderOverProvisionPriorYear { get; set; }

    public decimal Transfers { get; set; }

    public decimal OtherMovements { get; set; }

    public decimal BusinessAcquisition { get; set; }

    public decimal BusinessDisposal { get; set; }

    public decimal Equity { get; set; }

    public decimal ClosingBalance { get; set; }

    public bool IsAccounting { get; set; }

    public string Comments { get; set; }

    public string? SourceSystem { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual Entity Entity { get; set; }

    //public virtual TaxAdjustmentCategory Tac { get; set; }
}
