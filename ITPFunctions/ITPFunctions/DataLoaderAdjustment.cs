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
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ITPFunctions
{
    public static class DataLoaderAdjustment
    {
        [FunctionName("DataLoaderAdjustement")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

             string name = req.Query["PageNo"];
            string NoOfRecords = req.Query["NoOfRecords"];
            string TotalNoOfItems = req.Query["TotalCount"];
            int stratPageNum = ((Convert.ToInt32(name)-1) * Convert.ToInt32(NoOfRecords))+ 1;
            //int ttlRecords = Convert.ToInt32(name) - 1 * Convert.ToInt32(stratPageNum);
            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //  dynamic data = JsonConvert.DeserializeObject(requestBody);
            //  name = name ?? data?.name;

            string responseMessage = "";

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                string text;
                if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(NoOfRecords))
                { 
                text = "SELECT TOP("+ Convert.ToInt32(NoOfRecords) + ") * FROM [dbo].[vw_Adjustments] where [RowNumber] >=" + stratPageNum+ "order by [RowNumber]";
                } 
                else if(!string.IsNullOrEmpty(TotalNoOfItems))
                {
                    text = "SELECT Count(*) FROM [dbo].[vw_Adjustments]";
                }
                else
                {
                    text = "SELECT * FROM [dbo].[vw_Adjustments]";
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

            DataTable dataTable = new DataTable{TableName= "Adjustements"};

            dataTable.Load(dataReader);

           

            string JSONString = string.Empty;

            JSONString = JsonConvert.SerializeObject(dataTable);
            //MemoryStream str = new MemoryStream();

            //dataTable.WriteXml(str, true);

            //str.Seek(0, SeekOrigin.Begin);

            //StreamReader sr = new StreamReader(str);

            string xmlstr;

            //xmlstr = sr.ReadToEnd();

            //return (xmlstr);
            //List<Adjustement> jsonres = JsonConvert.DeserializeObject<List<Adjustement>>(JSONString);
            //XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonres.ToString());



            //StringBuilder output = new StringBuilder();
            //using (var sw = new StringWriter(output))
            //    doc.WriteTo(new XmlTextWriter(sw));

          
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
