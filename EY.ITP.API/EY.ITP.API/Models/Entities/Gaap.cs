using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Entities;

public class Gaap
{
    public int Gaapid { get; set; }

    [Column("GAAP")]
    public string GaapName { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual ICollection<Account> Accounts { get; } = new List<Account>();
}
