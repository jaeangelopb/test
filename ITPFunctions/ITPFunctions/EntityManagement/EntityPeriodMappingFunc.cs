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

namespace ITPFunctions.EntityManagement
{
    public static class EntityPeriodMappingFunc
    {
        [FunctionName("EntityPeriodMapping")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "EntityPeriodMapping")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var epm = JsonConvert.DeserializeObject<IEnumerable<EntityPeriodMapping>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable entityPeriodMapping = new DataTable();

                // Adding Columns
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityPeriodId";
                COLUMN.DataType = typeof(int);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "YearId";
                COLUMN.DataType = typeof(int);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "MonthStartId";
                COLUMN.DataType = typeof(int);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "MonthEndId";
                COLUMN.DataType = typeof(int);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Frequency";
                COLUMN.DataType = typeof(string);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                entityPeriodMapping.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                entityPeriodMapping.Columns.Add(COLUMN);

                DateTime timestamp = DateTime.Now;

                foreach (EntityPeriodMapping rowItem in epm)
                {
                    DataRow DR = entityPeriodMapping.NewRow();
                    DR["EntityPeriodId"] = rowItem.EntityPeriodId == null ? DBNull.Value : (object)rowItem.EntityPeriodId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["YearId"] = rowItem.YearId;
                    DR["MonthStartId"] = rowItem.MonthStartId;
                    DR["MonthEndId"] = rowItem.MonthEndId;
                    DR["Frequency"] = rowItem.Frequency;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = timestamp;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = timestamp;

                    entityPeriodMapping.Rows.Add(DR);
                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_EntityPeriodMapping";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = entityPeriodMapping;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Core].[sp_EntityPeriodMapping_BulkSave]", conn))
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
                    catch (SqlException ex)
                    {
                        return new BadRequestObjectResult(ex.Message);
                    }
                }
            }

            return new OkObjectResult("success");
        }
    }
}
