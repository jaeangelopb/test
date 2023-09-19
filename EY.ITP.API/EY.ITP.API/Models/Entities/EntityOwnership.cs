using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class EntityOwnership
{
    public int EntityOwnershipId { get; set; }

    public string GroupingName { get; set; }

    public int ParentEntityId { get; set; }

    public int SubEntityId { get; set; }

    public decimal? OwnershipPercentage { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual Entity ParentEntity { get; set; }

    //public virtual Entity SubEntity { get; set; }
}
