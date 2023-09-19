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
using ITPFunctions.Models;

namespace ITPFunctions.MasterData
{
    public static class JurisdictionFunc
    {
        [FunctionName("Jurisdiction")]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Jurisdiction")] HttpRequestMessage req,
             ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var jur = JsonConvert.DeserializeObject<IEnumerable<Jurisdiction>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable jurisdiction = new DataTable();

                // Adding Columns
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "JurisdictionId";
                COLUMN.DataType = typeof(int);
                jurisdiction.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Jurisdiction";
                COLUMN.DataType = typeof(string);
                jurisdiction.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxRate";
                COLUMN.DataType = typeof(double);
                jurisdiction.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                jurisdiction.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                jurisdiction.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                jurisdiction.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                jurisdiction.Columns.Add(COLUMN);

                DateTime timestamp = DateTime.Now;

                foreach (Jurisdiction rowItem in jur)
                {
                    DataRow DR = jurisdiction.NewRow();
                    DR["JurisdictionId"] = rowItem.JurisdictionId == null ? DBNull.Value : (object)rowItem.JurisdictionId;
                    DR["Jurisdiction"] = rowItem.JurisdictionName == "" ? throw new ArgumentNullException("Empty jurisdiction name is not allowed!") : (object)rowItem.JurisdictionName;
                    DR["TaxRate"] = rowItem.TaxRate;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = timestamp;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = timestamp;

                    jurisdiction.Rows.Add(DR);
                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_Jurisdiction";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = jurisdiction;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Common].[sp_Jurisdiction_BulkSave]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(Parameter);
                    cmd.Parameters.Add(Parameter1);

                    try
                    {
                        int res = cmd.ExecuteNonQuery();
                        return new OkObjectResult(cmd.Parameters[1].Value);
                    }
                    catch (Exception ex)
                    {
                        return new OkObjectResult(ex.Message);
                    }
                }
            }
        }
    }
}
