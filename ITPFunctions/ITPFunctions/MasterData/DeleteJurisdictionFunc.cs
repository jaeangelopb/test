using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ITPFunctions.MasterData
{
    public static class DeleteJurisdictionFunc
    {
        [FunctionName("DeleteJurisdictionFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteJurisdiction")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string JurisdictionId = req.Query["JurisdictionId"];
            int res;
            string procName = string.Empty;

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();

                Parameter.ParameterName = "@JurisdictionId";
                procName = "[Common].[sp_Jurisdiction_Delete]";

                Parameter.SqlDbType = SqlDbType.Int;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = JurisdictionId;

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
                    catch (Exception ex)
                    {
                        return new BadRequestObjectResult(ex.Message);
                    }
                }
            }
        }
    }
}
