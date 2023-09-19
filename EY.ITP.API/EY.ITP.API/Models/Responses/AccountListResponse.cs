using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_Account_List")]
public class AccountListResponse
{
    public int AccountId { get; set; }

    public string AccountType { get; set; }

    public string Gaap { get; set; }

    public string TaxSensitivity { get; set; }

    public string DisclosureJurisdiction { get; set; }

    public string? DisclosureLabel { get; set; }

    public string? DisclosureDescription { get; set; }

    public string? DisclosureSchedule { get; set; }

    public string? DisclosureGroup { get; set; }

    public int? DisclosureSorting { get; set; }

    public string? AdjustmentCategory { get; set; }

    public string AccountCode { get; set; }

    public string AccountName { get; set; }

    public string Risk { get; set; }

    public string AccountingTreatment { get; set; }

    public string TaxTreatment { get; set; }

    public string? Comments { get; set; }

    public string VersionName { get; set; }

    public string SourceSystem { get; set; }
}
