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
    public static class EntityOwnershipFunc
    {
        [FunctionName("EntityOwnership")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "EntityOwnership")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var eno = JsonConvert.DeserializeObject<IEnumerable<EntityOwnership>>(body as string);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                //Creating Table    
                DataTable entityOwnership = new DataTable();

                // Adding Columns
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityOwnershipId";
                COLUMN.DataType = typeof(int);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "GroupingName";
                COLUMN.DataType = typeof(string);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ParentEntityId";
                COLUMN.DataType = typeof(int);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "SubEntityId";
                COLUMN.DataType = typeof(int);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "OwnershipPercentage";
                COLUMN.DataType = typeof(double);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntryDate";
                COLUMN.DataType = typeof(DateTime);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ExitDate";
                COLUMN.DataType = typeof(DateTime);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                entityOwnership.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                entityOwnership.Columns.Add(COLUMN);

                DateTime timestamp = DateTime.Now;

                foreach (EntityOwnership rowItem in eno)
                {
                    DataRow DR = entityOwnership.NewRow();
                    DR["EntityOwnershipId"] = rowItem.EntityOwnershipId == null ? DBNull.Value : (object)rowItem.EntityOwnershipId;
                    DR["GroupingName"] = rowItem.GroupingName;
                    DR["ParentEntityId"] = rowItem.ParentEntityId;
                    DR["SubEntityId"] = rowItem.SubEntityId;
                    DR["OwnershipPercentage"] = rowItem.OwnershipPercentage == null ? DBNull.Value : (object)rowItem.OwnershipPercentage;
                    DR["EntryDate"] = rowItem.EntryDate == null ? DBNull.Value : (object)rowItem.EntryDate;
                    DR["ExitDate"] = rowItem.ExitDate == null ? DBNull.Value : (object)rowItem.ExitDate;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = timestamp;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = timestamp;

                    entityOwnership.Rows.Add(DR);
                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_EntityOwnership";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = entityOwnership;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Core].[sp_EntityOwnership_BulkSave]", conn))
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
