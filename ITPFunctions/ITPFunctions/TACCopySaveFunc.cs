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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace ITPFunctions
{
    public static class TACCopySaveFunc
    {
        [FunctionName("TACCopySaveFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "TACCopySave")] HttpRequestMessage req,
              ILogger log)
        {

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            string procName = string.Empty;

            dynamic body = await req.Content.ReadAsStringAsync();

            var fra = JsonConvert.DeserializeObject<IEnumerable<TaxAdjustmentCategory>>(body as string);
            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {

                conn.Open();

                SqlParameter MappingName = new SqlParameter();
                SqlParameter OldMappingName = new SqlParameter();
                SqlParameter Userid = new SqlParameter();
                foreach (TaxAdjustmentCategory rowItem in fra)
                {
                    MappingName.SqlDbType = SqlDbType.NVarChar;
                    MappingName.Direction = ParameterDirection.Input;
                    MappingName.Value = rowItem.MappingName;


                    OldMappingName.SqlDbType = SqlDbType.NVarChar;
                    OldMappingName.Direction = ParameterDirection.Input;
                    OldMappingName.Value = rowItem.OldMappingName;


                    Userid.SqlDbType = SqlDbType.NVarChar;
                    Userid.Direction = ParameterDirection.Input;
                    Userid.Value = UserID;
                }
                //Parameter declaration    
                MappingName.ParameterName = "@MappingName";

                OldMappingName.ParameterName = "@OldMappingName";


                Userid.ParameterName = "@UserID";
                procName = "[Common].[sp_TACMapping_BulkCopy]";

               

                SqlParameter ReturnStatus = new SqlParameter();
                ReturnStatus.ParameterName = "@ReturnStatus";
                ReturnStatus.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(MappingName);
                    cmd.Parameters.Add(OldMappingName);
                    cmd.Parameters.Add(Userid);
                    cmd.Parameters.Add(ReturnStatus);
                    //Executing Procedure  
                    try
                    {
                        res = cmd.ExecuteNonQuery();
                        return new OkObjectResult(cmd.Parameters[3].Value);
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
