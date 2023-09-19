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
    public static class ETRAdjustmentsSaveFunc
    {
        [FunctionName("ETRAdjustments")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ETR")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();

            dynamic body = await req.Content.ReadAsStringAsync();
            var etrAdjustments = JsonConvert.DeserializeObject<IEnumerable<ETRAdjustments>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable etrAssetsAdjustment = new DataTable();

                //Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "ETRId";
                COLUMN.DataType = typeof(int);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Category";
                COLUMN.DataType = typeof(string);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Amount";
                COLUMN.DataType = typeof(double);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                etrAssetsAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                etrAssetsAdjustment.Columns.Add(COLUMN);

                // INSERTING DATA    
                foreach (ETRAdjustments rowItem in etrAdjustments)
                {
                    DataRow DR = etrAssetsAdjustment.NewRow();
                    DR["ETRId"] = rowItem.ETRId == null ? DBNull.Value : (object)rowItem.ETRId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["Period"] = rowItem.Period;
                    DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR["Description"] = rowItem.Description;
                    DR["Category"] = rowItem.Category == null ? DBNull.Value : (object)rowItem.Category;
                    DR["Amount"] = rowItem.Amount == null ? DBNull.Value : (object)rowItem.Amount;
                    DR["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;

                    etrAssetsAdjustment.Rows.Add(DR);

                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_ETRSummarySave";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = etrAssetsAdjustment;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_ETRSummary_BulkSave]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(Parameter);
                    cmd.Parameters.Add(Parameter1);
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

            return new OkObjectResult("success");
        }
    }
}