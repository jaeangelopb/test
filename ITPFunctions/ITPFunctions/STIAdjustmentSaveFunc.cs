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

namespace ITPFunctions
{
    public static class STIAdjustmentSaveFunc
    {
        [FunctionName("STIAdjustmentSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "STIAdjSave")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var sti = JsonConvert.DeserializeObject<IEnumerable<STIAdjustment>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            int res;
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                
                DataTable stiAdjData = new DataTable();


                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "STIAdjustmentId";
                COLUMN.DataType = typeof(int);
                stiAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OPAId";
                COLUMN.DataType = typeof(int);
                stiAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AccountId";
                COLUMN.DataType = typeof(int);
                stiAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TACId";
                COLUMN.DataType = typeof(int);
                stiAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                stiAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DisclosuresId";
                COLUMN.DataType = typeof(int);
                stiAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);

                stiAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);

                stiAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);

                stiAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Amount";
                COLUMN.DataType = typeof(double);

                stiAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                stiAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                stiAdjData.Columns.Add(COLUMN);



                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);

                stiAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);

                stiAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);

                stiAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);

                stiAdjData.Columns.Add(COLUMN);

                foreach (STIAdjustment rowItem in sti)
                {
                    DataRow DR = stiAdjData.NewRow();
                    DR["STIAdjustmentId"] = rowItem.STIAdjustmentId == null ? DBNull.Value : (object)rowItem.STIAdjustmentId;
                    DR["OPAId"] = rowItem.OPAId == null ? DBNull.Value : (object)rowItem.OPAId;
                    DR["AccountId"] = rowItem.AccountId == null || rowItem.AccountId == 0 ? DBNull.Value : (object)rowItem.AccountId;
                    DR["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                    DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR["DisclosuresId"] = rowItem.DisclosuresId == null ? DBNull.Value : (object)rowItem.DisclosuresId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["Period"] = rowItem.Period;
                    DR["Description"] = rowItem.Description;
                    DR["Amount"] = rowItem.Amount==null ? DBNull.Value:(object)rowItem.Amount;
                    DR["Comments"] = rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;



                    stiAdjData.Rows.Add(DR);
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_STIAdjustmentsSave";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = stiAdjData;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_STIAdjustments_BulkSave]", conn))
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
                    catch(System.Data.SqlClient.SqlException ex)
                    {
                        return new BadRequestObjectResult(ex.Message);
                    }
                  
                }
            }

                return new OkObjectResult(res);
        }
    }
}
