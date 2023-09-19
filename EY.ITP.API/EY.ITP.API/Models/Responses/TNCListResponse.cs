using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_TNC_List")]
public class TNCListResponse
{
    public int Tncid { get; set; }

    public string Code { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public int Sorting { get; set; }
}
