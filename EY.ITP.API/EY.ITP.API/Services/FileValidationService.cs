using EY.ITP.API.Interfaces.Providers;
using EY.ITP.API.Interfaces.Services;
using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Requests;
using EY.ITP.API.Models.Responses;

namespace EY.ITP.API.Services
{
    public class FileValidationService : IFileValidationService
    {
        private readonly IFileValidationProvider _provider;

        public FileValidationService(IFileValidationProvider provider)
            => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        public async Task<bool> BulkSaveFileValidation(IEnumerable<FileValidationSaveRequest> records)
        {
            //do mapping and validation here
            //Mapping of request to entity
            List<FileValidationSave> mappedData = records.Select(r => new FileValidationSave()
            {
                GAAPId = r.GAAPId,
                TaxYear = r.TaxYear,
                Period = r.Period,
                FileType = r.FileType,
                FileName = r.FileName,
                TemplateName = r.TemplateName,
                SourceSystem = r.SourceSystem,
                Modules = r.Modules,
                EntityVolume = r.EntityVolume,
                Status = r.Status,
                Errors = r.Errors,
                CreatedBy = r.CreatedBy,
                CreatedOn = r.CreatedOn,
                ModifiedBy = r.ModifiedBy,
                ModifiedOn = r.ModifiedOn
            }).ToList();
            return await _provider.BulkSaveFileValidation(mappedData);
        }

        public async Task<bool> UpdateFileValidation(IEnumerable<FileValidationUpdateRequest> records)
        {
            //do mapping and validation here
            List<FileValidationUpdate> mappedData = records.Select(r => new FileValidationUpdate()
            {
                FileValidationId = r.FileValidationId,
                GAAPId = r.GAAPId,
                TaxYear = r.TaxYear,
                Period = r.Period,
                FileType = r.FileType,
                FileName = r.FileName,
                TemplateName = r.TemplateName,
                SourceSystem = r.SourceSystem,
                Modules = r.Modules,
                EntityVolume = r.EntityVolume,
                Status = r.Status,
                Errors = r.Errors,
                CreatedBy = r.CreatedBy,
                CreatedOn = r.CreatedOn,
                ModifiedBy = r.ModifiedBy,
                ModifiedOn = r.ModifiedOn
            }).ToList();
            return await _provider.UpdateFileValidation(mappedData);
        }

        public async Task<IEnumerable<FileValidationListResponse>> GetFileValidationRecord (int fileValidationId)
        {
            //do mapping and validation here
            var records = await _provider.GetFileValidationRecord(fileValidationId);
            
            List<FileValidationListResponse> mappedData = records.Select(r => new FileValidationListResponse()
            {
                FileValidationId = r.FileValidationId,
                Filename = r.Filename,
                Uploadedby = r.Uploadedby,
                Uploadeddate = r.Uploadeddate,
                Status = r.Status,
                FileType = r.FileType,
                Modules = r.Modules,
                EntityVolume = r.EntityVolume,
            }).ToList();

            return mappedData;
        }
    }
}
