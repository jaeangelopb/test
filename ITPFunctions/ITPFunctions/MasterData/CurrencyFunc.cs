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
    public static class CurrencyFunc
    {
        [FunctionName("Currency")]
        public static async Task<IActionResult> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Currency")] HttpRequestMessage req,
             ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var curr = JsonConvert.DeserializeObject<IEnumerable<Currency>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable currency = new DataTable();

                // Adding Columns
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "CurrencyId";
                COLUMN.DataType = typeof(int);
                currency.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Currency";
                COLUMN.DataType = typeof(string);
                currency.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                currency.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                currency.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                currency.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                currency.Columns.Add(COLUMN);

                DateTime timestamp = DateTime.Now;

                foreach (Currency rowItem in curr)
                {
                    DataRow DR = currency.NewRow();
                    DR["CurrencyId"] = rowItem.CurrencyId == null ? DBNull.Value : (object)rowItem.CurrencyId;
                    DR["Currency"] = rowItem.CurrencyName == "" ? throw new ArgumentNullException("Empty currency name is not allowed!") : (object)rowItem.CurrencyName;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = timestamp;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = timestamp;

                    currency.Rows.Add(DR);
                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_Currency";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = currency;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Common].[sp_Currency_BulkSave]", conn))
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
