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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Text;
using System.Xml.Serialization;

namespace ITPFunctions
{
    public static class GetConsolTaxNoteSummaryFunc
    {
        [FunctionName("GetConsolTaxNoteSummaryFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "GetConsolTaxNoteSummary")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string type = req.Query["Name"];
            string GroupingName = req.Query["GroupingName"];
            string Period = req.Query["Period"];
            string Process = req.Query["Process"];
            string EntityId = req.Query["EntityId"];
            int res;
            string procName = string.Empty;
            string responseMessage = "";

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                //Parameter declaration    
                SqlParameter GroupingNameParam = new SqlParameter();

                SqlParameter PeriodParam = new SqlParameter();
                SqlParameter ProcessParam = new SqlParameter();
                SqlParameter EntityIdParam = new SqlParameter();

                if (type.Equals("Consol_Permanent"))
                {
                    GroupingNameParam.ParameterName = "@GroupingName";
                    PeriodParam.ParameterName = "@Period";
                    ProcessParam.ParameterName = "@Process";
                    EntityIdParam.ParameterName = "@EntityId";
                    procName = "[WorkPaper].[sp_Consol_TaxNoteSummary_Permanent]";

                }

                GroupingNameParam.SqlDbType = SqlDbType.NVarChar;
                GroupingNameParam.Direction = ParameterDirection.Input;
                GroupingNameParam.Value = GroupingName;

                PeriodParam.SqlDbType = SqlDbType.NVarChar;
                PeriodParam.Direction = ParameterDirection.Input;
                PeriodParam.Value = Period;

                ProcessParam.SqlDbType = SqlDbType.NVarChar;
                ProcessParam.Direction = ParameterDirection.Input;
                ProcessParam.Value = Process;

                EntityIdParam.SqlDbType = SqlDbType.Int;
                EntityIdParam.Direction = ParameterDirection.Input;
                EntityIdParam.Value = EntityId;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(GroupingNameParam);
                    cmd.Parameters.Add(PeriodParam);
                    cmd.Parameters.Add(ProcessParam);
                    cmd.Parameters.Add(EntityIdParam);
                    cmd.Parameters.Add(Parameter1);
                    //Executing Procedure  
                    try
                    {
                        res = cmd.ExecuteNonQuery();

                        var adapt = new SqlDataAdapter();
                        adapt.SelectCommand = cmd;
                        var dataset = new DataSet();
                        adapt.Fill(dataset);
                        string queryop = "";

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            queryop = sqlDatoToJson(reader);
                        }
                        responseMessage = (queryop);

                        return new OkObjectResult(responseMessage);

                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        return new BadRequestObjectResult(ex.Message);
                    }
                }
            }

        }
        static String sqlDatoToJson(SqlDataReader dataReader)

        // transform the returned data to JSON

        {
            DataTable dataTable = new DataTable { TableName = "STISummary" };

            dataTable.Load(dataReader);

            string JSONString = string.Empty;

            JSONString = JsonConvert.SerializeObject(dataTable);
            string xmlstr;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataTable));
                    xmlSerializer.Serialize(streamWriter, dataTable);
                    xmlstr = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            return JSONString;
        }
    }
}