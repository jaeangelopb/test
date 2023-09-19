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
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace ITPFunctions
{
    public static class GetTEASummaryFunc
    {
        [FunctionName("GetTEASummaryFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetTEASummary")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string responseMessage = "";
            //string name = req.Query["name"];

            string name = req.Query["name"];
            string EntityId = req.Query["EntityId"];
            string Period = req.Query["Period"];
            string Process = req.Query["Process"];
            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            string text = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    if (name.Equals("TEASummary") && !string.IsNullOrEmpty(EntityId))
                    {
                        //text = "SELECT * FROM WorkPaper.[vw_TEASummary]" +
                        //     " where [EntityId]=" + EntityId + " AND [Period]='" + Period + "' AND ([Process] = '" + Process + "' OR [Process] is null)";
                        
                        
                        
                        SqlParameter EntityIdParam = new SqlParameter();
                        EntityIdParam.ParameterName = "@EntityId";
                        EntityIdParam.SqlDbType = SqlDbType.Int;
                        EntityIdParam.Direction = ParameterDirection.Input;
                        EntityIdParam.Value = EntityId;

                        SqlParameter PeriodParam = new SqlParameter();
                        PeriodParam.ParameterName = "@Period";
                        PeriodParam.SqlDbType = SqlDbType.NVarChar;
                        PeriodParam.Direction = ParameterDirection.Input;
                        PeriodParam.Value = Period;

                        SqlParameter ProcessParam = new SqlParameter();
                        ProcessParam.ParameterName = "@Process";
                        ProcessParam.SqlDbType = SqlDbType.NVarChar;
                        ProcessParam.Direction = ParameterDirection.Input;
                        ProcessParam.Value = Process;


                        SqlParameter Parameter1 = new SqlParameter();
                        Parameter1.ParameterName = "@ReturnStatus";
                        Parameter1.Direction = ParameterDirection.ReturnValue;

                        using (SqlCommand cmd = new SqlCommand("[WorkPaper].[sp_TEASummary]", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(EntityIdParam);
                            cmd.Parameters.Add(PeriodParam);
                            cmd.Parameters.Add(ProcessParam);
                            cmd.Parameters.Add(Parameter1);
                            //Executing Procedure  
                            try
                            {
                                int res = cmd.ExecuteNonQuery();

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
                    else if (name.Equals("TEASummaryFooter") && !string.IsNullOrEmpty(EntityId))
                    {
                        text = "SELECT * FROM [WorkPaper].[vw_TEASummary_Footer] " +
                             " where [EntityId]=" + EntityId + " AND [Period]='" + Period + "' AND ([Process] = '" + Process + "' OR [Process] is null)";

                    }
                    else if (name.Equals("TEAOPBT") && !string.IsNullOrEmpty(EntityId))
                    {
                        text = "SELECT * FROM [WorkPaper].[vw_TEA_OPBT_Summary]" +
                             " where [EntityId]=" + EntityId + " AND [Period]='" + Period + "' AND ([Process] = '" + Process + "' OR [Process] is null)";
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
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }


        static String sqlDatoToJson(SqlDataReader dataReader)

        // transform the returned data to JSON

        {

            DataTable dataTable = new DataTable { TableName = "TEASummary" };

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
