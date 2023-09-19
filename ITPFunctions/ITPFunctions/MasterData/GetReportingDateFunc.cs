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
using System.Xml.Serialization;
using System.Text;

namespace ITPFunctions.MasterData
{
    public static class GetReportingDateFunc
    {
        [FunctionName("GetReportingDateFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetReportingDate")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            string EntityId = req.Query["EntityId"];
            string Period = req.Query["Period"];
            string procName = string.Empty;

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                //Parameter declaration    
                SqlParameter EntityIdParam= new SqlParameter();

                SqlParameter PeriodParam = new SqlParameter();

                EntityIdParam.ParameterName = "@EntityId";
                PeriodParam.ParameterName = "@Period";
                procName = "[Core].[sp_ReportingDate_Read]";

                EntityIdParam.SqlDbType = SqlDbType.Int;
                EntityIdParam.Direction = ParameterDirection.Input;
                EntityIdParam.Value = EntityId;

                PeriodParam.SqlDbType = SqlDbType.NVarChar;
                PeriodParam.Direction = ParameterDirection.Input;
                PeriodParam.Value = Period;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(EntityIdParam);
                    cmd.Parameters.Add(PeriodParam);
                    cmd.Parameters.Add(Parameter1);
                    //Executing Procedure  
                    try
                    {
                        res = cmd.ExecuteNonQuery();

                        var adapt = new SqlDataAdapter();
                        adapt.SelectCommand = cmd;
                        var dataset = new DataSet();
                        adapt.Fill(dataset);
                        conn.Close();
                        if (((System.Data.InternalDataCollectionBase)(dataset.Tables)).Count != 0)
                        {

                            return new OkObjectResult(dataset.Tables[0]);

                        }
                        else
                        {

                            return new OkObjectResult(dataset.Tables);
                        }
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
