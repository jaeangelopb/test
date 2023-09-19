using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Requests;
using EY.ITP.API.Models.Responses;

namespace EY.ITP.API.Interfaces.Services
{
    public interface IFileValidationService
    {
        Task<bool> BulkSaveFileValidation(IEnumerable<FileValidationSaveRequest> records);
        Task<bool> UpdateFileValidation(IEnumerable<FileValidationUpdateRequest> records);
        Task<IEnumerable<FileValidationListResponse>> GetFileValidationRecord(int fileValidationId);
    }
}
