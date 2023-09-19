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
    public static class FITOAdjustmentsSaveFunc
    {
        [FunctionName("FITOAdjustmentsSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "FITOAdjustmentsSave")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var fra = JsonConvert.DeserializeObject<IEnumerable<FITOAdjustments>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable ConduitForeignIncome = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "FITOId";
                COLUMN.DataType = typeof(int);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "YearIncurred";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "YearOfExpiry";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OpeningBalanceRollForward";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OpeningBalanceAdjustment";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CurrentYearTaxCredits";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxCreditsUtilised";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxCreditsExpired";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OtherAdjustment";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                ConduitForeignIncome.Columns.Add(COLUMN);


                foreach (FITOAdjustments rowItem in fra)
                {
                    DataRow DR = ConduitForeignIncome.NewRow();
                    DR["FITOId"] = rowItem.FITOId == null ? DBNull.Value : (object)rowItem.FITOId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["Period"] = rowItem.Period;
                    DR["YearIncurred"] = rowItem.YearIncurred;
                    DR["YearOfExpiry"] = rowItem.YearOfExpiry;
                    DR["Description"] = rowItem.Description;
                    DR["OpeningBalanceRollForward"] = rowItem.OpeningBalanceRollForward == null ? DBNull.Value : (object)rowItem.OpeningBalanceRollForward;
                    DR["OpeningBalanceAdjustment"] = rowItem.OpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.OpeningBalanceAdjustment;
                    DR["CurrentYearTaxCredits"] = rowItem.CurrentYearTaxCredits == null ? DBNull.Value : (object)rowItem.CurrentYearTaxCredits;
                    DR["TaxCreditsUtilised"] = rowItem.TaxCreditsUtilised == null ? DBNull.Value : (object)rowItem.TaxCreditsUtilised;
                    DR["TaxCreditsExpired"] = rowItem.TaxCreditsExpired == null ? DBNull.Value : (object)rowItem.TaxCreditsExpired;
                    DR["OtherAdjustment"] = rowItem.OtherAdjustment == null ? DBNull.Value : (object)rowItem.OtherAdjustment;
                    DR["Comments"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;
                    ConduitForeignIncome.Rows.Add(DR);


                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_FITOAdjustments";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = ConduitForeignIncome;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_FITOAdjustments_BulkSave]", conn))
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
