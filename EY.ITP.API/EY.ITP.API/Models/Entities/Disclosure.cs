using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class Disclosure
{
    public int DisclosuresId { get; set; }

    public int JurisdictionId { get; set; }

    public string Label { get; set; }

    public string Description { get; set; }

    public string Schedule { get; set; }

    public string Group { get; set; }

    public int Sorting { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual ICollection<Account> Accounts { get; } = new List<Account>();

    //public virtual Jurisdiction Jurisdiction { get; set; } = null!;
}
