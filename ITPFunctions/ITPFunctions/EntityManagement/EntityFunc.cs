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
    public static class EntityFunc
    {
        [FunctionName("Entity")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Entity")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var ent = JsonConvert.DeserializeObject<IEnumerable<Entity>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable entity = new DataTable();

                // Adding Columns
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "JurisdictionId";
                COLUMN.DataType = typeof(int);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ReportingCurrencyId";
                COLUMN.DataType = typeof(int);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityCode";
                COLUMN.DataType = typeof(string);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityName";
                COLUMN.DataType = typeof(string);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityType";
                COLUMN.DataType = typeof(string);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TFN";
                COLUMN.DataType = typeof(string);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ABN";
                COLUMN.DataType = typeof(string);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Active";
                COLUMN.DataType = typeof(bool);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "SetUpDate";
                COLUMN.DataType = typeof(DateTime);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "WoundUpDate";
                COLUMN.DataType = typeof(DateTime);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                entity.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                entity.Columns.Add(COLUMN);

                DateTime timestamp = DateTime.Now;

                foreach (Entity rowItem in ent)
                {
                    DataRow DR = entity.NewRow();
                    DR["EntityId"] = rowItem.EntityId == null ? DBNull.Value : (object)rowItem.EntityId;
                    DR["JurisdictionId"] = rowItem.JurisdictionId;
                    DR["ReportingCurrencyId"] = rowItem.ReportingCurrencyId;
                    DR["EntityCode"] = rowItem.EntityCode;
                    DR["EntityName"] = rowItem.EntityName;
                    DR["EntityType"] = rowItem.EntityType;
                    DR["TFN"] = rowItem.TFN;
                    DR["ABN"] = rowItem.ABN;
                    DR["Active"] = rowItem.Active;
                    DR["SetUpDate"] = rowItem.SetUpDate == null ? DBNull.Value : (object)rowItem.SetUpDate;
                    DR["WoundUpDate"] = rowItem.WoundUpDate == null ? DBNull.Value : (object)rowItem.WoundUpDate;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = timestamp;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = timestamp;

                    entity.Rows.Add(DR);
                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_Entity";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = entity;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Core].[sp_Entity_BulkSave]", conn))
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
