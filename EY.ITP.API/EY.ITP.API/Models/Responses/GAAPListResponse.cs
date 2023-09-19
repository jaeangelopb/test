using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_GAAP_List")]
public class GAAPListResponse
{
    public int Gaapid { get; set; }

    public string Gaap { get; set; }
}
