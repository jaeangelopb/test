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
    public static class TaxAdjustmentCategorySaveFunc
    {
        [FunctionName("TaxAdjustmentCategorySaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "TaxAdjustmentCategorySave")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var fra = JsonConvert.DeserializeObject<IEnumerable<TaxAdjustmentCategory>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable TaxAdjustmentCategory = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "MappingId";
                COLUMN.DataType = typeof(int);
                TaxAdjustmentCategory.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "MappingName";
                COLUMN.DataType = typeof(string);
                TaxAdjustmentCategory.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Default";
                COLUMN.DataType = typeof(bool);
                TaxAdjustmentCategory.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                TaxAdjustmentCategory.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                TaxAdjustmentCategory.Columns.Add(COLUMN);


                foreach (TaxAdjustmentCategory rowItem in fra)
                {
                    DataRow DR = TaxAdjustmentCategory.NewRow();
                    DR["MappingId"] = rowItem.MappingId == null ? DBNull.Value : (object)rowItem.MappingId;
                    DR["MappingName"] = rowItem.MappingName;
                    DR["Default"] = rowItem.Default;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;
                    TaxAdjustmentCategory.Rows.Add(DR);


                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_TaxAdjustmentCategory";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = TaxAdjustmentCategory;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Common].[sp_TaxAdjustmentCategory_BulkSave]", conn))
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
