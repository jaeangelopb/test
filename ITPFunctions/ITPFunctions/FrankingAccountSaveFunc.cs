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
    public static class FrankingAccountSaveFunc
    {
        [FunctionName("FrankingAccountSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "FrankingAccountSave")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var fra = JsonConvert.DeserializeObject<IEnumerable<FrankingAccount>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable frankingAccount = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "FAId";
                COLUMN.DataType = typeof(int);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Date";
                COLUMN.DataType = typeof(DateTime);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "FrankedDividendPaid";
                COLUMN.DataType = typeof(double);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "FrankedDividendReceived";
                COLUMN.DataType = typeof(double);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxRefunds";
                COLUMN.DataType = typeof(double);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxPayments";
                COLUMN.DataType = typeof(double);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Balance";
                COLUMN.DataType = typeof(double);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                frankingAccount.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                frankingAccount.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                frankingAccount.Columns.Add(COLUMN);


                foreach (FrankingAccount rowItem in fra)
                {
                    try
                    {
                        DataRow DR = frankingAccount.NewRow();
                        DR["FAId"] = rowItem.FAId == null ? DBNull.Value : (object)rowItem.FAId;
                        DR["EntityId"] = rowItem.EntityId;
                        DR["Period"] = rowItem.Period;
                        DR["Date"] = rowItem.Date == null ? DBNull.Value : (object)rowItem.Date;
                        DR["Description"] = rowItem.Description == null ? DBNull.Value : (object)rowItem.Description;
                        DR["FrankedDividendPaid"] = rowItem.FrankedDividendPaid == null ? DBNull.Value : (object)rowItem.FrankedDividendPaid;
                        DR["FrankedDividendReceived"] = rowItem.FrankedDividendReceived == null ? DBNull.Value : (object)rowItem.FrankedDividendReceived;
                        DR["TaxRefunds"] = rowItem.TaxRefunds == null ? DBNull.Value : (object)rowItem.TaxRefunds;
                        DR["TaxPayments"] = rowItem.TaxPayments == null ? DBNull.Value : (object)rowItem.TaxPayments;
                        DR["Balance"] = rowItem.Balance == null ? DBNull.Value : (object)rowItem.Balance;
                        DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                        DR["CreatedBy"] = UserID;
                        DR["CreatedOn"] = DateTime.Now;
                        DR["ModifiedBy"] = UserID;
                        DR["ModifiedOn"] = DateTime.Now;
                        frankingAccount.Rows.Add(DR);
                    }
                    catch (Exception ex)
                    {
                        return new OkObjectResult(ex.Message);
                    }
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_FrankingAccount";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = frankingAccount;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_FrankingAccount_BulkSave]", conn))
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
