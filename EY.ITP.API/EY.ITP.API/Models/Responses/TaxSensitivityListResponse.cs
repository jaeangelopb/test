using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_TaxSensitivity_List")]
public class TaxSensitivityListResponse
{
    public int TaxSensitivityId { get; set; }

    public string TaxSensitivity { get; set; }
}
