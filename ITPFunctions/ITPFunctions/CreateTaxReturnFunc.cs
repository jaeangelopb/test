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
using ITPFunctions.Models;

namespace ITPFunctions
{
    internal class CreateTaxReturnFunc
    {
        [FunctionName("CreateTaxReturnFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CreateTaxReturn")] HttpRequestMessage req,
            ILogger log)
        {
            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            log.LogInformation("C# HTTP trigger function processed a request.");
            int res;
            string procName = string.Empty;
            int result = 0;

            string body = await req.Content.ReadAsStringAsync();

            var fra = JsonConvert.DeserializeObject<IEnumerable<SaveTaxReturn>>(body);

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");

            using (SqlConnection conn = new SqlConnection(str))
            {
                await conn.OpenAsync();

                foreach (SaveTaxReturn rowItem in fra)
                {

                    SqlParameter EntityID = new SqlParameter();
                    SqlParameter Period = new SqlParameter();
                    SqlParameter Process = new SqlParameter();
                    SqlParameter FileName = new SqlParameter();
                    SqlParameter Userid = new SqlParameter();

                    EntityID.SqlDbType = SqlDbType.Int;
                    EntityID.Direction = ParameterDirection.Input;
                    EntityID.Value = rowItem.EntityID;

                    Period.SqlDbType = SqlDbType.NVarChar;
                    Period.Direction = ParameterDirection.Input;
                    Period.Value = rowItem.Period;

                    FileName.SqlDbType = SqlDbType.NVarChar;
                    FileName.Direction = ParameterDirection.Input;
                    FileName.Value = rowItem.FileName;

                    Process.SqlDbType = SqlDbType.NVarChar;
                    Process.Direction = ParameterDirection.Input;
                    Process.Value = rowItem.Process;

                    Userid.SqlDbType = SqlDbType.NVarChar;
                    Userid.Direction = ParameterDirection.Input;
                    Userid.Value = UserID;


                    EntityID.ParameterName = "@EntityID";
                    Period.ParameterName = "@Period";
                    FileName.ParameterName = "@FileName";
                    Process.ParameterName = "@Process";
                    Userid.ParameterName = "@UserID";
                    procName = "[Core].[sp_CreateTaxReturn_BulkCopy]";

                    SqlParameter ReturnStatus = new SqlParameter();
                    ReturnStatus.ParameterName = "@ReturnStatus";
                    ReturnStatus.Direction = ParameterDirection.ReturnValue;

                    using (SqlCommand cmd = new SqlCommand(procName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(EntityID);
                        cmd.Parameters.Add(Period);
                        cmd.Parameters.Add(FileName ?? Convert.DBNull);
                        cmd.Parameters.Add(Process);
                        cmd.Parameters.Add(Userid);
                        cmd.Parameters.Add(ReturnStatus);

                        try
                        {
                            res = await cmd.ExecuteNonQueryAsync();
                            result = Convert.ToInt32(cmd.Parameters[5].Value);
                        }
                        catch (SqlException ex)
                        {
                            return new BadRequestObjectResult(ex.Message);
                        }
                    }
                }

                return new OkObjectResult(result);

            }
        }
    }
}
