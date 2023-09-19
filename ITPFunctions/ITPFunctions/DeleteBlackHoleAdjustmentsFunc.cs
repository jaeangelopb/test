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
    public static class DeleteBlackHoleAdjustmentFunc
    {
        [FunctionName("DeleteBlackHoleAdjustmentFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteBlackHoleAdjustment")] HttpRequest req,
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
                SqlParameter RowId = new SqlParameter();
                
                if (type.Equals("BlackHoleAdjustments"))
                {
                    RowId.ParameterName = "@BHAdjId";                    
                    procName = "[WorkPaper].[sp_BlackHoleAdjustments_Delete]";

                }

                RowId.SqlDbType = SqlDbType.Int;
                RowId.Direction = ParameterDirection.Input;
                RowId.Value = Id;

                SqlParameter ReturnStatus = new SqlParameter();
                ReturnStatus.ParameterName = "@ReturnStatus";
                ReturnStatus.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(RowId);
                    cmd.Parameters.Add(ReturnStatus);
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
