using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_Jurisdiction_List")]
public class JurisdictionListResponse
{
    public int JurisdictionId { get; set; }

    public string Jurisdiction { get; set; }
}
