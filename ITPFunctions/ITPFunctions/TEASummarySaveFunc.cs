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
    public static class TEASummarySaveFunc
    {
        [FunctionName("TEASummarySaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "TEASummarySave")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var fra = JsonConvert.DeserializeObject<IEnumerable<TEASummary>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable TEASummary = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "TEAId";
                COLUMN.DataType = typeof(int);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CurrentTaxExpense";
                COLUMN.DataType = typeof(double);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DeferredTaxExpense";
                COLUMN.DataType = typeof(double);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DeferredTaxAsset";
                COLUMN.DataType = typeof(double);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DeferredTaxLiability";
                COLUMN.DataType = typeof(double);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxPayable";
                COLUMN.DataType = typeof(double);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Equity";
                COLUMN.DataType = typeof(double);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Intercompany";
                COLUMN.DataType = typeof(double);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Other";
                COLUMN.DataType = typeof(double);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                TEASummary.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                TEASummary.Columns.Add(COLUMN);


                foreach (TEASummary rowItem in fra)
                {
                    
                        DataRow DR = TEASummary.NewRow();
                        DR["TEAId"] = rowItem.TEAId == null ? DBNull.Value : (object)rowItem.TEAId;
                        DR["EntityId"] = rowItem.EntityId;
                        DR["Period"] = rowItem.Period;
                        DR["Description"] = rowItem.Description == null ? DBNull.Value : (object)rowItem.Description;
                        DR["CurrentTaxExpense"] = rowItem.CurrentTaxExpense == null ? DBNull.Value : (object)rowItem.CurrentTaxExpense;
                        DR["DeferredTaxExpense"] = rowItem.DeferredTaxExpense == null ? DBNull.Value : (object)rowItem.DeferredTaxExpense;
                        DR["DeferredTaxAsset"] = rowItem.DeferredTaxAsset == null ? DBNull.Value : (object)rowItem.DeferredTaxAsset;
                        DR["DeferredTaxLiability"] = rowItem.DeferredTaxLiability == null ? DBNull.Value : (object)rowItem.DeferredTaxLiability;
                        DR["TaxPayable"] = rowItem.TaxPayable == null ? DBNull.Value : (object)rowItem.TaxPayable;
                        DR["Equity"] = rowItem.Equity == null ? DBNull.Value : (object)rowItem.Equity;
                        DR["Intercompany"] = rowItem.Intercompany == null ? DBNull.Value : (object)rowItem.Intercompany;
                        DR["Other"] = rowItem.Other == null ? DBNull.Value : (object)rowItem.Other;
                        DR["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                        DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                        DR["CreatedBy"] = UserID;
                        DR["CreatedOn"] = DateTime.Now;
                        DR["ModifiedBy"] = UserID;
                        DR["ModifiedOn"] = DateTime.Now;
                        TEASummary.Rows.Add(DR);

                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_TEASummary";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = TEASummary;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_TEASummary_BulkSave]", conn))
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
