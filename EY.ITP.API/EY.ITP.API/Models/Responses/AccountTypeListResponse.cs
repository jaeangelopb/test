using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Responses;


public class AccountTypeListResponse
{
    public int AccountTypeId { get; set; }

    public string AccountType { get; set; }
    public int Sorting { get; set; }
}
