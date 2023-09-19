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
    public static class ConduitForeignIncomeSaveFunc
    {
        [FunctionName("ConduitForeignIncomeSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ConduitForeignIncomeSave")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var fra = JsonConvert.DeserializeObject<IEnumerable<ConduitForeignIncome>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable ConduitForeignIncome = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "CFIId";
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
                COLUMN.ColumnName = "Date";
                COLUMN.DataType = typeof(DateTime);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ForeignDividendReceivedInLocalCurrency";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ExchangeRate";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ForeignDividendPaidInAUD";
                COLUMN.DataType = typeof(double);
                ConduitForeignIncome.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Balance";
                COLUMN.DataType = typeof(double);
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


                foreach (ConduitForeignIncome rowItem in fra)
                {
                   
                        DataRow DR = ConduitForeignIncome.NewRow();
                        DR["CFIId"] = rowItem.CFIId == null ? DBNull.Value : (object)rowItem.CFIId;
                        DR["EntityId"] = rowItem.EntityId;
                        DR["Period"] = rowItem.Period;
                        DR["Date"] = rowItem.Date == null ? DBNull.Value : (object)rowItem.Date;
                        DR["Description"] = rowItem.Description == null ? DBNull.Value : (object)rowItem.Description;
                        DR["ForeignDividendReceivedInLocalCurrency"] = rowItem.ForeignDividendReceivedInLocalCurrency == null ? DBNull.Value : (object)rowItem.ForeignDividendReceivedInLocalCurrency;
                        DR["ExchangeRate"] = rowItem.ExchangeRate == null ? DBNull.Value : (object)rowItem.ExchangeRate;
                        DR["ForeignDividendPaidInAUD"] = rowItem.ForeignDividendPaidInAUD == null ? DBNull.Value : (object)rowItem.ForeignDividendPaidInAUD;
                        DR["Balance"] = rowItem.Balance == null ? DBNull.Value : (object)rowItem.Balance;
                        DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                        DR["CreatedBy"] = UserID;
                        DR["CreatedOn"] = DateTime.Now;
                        DR["ModifiedBy"] = UserID;
                        DR["ModifiedOn"] = DateTime.Now;
                        ConduitForeignIncome.Rows.Add(DR);
                 
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_ConduitForeignIncome";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = ConduitForeignIncome;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_ConduitForeignIncome_BulkSave]", conn))
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
