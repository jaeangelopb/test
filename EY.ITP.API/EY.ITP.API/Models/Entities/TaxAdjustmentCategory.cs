using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class TaxAdjustmentCategory
{
    public int Tacid { get; set; }

    public int Tncid { get; set; }

    public string Description { get; set; }

    public string Category { get; set; }

    public string Source { get; set; }

    public bool Default { get; set; }

    public string Sorting { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual ICollection<Account> Accounts { get; } = new List<Account>();

    //public virtual ICollection<OtherPermanentAdjustment> OtherPermanentAdjustments { get; } = new List<OtherPermanentAdjustment>();

    //public virtual ICollection<OtherTemporaryAdjustment> OtherTemporaryAdjustments { get; } = new List<OtherTemporaryAdjustment>();

    //public virtual TaxNoteCategory Tnc { get; set; } = null!;
}
