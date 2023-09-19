using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Entities;

public class TaxSensitivity
{
    public int TaxSensitivityId { get; set; }

    [Column("TaxSensitivity")]
    public string TaxSensitivityName { get; set; }

    public int Sorting { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual ICollection<Account> Accounts { get; } = new List<Account>();
}
