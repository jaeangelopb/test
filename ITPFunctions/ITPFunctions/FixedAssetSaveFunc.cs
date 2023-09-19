using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.Linq;

namespace ITPFunctions
{
    public static class FixedAssetSaveFunc
    {
        [FunctionName("FixedAssetSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "FA")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");    
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var far = JsonConvert.DeserializeObject<IEnumerable<FixedAsset>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                
                DataTable fixedAssetData = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "FARId";
                COLUMN.DataType = typeof(int);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TACId";
                COLUMN.DataType = typeof(int);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxSensitivityId";
                COLUMN.DataType = typeof(int);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AssetClass";
                COLUMN.DataType = typeof(string);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AssetDescription";
                COLUMN.DataType = typeof(string);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OpeningBalanceAdjustment";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "PriorYearAdjustments";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Additions";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Depreciation";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Disposals";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IntercompanyTransfers";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Other";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ClosingBalance";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Proceeds";
                COLUMN.DataType = typeof(double);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IsAccounting";
                COLUMN.DataType = typeof(bool);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IsTangible";
                COLUMN.DataType = typeof(bool);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                fixedAssetData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                fixedAssetData.Columns.Add(COLUMN);


                foreach(FixedAsset rowItem in far)
                {
                    DataRow DR = fixedAssetData.NewRow();
                    DR["FARId"] = rowItem.FARId_Accounting == null ? DBNull.Value : (object)rowItem.FARId_Accounting;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                    DR["TaxSensitivityId"] = rowItem.TaxSensitivityId == null ? DBNull.Value : (object)rowItem.TaxSensitivityId;
                    DR["Period"] = rowItem.Period;
                    DR["AssetClass"] = rowItem.AssetClass;
                    DR["AssetDescription"] = rowItem.AssetDescription;
                    DR["OpeningBalanceAdjustment"] = rowItem.OpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.OpeningBalanceAdjustment;
                    DR["PriorYearAdjustments"] = rowItem.PriorYearAdjustments == null ? DBNull.Value : (object)rowItem.PriorYearAdjustments;
                    DR["Additions"] = rowItem.Additions == null ? DBNull.Value : (object)rowItem.Additions;
                    DR["Depreciation"] = rowItem.Depreciation == null ? DBNull.Value : (object)rowItem.Depreciation;
                    DR["Disposals"] = rowItem.Disposals == null ? DBNull.Value : (object)rowItem.Disposals;
                    DR["IntercompanyTransfers"] = rowItem.IntercompanyTransfers == null ? DBNull.Value : (object)rowItem.IntercompanyTransfers;
                    DR["Other"] = rowItem.Other == null ? DBNull.Value : (object)rowItem.Other;
                    DR["ClosingBalance"] = rowItem.ClosingBalance == null ? DBNull.Value : (object)rowItem.ClosingBalance;
                    DR["Proceeds"] = rowItem.Proceeds == null ? DBNull.Value : (object)rowItem.Proceeds;
                    DR["IsAccounting"] = true;
                    DR["IsTangible"] = rowItem.IsTangible == null ? DBNull.Value : (object)rowItem.IsTangible;
                    DR["Comments"] = rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;
                    fixedAssetData.Rows.Add(DR);

                    DataRow DR1 = fixedAssetData.NewRow();
                    DR1["FARId"] = rowItem.FARId_Tax == null ? DBNull.Value : (object)rowItem.FARId_Tax;
                    DR1["EntityId"] = rowItem.EntityId;
                    DR1["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                    DR1["TaxSensitivityId"] = rowItem.TaxSensitivityId == null ? DBNull.Value : (object)rowItem.TaxSensitivityId;
                    DR1["Period"] = rowItem.Period;
                    DR1["AssetClass"] = rowItem.AssetClass;
                    DR1["AssetDescription"] = rowItem.AssetDescription;
                    DR1["OpeningBalanceAdjustment"] = rowItem.TaxOpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.TaxOpeningBalanceAdjustment;
                    DR1["PriorYearAdjustments"] = rowItem.TaxPriorYearAdjustments == null ? DBNull.Value : (object)rowItem.TaxPriorYearAdjustments;
                    DR1["Additions"] = rowItem.TaxAdditions == null ? DBNull.Value : (object)rowItem.TaxAdditions;
                    DR1["Depreciation"] = rowItem.TaxDepreciation == null ? DBNull.Value : (object)rowItem.TaxDepreciation;
                    DR1["Disposals"] = rowItem.TaxDisposals == null ? DBNull.Value : (object)rowItem.TaxDisposals;
                    DR1["IntercompanyTransfers"] = rowItem.TaxIntercompanyTransfers == null ? DBNull.Value : (object)rowItem.TaxIntercompanyTransfers;
                    DR1["Other"] = rowItem.TaxOther == null ? DBNull.Value : (object)rowItem.TaxOther;
                    DR1["ClosingBalance"] = rowItem.TaxClosingBalance == null ? DBNull.Value : (object)rowItem.TaxClosingBalance;
                    DR1["Proceeds"] = rowItem.TaxProceeds == null ? DBNull.Value : (object)rowItem.TaxProceeds;
                    DR1["IsAccounting"] = false;
                    DR1["IsTangible"] = rowItem.IsTangible == null ? DBNull.Value : (object)rowItem.IsTangible;
                    DR1["Comments"] = rowItem.Comments;
                    DR1["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR1["CreatedBy"] = UserID;
                    DR1["CreatedOn"] = DateTime.Now;
                    DR1["ModifiedBy"] = UserID;
                    DR1["ModifiedOn"] = DateTime.Now;
                    fixedAssetData.Rows.Add(DR1);
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_FAR_BulkSave";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = fixedAssetData;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_FAR_BulkSave]", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(Parameter);
                    cmd.Parameters.Add(Parameter1);
                    //Executing Procedure  
                    try
                    {
                        res = cmd.ExecuteNonQuery();
                        return new OkObjectResult(cmd.Parameters[1].Value);
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        return new BadRequestObjectResult(ex.Message);
                    }

                }


            }

                return new OkObjectResult(res);
        }
    }
}
