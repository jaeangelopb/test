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
    public static class OtherTemporaryAdjustmentsFunc
    {
        [FunctionName("OtherTemporaryAdjustments")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "OTA")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var ota = JsonConvert.DeserializeObject<IEnumerable<OtherTemporaryAdjustment>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable otherTemporaryAdjustment = new DataTable();

                //Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "OTAId";
                COLUMN.DataType = typeof(int);
                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DisclosuresId";
                COLUMN.DataType = typeof(int);
                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxYear";
                COLUMN.DataType = typeof(string);
                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OpeningBalanceAdjustment";
                COLUMN.DataType = typeof(double);
                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "UnderorOverProvisionPriorYear";
                COLUMN.DataType = typeof(double);

                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Transfers";
                COLUMN.DataType = typeof(double);

                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OtherMovements";
                COLUMN.DataType = typeof(double);

                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "BusinessAcquisition";
                COLUMN.DataType = typeof(double);

                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "BusinessDisposal";
                COLUMN.DataType = typeof(double);

                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Equity";
                COLUMN.DataType = typeof(double);

                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ClosingBalance";
                COLUMN.DataType = typeof(double);

                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IsAccounting";
                COLUMN.DataType = typeof(bool);

                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);

                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);

                otherTemporaryAdjustment.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);

                otherTemporaryAdjustment.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);

                otherTemporaryAdjustment.Columns.Add(COLUMN);




                // INSERTING DATA    
                foreach (OtherTemporaryAdjustment rowItem in ota)
                {
                    DataRow DR = otherTemporaryAdjustment.NewRow();
                    DR["OTAId"] = rowItem.OTAId_Acct == null ? DBNull.Value : (object)rowItem.OTAId_Acct;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["Description"] = rowItem.Description == "" ? throw new ArgumentNullException("Empty description is not allowed!") : (object)rowItem.Description;
                    DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR["DisclosuresId"] = rowItem.DisclosuresId;
                    DR["TaxYear"] = rowItem.TaxYear;
                    DR["Period"] = rowItem.Period;
                    DR["OpeningBalanceAdjustment"] = rowItem.OpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.OpeningBalanceAdjustment;
                    DR["UnderorOverProvisionPriorYear"] = rowItem.UnderOrOverProvisionPriorYear == null ? DBNull.Value : (object)rowItem.UnderOrOverProvisionPriorYear;
                    DR["Transfers"] = rowItem.Transfers == null ? DBNull.Value : (object)rowItem.Transfers;
                    DR["OtherMovements"] = rowItem.OtherMovements == null ? DBNull.Value : (object)rowItem.OtherMovements;
                    DR["BusinessAcquisition"] = rowItem.BusinessAcquistion == null ? DBNull.Value : (object)rowItem.BusinessAcquistion;
                    DR["BusinessDisposal"] = rowItem.BusinessDisposal == null ? DBNull.Value : (object)rowItem.BusinessDisposal;
                    DR["Equity"] = rowItem.Equity == null ? DBNull.Value : (object)rowItem.Equity;
                    DR["ClosingBalance"] = rowItem.ClosingBalance == null ? DBNull.Value : (object)rowItem.ClosingBalance;
                    DR["IsAccounting"] = 1;
                    DR["Comments"] = rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;

                    otherTemporaryAdjustment.Rows.Add(DR);

                    DataRow DR1 = otherTemporaryAdjustment.NewRow();
                    DR1["OTAId"] = rowItem.OTAId_Tax == null ? DBNull.Value : (object)rowItem.OTAId_Tax;
                    DR1["EntityId"] = rowItem.EntityId;
                    DR1["Description"] = rowItem.Description == "" ? throw new ArgumentNullException("Empty description is not allowed!") : (object)rowItem.Description;
                    DR1["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR1["DisclosuresId"] = rowItem.DisclosuresId;
                    DR1["TaxYear"] = rowItem.TaxYear;
                    DR1["Period"] = rowItem.Period;
                    DR1["OpeningBalanceAdjustment"] = rowItem.TaxOpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.TaxOpeningBalanceAdjustment;
                    DR1["UnderorOverProvisionPriorYear"] = rowItem.TaxUnderOrOverProvisionPriorYear == null ? DBNull.Value : (object)rowItem.TaxUnderOrOverProvisionPriorYear;
                    DR1["Transfers"] = rowItem.TaxTransfers == null ? DBNull.Value : (object)rowItem.TaxTransfers;
                    DR1["OtherMovements"] = rowItem.TaxOtherMovements == null ? DBNull.Value : (object)rowItem.TaxOtherMovements;
                    DR1["BusinessAcquisition"] = rowItem.TaxBusinessAcquistion == null ? DBNull.Value : (object)rowItem.TaxBusinessAcquistion;
                    DR1["BusinessDisposal"] = rowItem.TaxBusinessDisposal == null ? DBNull.Value : (object)rowItem.TaxBusinessDisposal;
                    DR1["Equity"] = rowItem.TaxEquity == null ? DBNull.Value : (object)rowItem.TaxEquity;
                    DR1["ClosingBalance"] = rowItem.TaxClosingBalance == null ? DBNull.Value : (object)rowItem.TaxClosingBalance;
                    DR1["IsAccounting"] = 0;
                    DR1["Comments"] = rowItem.Comments;
                    DR1["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR1["CreatedBy"] = UserID;
                    DR1["CreatedOn"] = DateTime.Now;
                    DR1["ModifiedBy"] = UserID;
                    DR1["ModifiedOn"] = DateTime.Now;

                    otherTemporaryAdjustment.Rows.Add(DR1);

                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_OTA_BulkSave";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = otherTemporaryAdjustment;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Core].[sp_OTA_BulkSave]", conn))
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
