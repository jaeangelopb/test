using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_TrialBalance_List")]
public class TrialBalanceListResponse
{
    public int TrialBalanceId { get; set; }

    public string EntityName { get; set; }

    public string AccountName { get; set; }

    public string TaxYear { get; set; }

    public DateTime Period { get; set; }

    public decimal? Amount { get; set; }

    public string SourceSystem { get; set; }
}
