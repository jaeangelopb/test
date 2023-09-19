using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;

[Table("vw_Entity_List")]
public class EntityListResponse
{
    public int EntityId { get; set; }

    public string Jurisdiction { get; set; }

    public string EntityCode { get; set; }

    public string EntityName { get; set; }

    public string EntityType { get; set; }

    public string Tfn { get; set; }

    public string Abn { get; set; }

    public bool Active { get; set; }

    public DateTime SetUpDate { get; set; }

    public DateTime WoundUpDate { get; set; }
}
