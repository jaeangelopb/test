using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class TaxNoteCategory
{
    public int Tncid { get; set; }

    public string Code { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public int Sorting { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } 

    public DateTime ModifiedOn { get; set; }

    //public virtual ICollection<TaxAdjustmentCategory> TaxAdjustmentCategories { get; } = new List<TaxAdjustmentCategory>();
}
