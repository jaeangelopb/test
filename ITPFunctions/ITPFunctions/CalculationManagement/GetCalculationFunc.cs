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
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ITPFunctions.CalculationManagement
{
    public static class GetCalculationFunc
    {
        [FunctionName("GetCalculationFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetCalculationFunc")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string responseMessage = "";

            string EntityId = req.Query["EntityId"];
            string Period = req.Query["Period"];
            string Process = req.Query["Process"];
            string Status = req.Query["Status"];
            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            string text = string.Empty;
            List<string> param = new List<string>();
            using (SqlConnection conn = new SqlConnection(str))
            {
                try
                {
                    conn.Open();

                    text = "SELECT * FROM [Core].[vw_Calculation_List] ";

                    if (!string.IsNullOrEmpty(EntityId))
                        param.Add("[EntityId] = " + EntityId + " ");
                    if (!string.IsNullOrEmpty(Period))
                        param.Add("[Period] = '" + Period + "' ");
                    if (!string.IsNullOrEmpty(Process))
                        param.Add("[Process] = '" + Process + "' ");
                    if (!string.IsNullOrEmpty(Status))
                        param.Add("[Status] = '" + Status + "' ");

                    if (param.Count > 0)
                        text += "WHERE ";
                    for (int i = 0; i < param.Count; i++)
                    {
                        text += param[i];
                        if (i < param.Count - 1)
                            text += "AND ";
                    }

                    using (SqlCommand cmd = new SqlCommand(text, conn))
                    {
                        string queryop = "";


                        using (SqlDataReader reader = cmd.ExecuteReader())

                        {

                            queryop = sqlDatoToJson(reader);

                        }


                        responseMessage = (queryop);

                    }
                    conn.Close();


                    return new OkObjectResult(responseMessage);
                }
                catch(Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }
           
        }


        static String sqlDatoToJson(SqlDataReader dataReader)

        // transform the returned data to JSON

        {

            DataTable dataTable = new DataTable { TableName = "Calculation" };

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
