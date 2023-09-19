using EY.ITP.API.Interfaces.Services;
using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EY.ITP.API.Controllers
{
    [Route("api/itp/common/disclosures")]
    public class DisclosuresController : Controller
    {
        private readonly IDisclosuresService _service;

        public DisclosuresController(IDisclosuresService service)
            => _service = service ?? throw new ArgumentNullException(nameof(service));

        ///<summary>
        ///Perform updating of a file validation record
        ///</summary>
        [HttpGet("get-all/", Name = nameof(GetDisclosuresLists))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDisclosuresLists()
        {
            var response = await _service.GetDisclosuresLists();
            return Ok(response);
        }
    }
}
