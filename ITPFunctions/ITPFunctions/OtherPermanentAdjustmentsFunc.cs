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
    public static class OtherPermanentAdjustmentsFunc
    {
        [FunctionName("OtherPermanentAdjustments")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "OPA")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var opa = JsonConvert.DeserializeObject<IEnumerable<OtherPermanentAdjustment>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable otherPermanentAdjustment = new DataTable();

                // Adding Columns
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "OPAId";
                COLUMN.DataType = typeof(int);
                otherPermanentAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                otherPermanentAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                otherPermanentAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                otherPermanentAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DisclosuresId";
                COLUMN.DataType = typeof(int);
                otherPermanentAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxYear";
                COLUMN.DataType = typeof(string);
                otherPermanentAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                otherPermanentAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IsAccounting";
                COLUMN.DataType = typeof(bool);

                otherPermanentAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Amount";
                COLUMN.DataType = typeof(double);

                otherPermanentAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                otherPermanentAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                otherPermanentAdjustment.Columns.Add(COLUMN);



                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);

                otherPermanentAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);

                otherPermanentAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);

                otherPermanentAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);

                otherPermanentAdjustment.Columns.Add(COLUMN);


                foreach (OtherPermanentAdjustment rowItem in opa)
                {
                    DataRow DR = otherPermanentAdjustment.NewRow();
                    DR["OPAId"] = rowItem.OPAId == null ? DBNull.Value : (object) rowItem.OPAId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["Description"] = rowItem.Description == "" ? throw new ArgumentNullException("Empty description is not allowed!") : (object)rowItem.Description;
                    DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object) rowItem.TNCId;
                    DR["DisclosuresId"] = rowItem.DisclosuresId;
                    DR["TaxYear"] = rowItem.Period.Substring(3);
                    DR["Period"] = rowItem.Period;                 
                    DR["IsAccounting"] = rowItem.IsAccounting;
                    DR["Amount"] = rowItem.Amount == null ? DBNull.Value : (object)rowItem.Amount;
                    DR["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;

                    otherPermanentAdjustment.Rows.Add(DR);
                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_OPA_BulkSave";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = otherPermanentAdjustment;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Core].[sp_OPA_BulkSave]", conn))
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
        }
    }
}
