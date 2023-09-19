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
using System.Data;

namespace ITPFunctions.EntityManagement
{
    public static class DeleteEntityCOAMappingFunc
    {
        [FunctionName("DeleteEntityCOAMappingFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteEntityCOAMapping")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string type = req.Query["Type"];
            string AccountId = req.Query["MappingId"];
            int res;
            string procName = string.Empty;

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();

                if (type.Equals("DeleteEntityCOAMapping"))
                {
                    Parameter.ParameterName = "@MappingId";
                    procName = "[Core].[sp_EntityCOAMapping_Delete]";

                }

                Parameter.SqlDbType = SqlDbType.Int;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = AccountId;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand(procName, conn))
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
                    catch (SqlException ex)
                    {
                        return new BadRequestObjectResult("Account is being used by Trial Balance and/or other workpapers");
                    }
                }
            }

            return new OkObjectResult(res);
        }
    }
}
