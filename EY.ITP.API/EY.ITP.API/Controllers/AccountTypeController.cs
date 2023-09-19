using EY.ITP.API.Interfaces.Services;
using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EY.ITP.API.Controllers
{
    [Route("api/itp/common/accounttype")]
    public class AccountTypeController : Controller
    {
        private readonly IAccountTypeService _service;

        public AccountTypeController(IAccountTypeService service)
            => _service = service ?? throw new ArgumentNullException(nameof(service));

        ///<summary>
        ///Perform updating of a file validation record
        ///</summary>
        [HttpGet("get-all/", Name = nameof(GetAccountTypeRecords))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAccountTypeRecords()
        {
            var response = await _service.GetAccountTypeList();
            return Ok(response);
        }
    }
}
