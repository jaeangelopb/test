using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Entities;

[Table("vw_AccountType_List")]
public class AccountTypeListView
{
    public int AccountTypeId { get; set; }

    public string AccountType { get; set; }

    public int Sorting { get; set; }
}
