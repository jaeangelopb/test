using EY.ITP.API.Interfaces.Services;
using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EY.ITP.API.Controllers
{
    [Route("api/itp/fileValidation")]
    public class FileValidationController : Controller
    {
        private readonly IFileValidationService _service;

        public FileValidationController(IFileValidationService service)
            => _service = service ?? throw new ArgumentNullException(nameof(service));

        ///<summary>
        ///Perform saving of a file validation record
        ///</summary>
        ///<param name="records"></param>
        [HttpPost("save", Name = nameof(BulkSaveFileValidationData))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkSaveFileValidationData([FromBody] IEnumerable<FileValidationSaveRequest> records)
        {
            var response = await _service.BulkSaveFileValidation(records);
            return Ok(response);
        }

        ///<summary>
        ///Perform updating of a file validation record
        ///</summary>
        ///<param name="records"></param>
        [HttpPut("update", Name = nameof(UpdateFileValidation))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFileValidation([FromBody] IEnumerable<FileValidationUpdateRequest> records)
        {
            var response = await _service.UpdateFileValidation(records);
            return Ok(response);
        }

        ///<summary>
        ///Perform updating of a file validation record
        ///</summary>
        ///<param name="fileValidationId"></param>
        [HttpGet("get/{fileValidationId}", Name = nameof(GetFileValidationRecord))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFileValidationRecord([FromRoute] int fileValidationId)
        {
            var response = await _service.GetFileValidationRecord(fileValidationId);
            return Ok(response);
        }
    }
}
