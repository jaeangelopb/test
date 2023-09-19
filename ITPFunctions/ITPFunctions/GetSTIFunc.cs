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

namespace ITPFunctions
{
    public static class GetSTIFunc
    {
        [FunctionName("GetSTIFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetSTI")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string responseMessage = "";
            string EntityId = req.Query["EntityId"];
            string Period = req.Query["Period"];
            string Process = req.Query["Process"];
            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            string text = string.Empty;
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                
                if (!string.IsNullOrEmpty(EntityId))
                {
                    text = "SELECT * FROM [WorkPaper].[vw_STI_Workpaper] where [EntityId]=" + EntityId + " AND [Period]='" + Period + "' AND ([Process] = '" + Process + "' OR [Process] is null) ORDER BY [IsEditable] desc, [Category Header], CASE WHEN [Tax Adjustment Category] LIKE '%[^0-9-]%' THEN 0 ELSE 1 END, CASE WHEN [Tax Adjustment Category] NOT LIKE '%[0-9-]%' THEN 0 ELSE 1 END";
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
                
            }


                return new OkObjectResult(responseMessage);
        }


        static String sqlDatoToJson(SqlDataReader dataReader)

        // transform the returned data to JSON

        {

            DataTable dataTable = new DataTable { TableName = "STI" };

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
