using EY.ITP.API.Constant;
using EY.ITP.API.DBContext;
using EY.ITP.API.Interfaces.Providers;
using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace EY.ITP.API.Providers
{
    public class FileValidationProvider : BaseProvider<FileValidationSave>, IFileValidationProvider
    {
        public FileValidationProvider(CoreDBContext dBContext) : base(dBContext) { }
        public FileValidationProvider(CoreDBContext dBContext, IConfiguration configuration) : base(dBContext, configuration) { }

        public async Task<bool> BulkSaveFileValidation(IEnumerable<FileValidationSave> records)
        {
            var sqlParams = new List<SqlParameter>
            {
                await BuildTableParameter(new StoredProcParameter()
                {
                    Parameter_Name = Constants.FileValidation_SP_BulkSave_TableType_ParamName,
                    Schema = Constants.Schema_Core,
                    Type_Name = Constants.FileValidation_SP_BulkSave_TableType_ParamType,
                    Parameter_Mode = Constants.SP_Param_Mode_In,
                },
                records)
            };

            var strParam = Constants.FileValidation_SP_BulkSave_TableType_ParamName;
            return await ExecuteNonQuery(Constants.Schema_Core, Constants.FileValidation_SP_BulkSave_SPName, strParam, sqlParams);
        }

        public async Task<bool> UpdateFileValidation(IEnumerable<FileValidationUpdate> records)
        {
            var sqlParams = new List<SqlParameter>
            {
                await BuildTableParameter(new StoredProcParameter()
                {
                    Parameter_Name = Constants.FileValidation_SP_BulkUpdate_TableType_ParamName,
                    Schema = Constants.Schema_Core,
                    Type_Name = Constants.FileValidation_SP_BulkUpdate_TableType_ParamType,
                    Parameter_Mode = Constants.SP_Param_Mode_In,
                },
                records)
            };

            var strParam = Constants.FileValidation_SP_BulkUpdate_TableType_ParamName;
            return await ExecuteNonQuery(Constants.Schema_Core, Constants.FileValidation_SP_BulkUpdate_SPName, strParam, sqlParams);
        }

        public async Task<IEnumerable<FileValidationListView>> GetFileValidationRecord(int fileValidationId)
        {
            var query = _dbContext.Set<FileValidationListView>().OfType<FileValidationListView>();
            return await query.Where(q => q.FileValidationId == fileValidationId).ToListAsync();
        }
    }
}
