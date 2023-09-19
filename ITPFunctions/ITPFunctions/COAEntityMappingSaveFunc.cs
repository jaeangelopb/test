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
    public static class COAEntityMappingSaveFunc
    {
        [FunctionName("COAEntityMappingSaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "COAEntityMappingSave")] HttpRequestMessage req,
              ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var coa = JsonConvert.DeserializeObject<IEnumerable<COAEntityMapping>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                DataTable COAEntityMapping = new DataTable();

                // Adding Columns    
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityCOAMappingId";
                COLUMN.DataType = typeof(int);
                COAEntityMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                COAEntityMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "COAName";
                COLUMN.DataType = typeof(string);
                COAEntityMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                COAEntityMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                COAEntityMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                COAEntityMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                COAEntityMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                COAEntityMapping.Columns.Add(COLUMN);

                DateTime timestamp = DateTime.Now;

                foreach (COAEntityMapping rowItem in coa)
                {
                    DataRow DR = COAEntityMapping.NewRow();
                    DR["EntityCOAMappingId"] = rowItem.MappingId == null ? DBNull.Value : (object)rowItem.MappingId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["COAName"] = rowItem.COAName;
                    DR["Period"] = rowItem.Period;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = timestamp;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = timestamp;
                    COAEntityMapping.Rows.Add(DR);
                }

                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_EntityCOAMapping_Save";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = COAEntityMapping;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Core].[sp_EntityCOAMapping_BulkSave]", conn))
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
