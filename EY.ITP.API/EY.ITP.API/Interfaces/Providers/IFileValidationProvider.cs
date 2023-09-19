using EY.ITP.API.Models.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EY.ITP.API.Interfaces.Providers
{
    public interface IFileValidationProvider
    {
        Task<bool> BulkSaveFileValidation(IEnumerable<FileValidationSave> records);
        Task<bool> UpdateFileValidation(IEnumerable<FileValidationUpdate> records);
        Task<IEnumerable<FileValidationListView>> GetFileValidationRecord(int fileValidationId);
    }
}
