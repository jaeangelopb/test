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

namespace ITPFunctions
{
    public static class DeleteAdjustmentFunc
    {
        [FunctionName("DeleteAdjustmentFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteAdj")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string type = req.Query["Type"];
            string Id = req.Query["Id"];
            int res;
            string procName = string.Empty;

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                
                if (type.Equals("OTA"))
                {
                    Parameter.ParameterName = "@OTAId";                    
                    procName = "[Core].[sp_OTA_Delete]";

                }
                else if(type.Equals("OPA"))
                {
                    Parameter.ParameterName = "@OPAId";
                    procName = "[Core].[sp_OPA_Delete]";
                }

                Parameter.SqlDbType = SqlDbType.Int;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = Id;

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
