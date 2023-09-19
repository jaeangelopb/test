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
    public static class NOLAdjustmentsFunc
    {
        [FunctionName("NOLAdjustmentsFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "NOLAdjustments")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var dtm = JsonConvert.DeserializeObject<IEnumerable<NOLAdjustments>>(body as string);
            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable NOLAdjustmentsData = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN = new DataColumn();
                COLUMN.ColumnName = "NOLId";
                COLUMN.DataType = typeof(int);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TACId";
                COLUMN.DataType = typeof(int);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "YearIncurred";
                COLUMN.DataType = typeof(string);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "YearOfExpiry";
                COLUMN.DataType = typeof(string);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TaxLossAmount";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OpeningBalanceRolledForward";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);



                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OpeningBalanceAdjustment";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);



                COLUMN = new DataColumn();
                COLUMN.ColumnName = "LossesPreviouslyUnrecognisedNowRecognised";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CurrentYearLosses";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "LossesUtilisedInOut";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "LossesExpiredInOut";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "UnrecognisedLossesOtherAdjustment";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "LossTransfersInOut";
                COLUMN.DataType = typeof(double);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IsRecognised?";
                COLUMN.DataType = typeof(bool);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Entity";
                COLUMN.DataType = typeof(string);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Comments";
                COLUMN.DataType = typeof(string);
                NOLAdjustmentsData.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                NOLAdjustmentsData.Columns.Add(COLUMN);


                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                NOLAdjustmentsData.Columns.Add(COLUMN);



                foreach (NOLAdjustments rowItem in dtm)
                {
                    try
                    {
                        DataRow DR = NOLAdjustmentsData.NewRow();
                        DR["NOLId"] = rowItem.Recognised_NOLId == null ? DBNull.Value : (object)rowItem.Recognised_NOLId;
                        DR["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                        DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                        DR["EntityId"] = rowItem.EntityId;
                        DR["Period"] = rowItem.Period == null ? DBNull.Value : (object)rowItem.Period;
                        DR["YearIncurred"] = rowItem.YearIncurred;
                        DR["YearOfExpiry"] = rowItem.YearOfExpiry;
                        DR["TaxLossAmount"] = rowItem.TaxLossAmount == null ? DBNull.Value : (object)rowItem.TaxLossAmount;
                        DR["OpeningBalanceAdjustment"] = rowItem.Recognised_OpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.Recognised_OpeningBalanceAdjustment;
                        DR["LossesPreviouslyUnrecognisedNowRecognised"] = rowItem.Recognised_LossesPreviouslyUnrecognisedNowRecognised == null ? DBNull.Value : (object)rowItem.Recognised_LossesPreviouslyUnrecognisedNowRecognised;
                        DR["CurrentYearLosses"] = rowItem.Recognised_CurrentYearLosses == null ? DBNull.Value : (object)rowItem.Recognised_CurrentYearLosses;
                        DR["LossesUtilisedInOut"] = rowItem.Recognised_LossesUtilisedInOut == null ? DBNull.Value : (object)rowItem.Recognised_LossesUtilisedInOut;
                        DR["LossesExpiredInOut"] = rowItem.Recognised_LossesExpiredInOut == null ? DBNull.Value : (object)rowItem.Recognised_LossesExpiredInOut;
                        DR["UnrecognisedLossesOtherAdjustment"] = rowItem.Recognised_UnrecognisedLossesOtherAdjustment == null ? DBNull.Value : (object)rowItem.Recognised_UnrecognisedLossesOtherAdjustment;
                        DR["LossTransfersInOut"] = rowItem.Recognised_LossTransfersInOut == null ? DBNull.Value : (object)rowItem.Recognised_LossTransfersInOut;
                        DR["IsRecognised?"] = true;
                        DR["Entity"] = rowItem.Entity == null ? DBNull.Value : (object)rowItem.Entity;
                        DR["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                        DR["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                        DR["CreatedBy"] = UserID;
                        DR["CreatedOn"] = DateTime.Now;
                        DR["ModifiedBy"] = UserID;
                        DR["ModifiedOn"] = DateTime.Now;

                        NOLAdjustmentsData.Rows.Add(DR);

                        DataRow DR1 = NOLAdjustmentsData.NewRow();
                        DR1["NOLId"] = rowItem.Unrecognised_NOLId == null ? DBNull.Value : (object)rowItem.Unrecognised_NOLId;
                        DR1["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                        DR1["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                        DR1["EntityId"] = rowItem.EntityId;
                        DR1["Period"] = rowItem.Period == null ? DBNull.Value : (object)rowItem.Period;
                        DR1["YearIncurred"] = rowItem.YearIncurred;
                        DR1["YearOfExpiry"] = rowItem.YearOfExpiry;
                        DR1["TaxLossAmount"] = rowItem.TaxLossAmount == null ? DBNull.Value : (object)rowItem.TaxLossAmount;
                        DR1["OpeningBalanceAdjustment"] = rowItem.Unrecognised_OpeningBalanceAdjustment == null ? DBNull.Value : (object)rowItem.Unrecognised_OpeningBalanceAdjustment;
                        DR1["CurrentYearLosses"] = rowItem.Unrecognised_CurrentYearLosses == null ? DBNull.Value : (object)rowItem.Unrecognised_CurrentYearLosses;
                        DR1["LossesUtilisedInOut"] = rowItem.Unrecognised_LossesUtilisedInOut == null ? DBNull.Value : (object)rowItem.Unrecognised_LossesUtilisedInOut;
                        DR1["LossesExpiredInOut"] = rowItem.Unrecognised_LossesExpiredInOut == null ? DBNull.Value : (object)rowItem.Unrecognised_LossesExpiredInOut;
                        DR1["LossTransfersInOut"] = rowItem.Unrecognised_LossTransfersInOut == null ? DBNull.Value : (object)rowItem.Unrecognised_LossTransfersInOut;
                        DR1["IsRecognised?"] = false;
                        DR1["Entity"] = rowItem.Entity == null ? DBNull.Value : (object)rowItem.Entity;
                        DR1["Comments"] = rowItem.Comments == null ? DBNull.Value : (object)rowItem.Comments;
                        DR1["Process"] = rowItem.Process == null ? DBNull.Value : (object)rowItem.Process;
                        DR1["CreatedBy"] = UserID;
                        DR1["CreatedOn"] = DateTime.Now;
                        DR1["ModifiedBy"] = UserID;
                        DR1["ModifiedOn"] = DateTime.Now;

                        NOLAdjustmentsData.Rows.Add(DR1);
                    }
                    catch (Exception ex)
                    {
                        return new OkObjectResult(ex.Message);

                    }
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_NOLAdjustments";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = NOLAdjustmentsData;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_NOLAdjustments_BulkSave]", conn))
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
