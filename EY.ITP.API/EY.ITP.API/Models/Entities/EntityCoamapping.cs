using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class EntityCoamapping
{
    public int EntityCoamappingId { get; set; }

    public int EntityId { get; set; }

    public string Coaname { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual Account CoanameNavigation { get; set; }

    //public virtual Entity Entity { get; set; }
}
