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
    public static class TaxAdjustmentCategoryInsertFunc
    {
        [FunctionName("TaxAdjustmentCategoryInsertFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "TaxAdjustmentCategoryInsert")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            var Version = req.Headers.GetValues("MappingName").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var fra = JsonConvert.DeserializeObject<IEnumerable<TaxAdjustmentCategoryInsert>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable TaxAdjustmentCategoryInsert = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "TACId";
                COLUMN.DataType = typeof(int);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TNCId";
                COLUMN.DataType = typeof(int);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "DisclosuresId";
                COLUMN.DataType = typeof(int);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Description";
                COLUMN.DataType = typeof(string);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "AdjustmentCategory";
                COLUMN.DataType = typeof(string);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Source";
                COLUMN.DataType = typeof(string);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Default";
                COLUMN.DataType = typeof(bool);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Sorting";
                COLUMN.DataType = typeof(string);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                TaxAdjustmentCategoryInsert.Columns.Add(COLUMN);


                foreach (TaxAdjustmentCategoryInsert rowItem in fra)
                {
                    try
                    {
                        DataRow DR = TaxAdjustmentCategoryInsert.NewRow();
                        DR["TACId"] = rowItem.TACId == null ? DBNull.Value : (object)rowItem.TACId;
                        DR["TNCId"] = rowItem.TNCId == null ? DBNull.Value : (object)rowItem.TNCId;
                        DR["DisclosuresId"] = rowItem.DisclosuresId == null ? DBNull.Value : (object)rowItem.DisclosuresId;
                        DR["Description"] = rowItem.Description;
                        DR["AdjustmentCategory"] = rowItem.AdjustmentCategory;
                        DR["Source"] = Version;
                        DR["Default"] = rowItem.Default;
                        DR["Sorting"] = rowItem.Sorting;
                        DR["CreatedBy"] = UserID;
                        DR["CreatedOn"] = DateTime.Now;
                        DR["ModifiedBy"] = UserID;
                        DR["ModifiedOn"] = DateTime.Now;
                        TaxAdjustmentCategoryInsert.Rows.Add(DR);
                    }
                    catch (Exception ex)
                    {

                        throw ;
                    }

                   


                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_TaxAdjustmentCategory_Insert";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = TaxAdjustmentCategoryInsert;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Common].[sp_TaxAdjustmentCategory_BulkInsert]", conn))
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
