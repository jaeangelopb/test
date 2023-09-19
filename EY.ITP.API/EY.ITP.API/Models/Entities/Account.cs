using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class Account
{
    public int AccountId { get; set; }

    public int AccountTypeId { get; set; }

    public int Gaapid { get; set; }

    public int TaxSensitivityId { get; set; }

    public int? DisclosuresId { get; set; }

    public int? Tacid { get; set; }

    public string AccountCode { get; set; }

    public string AccountName { get; set; }

    public string Risk { get; set; }

    public string AccountingTreatment { get; set; }

    public string TaxTreatment { get; set; }

    public string? Comments { get; set; }

    public string VersionName { get; set; }

    public string SourceSystem { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual AccountType AccountType { get; set; }

    //public virtual Disclosure? Disclosures { get; set; }

    //public virtual ICollection<EntityCoamapping> EntityCoamappings { get; } = new List<EntityCoamapping>();

    //public virtual Gaap Gaap { get; set; }

    //public virtual TaxAdjustmentCategory? Tac { get; set; }

    //public virtual TaxSensitivity TaxSensitivity { get; set; }

    //public virtual ICollection<TrialBalance> TrialBalances { get; }
}
