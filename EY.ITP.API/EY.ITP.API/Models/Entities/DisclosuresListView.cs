using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Entities;

[Table("vw_Disclosures_List")]
public class DisclosuresListView
{
    public int DisclosuresId { get; set; }

    public string Jurisdiction { get; set; }

    public string Description { get; set; }

    public string Group { get; set; }
    
    public int Sorting { get; set; }
}
