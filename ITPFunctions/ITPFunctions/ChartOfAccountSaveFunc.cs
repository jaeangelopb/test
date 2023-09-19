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
    public static class ChartOfAccountSaveFunc
    {
        [FunctionName("ChartOfAccountSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ChartOfAccountSave")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            var Version = req.Headers.GetValues("TBMappingName").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var coa = JsonConvert.DeserializeObject<IEnumerable<ChartOfAccount>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable ChartOfAccount = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "AccountId";
                COLUMN.DataType = typeof(int);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AccountTypeId";
                COLUMN.DataType = typeof(int);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxSensitivityId";
                COLUMN.DataType = typeof(int);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DisclosuresId";
                COLUMN.DataType = typeof(int);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TACId";
                COLUMN.DataType = typeof(int);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AccountCode";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AccountName";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Risk";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AccountingTreatment";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxTreatment";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "VersionName";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "SourceSystem";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                ChartOfAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                ChartOfAccount.Columns.Add(COLUMN);


                foreach (ChartOfAccount rowItem in coa)
                {
                    
                        DataRow DR = ChartOfAccount.NewRow();
                        DR["AccountId"] = rowItem.AccountId == null ? DBNull.Value : (object)rowItem.AccountId;
                        DR["AccountTypeId"] = rowItem.AccountTypeId == null ? DBNull.Value : (object)rowItem.AccountTypeId;
                        DR["TaxSensitivityId"] = rowItem.TaxSensitivityId == null ? DBNull.Value : (object)rowItem.TaxSensitivityId;
                        DR["DisclosuresId"] = rowItem.DisclosuresId == null ? DBNull.Value : (object)rowItem.DisclosuresId;
                        DR["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                        DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                        DR["AccountCode"] = rowItem.AccountCode;
                        DR["AccountName"] = rowItem.AccountName;
                        DR["Risk"] = rowItem.Risk == null ? DBNull.Value : (object)rowItem.Risk;
                        DR["AccountingTreatment"] = rowItem.AccountingTreatment == null ? DBNull.Value : (object)rowItem.AccountingTreatment;
                        DR["TaxTreatment"] = rowItem.TaxTreatment == null ? DBNull.Value : (object)rowItem.TaxTreatment;
                        DR["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                        DR["VersionName"] = Version;
                        DR["SourceSystem"] = rowItem.SourceSystem == null ? DBNull.Value : (object)rowItem.SourceSystem;
                        DR["CreatedBy"] = UserID;
                        DR["CreatedOn"] = DateTime.Now;
                        DR["ModifiedBy"] = UserID;
                        DR["ModifiedOn"] = DateTime.Now;
                        ChartOfAccount.Rows.Add(DR);

                       
                 
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_ChartOfAccount";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = ChartOfAccount;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Core].[sp_ChartOfAccount_BulkSave]", conn))
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
        }
    }
}
