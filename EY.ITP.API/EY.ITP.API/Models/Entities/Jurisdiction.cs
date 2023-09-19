using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Entities;

public class Jurisdiction
{
    public int JurisdictionId { get; set; }

    [Column("Jurisdiction")]
    public string JurisdictionName { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual ICollection<Disclosure> Disclosures { get; } = new List<Disclosure>();

    //public virtual ICollection<Entity> Entities { get; } = new List<Entity>();
}
