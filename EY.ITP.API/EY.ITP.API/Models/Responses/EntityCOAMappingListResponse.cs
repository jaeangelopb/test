using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_EntityCOAMapping_List")]
public class EntityCOAMappingListResponse
{
    public int EntityCoamappingId { get; set; }

    public string EntityName { get; set; }

    public string Coaname { get; set; }
}
