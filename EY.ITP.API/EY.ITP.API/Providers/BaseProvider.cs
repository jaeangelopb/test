using EY.ITP.API.Models.Entities;
using EY.ITP.API.Models.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using EY.ITP.API.Extensions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EY.ITP.API.Providers
{
    public abstract class BaseProvider<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;

        public BaseProvider(DbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _configuration = configuration;
        }

        public BaseProvider(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected async Task<bool> ExecuteNonQuery(string schema, string spName, string strParam, IEnumerable<SqlParameter> sqlParams)
        {
            var updateSQLParams = AddReturnParameter(sqlParams.ToList());
            await _dbContext.Database.ExecuteSqlRawAsync($"exec @returnval = {schema}.{spName} {strParam}", updateSQLParams);
            return updateSQLParams.FirstOrDefault(p => p.ParameterName.ToLower().Equals("@returnval")).Value.Equals(0);
        }

        #region Query Builders
        private async Task<IEnumerable<TableTypeColumn>> GetTableTypeColumns(string schema, string customTableType)
        {
            var paramSchema = new SqlParameter("schema", schema);
            var paramCustomTableType = new SqlParameter("customTableType", customTableType);
            var sql = $"SELECT C.NAME AS [Column_Name], T.NAME AS [Column_Type], C.IS_NULLABLE AS [Nullable] " +
                    $"FROM SYS.COLUMNS C " +
                    $"INNER JOIN SYS.TYPES T " +
                    $"ON T.USER_TYPE_ID = C.USER_TYPE_ID " +
                    $"WHERE C.OBJECT_ID = (SELECT T.TYPE_TABLE_OBJECT_ID FROM SYS.TABLE_TYPES T " +
                    $"INNER JOIN SYS.SCHEMAS S ON T.schema_id = S.schema_id " +
                    $"WHERE S.name = @schema and T.name = @customTableType) " +
                    $"ORDER BY C.COLUMN_ID";

            return await this._dbContext.Set<TableTypeColumn>().FromSqlRaw(sql, paramSchema, paramCustomTableType).ToListAsync();
        }

        private static List<SqlParameter> AddReturnParameter(List<SqlParameter> sqlParams)
        {
            if (sqlParams.FindIndex(p => p.ParameterName.ToLower().Equals("@returnval")) < 0)   //check if returnval is not yet added 
            {
                sqlParams.Add(new SqlParameter("@returnval", SqlDbType.Int) { Direction = ParameterDirection.Output });
            }

            return sqlParams;
        }

        protected async Task<SqlParameter> BuildTableParameter(StoredProcParameter param, IEnumerable data)
        {
            var table = new DataTable();
            List<string> columnNames = new List<string>();
            //Get the Custom Table Columns
            IEnumerable<TableTypeColumn> customTableTypeColumns = await this.GetTableTypeColumns(param.Schema, param.Type_Name);

            //Create the Data Table
            foreach (TableTypeColumn customTableTypeColumn in customTableTypeColumns)
            {
                table.Columns.Add(customTableTypeColumn.Column_Name, customTableTypeColumn.ToType());
                columnNames.Add(customTableTypeColumn.Column_Name); //Create a collection of columns to populate
            }

            //Create a dictionary of column names
            Dictionary<string, PropertyInfo> propList = null;
            Dictionary<string, object> propDefaultValue = null;

            //Enumerate thru the collection and add it to the table
            foreach (var entry in data)
            {
                var newrow = table.NewRow();

                if (propList == null)
                {
                    propList = new Dictionary<string, PropertyInfo>();
                    propDefaultValue = new Dictionary<string, object>();
                    //Initiliaze property list
                    var propInfos = entry.GetType().GetProperties();
                    foreach (var columnName in columnNames)
                    {
                        var propInfo = propInfos.FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                        if (propInfo == null)
                        {
                            foreach (var prop in propInfos)
                            {
                                var customAttr = prop.CustomAttributes.FirstOrDefault(i => i.AttributeType.Equals(typeof(ColumnAttribute)) && i.ConstructorArguments[0].Value.ToString().Equals(columnName, StringComparison.OrdinalIgnoreCase));
                                if (customAttr != null)
                                {
                                    propInfo = prop;
                                    break;
                                }
                            }
                        }
                        if (propInfo == null)
                        {
                            throw new NotImplementedException($"Column {columnName} was not found on object {entry.GetType().FullName}");
                        }
                        //Add to Property List
                        propList.Add(columnName, propInfo);
                        //Find If there is a default value
                        var defaultAttr = propInfo.CustomAttributes.FirstOrDefault(i => i.AttributeType.Equals(typeof(DefaultValueAttribute)));
                        if (defaultAttr != null)
                        {
                            propDefaultValue.Add(columnName, defaultAttr.ConstructorArguments[0].Value);
                        }
                        else
                        {
                            propDefaultValue.Add(columnName, propInfo.ToDefaultValue(entry));
                        }
                    }
                }

                foreach (var columnName in columnNames)
                {
                    var value = propList[columnName].GetValue(entry);
                    newrow[columnName] = value ?? propDefaultValue[columnName];
                }
                table.Rows.Add(newrow);
            }

            return await Task.FromResult(new SqlParameter
            {
                ParameterName = param.Parameter_Name,
                Value = table,
                TypeName = $"{param.Schema}.{param.Type_Name}",
                SqlDbType = SqlDbType.Structured,
                Direction = param.ToParameterDirection()
            });
        }
        #endregion
    }
}
