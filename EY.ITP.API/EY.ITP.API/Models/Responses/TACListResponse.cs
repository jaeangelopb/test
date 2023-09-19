using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_TAC_List")]
public class TACListResponse
{
    public int Tacid { get; set; }

    public string TaxNoteCategory { get; set; }

    public string Description { get; set; }

    public string Category { get; set; }

    public string Source { get; set; }

    public bool Default { get; set; }

    public string Sorting { get; set; }
}
