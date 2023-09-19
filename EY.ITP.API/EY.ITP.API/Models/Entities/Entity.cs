using System;
using System.Collections.Generic;

namespace EY.ITP.API.Models.Entities;

public class Entity
{
    public int EntityId { get; set; }

    public int JurisdictionId { get; set; }

    public string EntityCode { get; set; }

    public string EntityName { get; set; }

    public string EntityType { get; set; }

    public string Tfn { get; set; }

    public string Abn { get; set; }

    public bool Active { get; set; }

    public DateTime SetUpDate { get; set; }

    public DateTime WoundUpDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    //public virtual ICollection<EntityCoamapping> EntityCoamappings { get; } = new List<EntityCoamapping>();

    //public virtual ICollection<EntityOwnership> EntityOwnershipParentEntities { get; } = new List<EntityOwnership>();

    //public virtual ICollection<EntityOwnership> EntityOwnershipSubEntities { get; } = new List<EntityOwnership>();

    //public virtual ICollection<FixedAssetRegister> FixedAssetRegisters { get; } = new List<FixedAssetRegister>();

    //public virtual Jurisdiction Jurisdiction { get; set; } = null!;

    //public virtual ICollection<OtherPermanentAdjustment> OtherPermanentAdjustments { get; } = new List<OtherPermanentAdjustment>();

    //public virtual ICollection<OtherTemporaryAdjustment> OtherTemporaryAdjustments { get; } = new List<OtherTemporaryAdjustment>();

    //public virtual ICollection<TrialBalance> TrialBalances { get; } = new List<TrialBalance>();
}
