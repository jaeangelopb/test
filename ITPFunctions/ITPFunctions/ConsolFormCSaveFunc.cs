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
    public static class ConsolFormCSaveFunc
    {
        [FunctionName("ConsolFormCSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ConsolFormCSave")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var fra = JsonConvert.DeserializeObject<IEnumerable<ConsolFormCDisclosure>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable ConsolFormC = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "FCAdjId";
                COLUMN.DataType = typeof(int);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "GroupingName";
                COLUMN.DataType = typeof(string);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ParentEntityId";
                COLUMN.DataType = typeof(int);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DisclosuresId";
                COLUMN.DataType = typeof(int);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Adjustment";
                COLUMN.DataType = typeof(double);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                ConsolFormC.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                ConsolFormC.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                ConsolFormC.Columns.Add(COLUMN);


                foreach (ConsolFormCDisclosure rowItem in fra)
                {

                    DataRow DR = ConsolFormC.NewRow();
                    DR["FCAdjId"] = rowItem.FCAdjId == null ? DBNull.Value : (object)rowItem.FCAdjId;
                    DR["GroupingName"] = rowItem.GroupingName;
                    DR["ParentEntityId"] = rowItem.ParentEntityId;
                    DR["Period"] = rowItem.Period;
                    DR["DisclosuresId"] = rowItem.DisclosuresId;
                    DR["Adjustment"] = rowItem.Adjustment == null ? DBNull.Value : (object)rowItem.Adjustment;
                    DR["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;
                    ConsolFormC.Rows.Add(DR);

                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_FormCAdjustments";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = ConsolFormC;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_FormCAdjustments_BulkSave]", conn))
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