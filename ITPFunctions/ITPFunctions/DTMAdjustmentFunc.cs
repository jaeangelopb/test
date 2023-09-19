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
    public static class DTMAdjustmentFunc
    {
        [FunctionName("DTMAdjustmentFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var dtm = JsonConvert.DeserializeObject<IEnumerable<DTMAdjustment>>(body as string);
            var str = Environment.GetEnvironmentVariable("SqlConnectionString");          
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable dtmAdjData = new DataTable();

                // Adding Columns    

                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "DTMAdjustmentId";
                COLUMN.DataType = typeof(int);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TACId";
                COLUMN.DataType = typeof(int);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AccountId";
                COLUMN.DataType = typeof(int);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName =  "EntityId";
                COLUMN.DataType = typeof(int);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OpeningBalanceAdjustment";
                COLUMN.DataType = typeof(double);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Under/(Over)ProvisionPriorYear";
                COLUMN.DataType = typeof(double);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Transfers";
                COLUMN.DataType = typeof(double);
                dtmAdjData.Columns.Add(COLUMN);



                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OtherMovements";
                COLUMN.DataType = typeof(double);
                dtmAdjData.Columns.Add(COLUMN);



                COLUMN = new DataColumn();
                COLUMN.ColumnName = "BusinessAcquistion";
                COLUMN.DataType = typeof(double);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "BusinessDisposal";
                COLUMN.DataType = typeof(double);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Equity";
                COLUMN.DataType = typeof(double);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ClosingBalance";
                COLUMN.DataType = typeof(double);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IsAccounting";
                COLUMN.DataType = typeof(bool);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                dtmAdjData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                dtmAdjData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                dtmAdjData.Columns.Add(COLUMN);



                foreach (DTMAdjustment rowItem in dtm)
                {
                    DataRow DR = dtmAdjData.NewRow();
                    DR["DTMAdjustmentId"] = rowItem.DTMAdjustmentId_Acct == null ? DBNull.Value : (object)rowItem.DTMAdjustmentId_Acct;
                    DR["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                    DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR["AccountId"] = rowItem.AccountId == null || rowItem.AccountId == 0 ? DBNull.Value : (object)rowItem.AccountId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["Period"] = rowItem.Period;
                    DR["OpeningBalanceAdjustment"] = rowItem.OpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.OpeningBalanceAdjustment;
                    DR["Under/(Over)ProvisionPriorYear"] = rowItem.UnderProvisionPriorYear == null ? DBNull.Value : (object)rowItem.UnderProvisionPriorYear;
                    DR["Transfers"] = rowItem.Transfers == null ? DBNull.Value : (object)rowItem.Transfers;
                    DR["OtherMovements"] = rowItem.OtherMovements == null ? DBNull.Value : (object)rowItem.OtherMovements;
                    DR["BusinessAcquistion"] = rowItem.BusinessAcquistion == null ? DBNull.Value : (object)rowItem.BusinessAcquistion;
                    DR["BusinessDisposal"] = rowItem.BusinessDisposal == null ? DBNull.Value : (object)rowItem.BusinessDisposal;
                    DR["Equity"] = rowItem.Equity == null ? DBNull.Value : (object)rowItem.Equity;
                    DR["ClosingBalance"] = DBNull.Value;
                    DR["IsAccounting"] = true;
                    DR["Comments"] = rowItem.Comments;
                    DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;

                    dtmAdjData.Rows.Add(DR);

                    DataRow DR1 = dtmAdjData.NewRow();
                    DR1["DTMAdjustmentId"] = rowItem.DTMAdjustmentId_Tax == null ? DBNull.Value : (object)rowItem.DTMAdjustmentId_Tax;
                    DR1["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                    DR1["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                    DR1["AccountId"] = rowItem.AccountId == null || rowItem.AccountId == 0 ? DBNull.Value : (object)rowItem.AccountId;
                    DR1["EntityId"] = rowItem.EntityId;
                    DR1["Period"] = rowItem.Period;
                    DR1["OpeningBalanceAdjustment"] = rowItem.TaxOpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.TaxOpeningBalanceAdjustment;
                    DR1["Under/(Over)ProvisionPriorYear"] = rowItem.TaxUnderProvisionPriorYear == null ? DBNull.Value : (object)rowItem.TaxUnderProvisionPriorYear;
                    DR1["Transfers"] = rowItem.TaxTransfers == null ? DBNull.Value : (object)rowItem.TaxTransfers;
                    DR1["OtherMovements"] = rowItem.TaxOtherMovements == null ? DBNull.Value : (object)rowItem.TaxOtherMovements;
                    DR1["BusinessAcquistion"] = rowItem.TaxBusinessAcquistion == null ? DBNull.Value : (object)rowItem.TaxBusinessAcquistion;
                    DR1["BusinessDisposal"] = rowItem.TaxBusinessDisposal == null ? DBNull.Value : (object)rowItem.TaxBusinessDisposal;
                    DR1["Equity"] = rowItem.TaxEquity == null ? DBNull.Value : (object)rowItem.TaxEquity;
                    DR1["ClosingBalance"] = rowItem.TaxClosingBalance == null ? DBNull.Value : (object)rowItem.TaxClosingBalance;
                    DR1["IsAccounting"] = false;
                    DR1["Comments"] = rowItem.Comments;
                    DR1["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                    DR1["CreatedBy"] = UserID;
                    DR1["CreatedOn"] = DateTime.Now;
                    DR1["ModifiedBy"] = UserID;
                    DR1["ModifiedOn"] = DateTime.Now;

                    dtmAdjData.Rows.Add(DR1);
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_DTMAdjustments";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = dtmAdjData;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_DTMAdjustments_BulkSave]", conn))
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
