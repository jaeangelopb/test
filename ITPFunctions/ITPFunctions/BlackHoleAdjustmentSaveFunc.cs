using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Linq;
using System.Reflection.Metadata;

namespace ITPFunctions
{
    public static class BlackHoleAdjustmentSaveFunc
    {
        [FunctionName("BlackHoleAdjustmentSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var bhAdj = JsonConvert.DeserializeObject<IEnumerable<BlackHoleAdjustment>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable blackHoleAdj = new DataTable();
                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN = new DataColumn();
                COLUMN.ColumnName = "BHAdjustmentId";
                COLUMN.DataType = typeof(int);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TACId";
                COLUMN.DataType = typeof(int);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Project";
                COLUMN.DataType = typeof(string);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "YearIncurred";
                COLUMN.DataType = typeof(string);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "InitialCost";
                COLUMN.DataType = typeof(double);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "s40-880";
                COLUMN.DataType = typeof(double);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Deductible";
                COLUMN.DataType = typeof(double);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "NonDeductible";
                COLUMN.DataType = typeof(double);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "40-880_InitialCosts";
                COLUMN.DataType = typeof(double);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OpeningBalanceAdjustment";
                COLUMN.DataType = typeof(double);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                blackHoleAdj.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                blackHoleAdj.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                blackHoleAdj.Columns.Add(COLUMN);


                foreach(BlackHoleAdjustment rowItem in bhAdj)
                {
                    DataRow DR = blackHoleAdj.NewRow();
                    DR["BHAdjustmentId"] = rowItem.BHAdjustmentId == null ? DBNull.Value : (object)rowItem.BHAdjustmentId;
                    DR["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                    DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["Project"] = rowItem.Project;
                    DR["Description"] = rowItem.Description;
                    DR["Period"] = rowItem.Period;
                    DR["YearIncurred"] = rowItem.YearIncurred;
                    DR["InitialCost"] = rowItem.InitialCost == null ? DBNull.Value : (object)rowItem.InitialCost;
                    DR["s40-880"] = rowItem.S40880 == null ? DBNull.Value : (object)rowItem.S40880;
                    DR["Deductible"] = rowItem.Deductible == null ? DBNull.Value : (object)rowItem.Deductible;
                    DR["NonDeductible"] = rowItem.NonDeductible == null ? DBNull.Value : (object)rowItem.NonDeductible;
                    DR["40-880_InitialCosts"] = rowItem.InitialCosts_40880 == null ? DBNull.Value : (object)rowItem.InitialCosts_40880;
                    DR["OpeningBalanceAdjustment"] = rowItem.OpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.OpeningBalanceAdjustment;
                    DR["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                    DR["Process"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;

                    blackHoleAdj.Rows.Add(DR);
                }

                //Parameter declaration
                SqlParameter TableParameter = new SqlParameter();
                TableParameter.ParameterName = "@tbl_BlackHoleAdjustments";
                TableParameter.SqlDbType = SqlDbType.Structured;
                TableParameter.Direction = ParameterDirection.Input;
                TableParameter.Value = blackHoleAdj;

                SqlParameter ReturnStatus = new SqlParameter();
                ReturnStatus.ParameterName = "@ReturnStatus";
                ReturnStatus.Direction = ParameterDirection.ReturnValue;

                using(SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_BlackHoleAdjustments_BulkSave]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(TableParameter);
                    cmd.Parameters.Add(ReturnStatus);
                    //Executing Procedure  

                    try
                    {
                        int res = cmd.ExecuteNonQuery();
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
