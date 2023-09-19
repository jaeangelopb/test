using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class TrialBalance
{
    public int TrialBalanceId { get; set; }

    public int EntityId { get; set; }

    public int AccountId { get; set; }

    public string TaxYear { get; set; }

    public DateTime Period { get; set; }

    public decimal? Amount { get; set; }

    public string SourceSystem { get; set; }

    public string FileName { get; set; }

    public bool Active { get; set; }

    public bool Delete { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual Account Account { get; set; }

    //public virtual Entity Entity { get; set; }
}
