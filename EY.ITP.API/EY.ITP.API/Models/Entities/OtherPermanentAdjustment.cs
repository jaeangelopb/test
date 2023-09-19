using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class OtherPermanentAdjustment
{
    public int Opaid { get; set; }

    public int EntityId { get; set; }

    public int Tacid { get; set; }

    public string TaxYear { get; set; }

    public DateTime Period { get; set; }

    public bool IsAccounting { get; set; }

    public decimal Amount { get; set; }

    public string Comments { get; set; }

    public string? SourceSystem { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual Entity Entity { get; set; }

    //public virtual TaxAdjustmentCategory Tac { get; set; }
}
