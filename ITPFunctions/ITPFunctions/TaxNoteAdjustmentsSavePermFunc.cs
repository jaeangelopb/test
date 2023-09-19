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
    public static class TaxNoteAdjustmentsSavePermFunc
    {
        [FunctionName("TaxNoteAdjustmentsSavePermFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "TaxNoteAdjustmentsSavePermanent")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var fra = JsonConvert.DeserializeObject<IEnumerable<TaxNoteAdjustmentsPerm>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable TaxNoteAdjustment = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNAdjId";
                COLUMN.DataType = typeof(int);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "GroupingName";
                COLUMN.DataType = typeof(string);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ParentEntityId";
                COLUMN.DataType = typeof(int);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Adjustment";
                COLUMN.DataType = typeof(double);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IsGross?";
                COLUMN.DataType = typeof(bool);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                TaxNoteAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                TaxNoteAdjustment.Columns.Add(COLUMN);


                foreach (TaxNoteAdjustmentsPerm rowItem in fra)
                {
                    DataRow DR = TaxNoteAdjustment.NewRow();
                    DR["TNAdjId"] = rowItem.TNAdjId_Gross == null ? DBNull.Value : (object)rowItem.TNAdjId_Gross;
                    DR["GroupingName"] = rowItem.GroupingName;
                    DR["ParentEntityId"] = rowItem.ParentEntityId;
                    DR["Period"] = rowItem.Period;
                    DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR["Description"] = rowItem.Description;
                    DR["Adjustment"] = rowItem.GrossAdjustment == null ? DBNull.Value : (object)rowItem.GrossAdjustment;
                    DR["IsGross?"] = true;
                    DR["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;
                    TaxNoteAdjustment.Rows.Add(DR);

                    DataRow DR1 = TaxNoteAdjustment.NewRow();
                    DR1["TNAdjId"] = rowItem.TNAdjId_Net == null ? DBNull.Value : (object)rowItem.TNAdjId_Net;
                    DR1["GroupingName"] = rowItem.GroupingName;
                    DR1["ParentEntityId"] = rowItem.ParentEntityId;
                    DR1["Period"] = rowItem.Period;
                    DR1["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR1["Description"] = rowItem.Description;
                    DR1["Adjustment"] = rowItem.NetAdjustment == null ? DBNull.Value : (object)rowItem.NetAdjustment;
                    DR1["IsGross?"] = false;
                    DR1["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                    DR1["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR1["CreatedBy"] = UserID;
                    DR1["CreatedOn"] = DateTime.Now;
                    DR1["ModifiedBy"] = UserID;
                    DR1["ModifiedOn"] = DateTime.Now;
                    TaxNoteAdjustment.Rows.Add(DR1);
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_TaxNoteAdjustments";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = TaxNoteAdjustment;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_TaxNoteAdjustments_BulkSave]", conn))
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