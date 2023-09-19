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
using System.ComponentModel.Design;

namespace ITPFunctions
{
    public static class GetMasterData
    {
        [FunctionName("GetMasterData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name = req.Query["name"];

            string responseMessage = "";

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                string text = string.Empty;

                switch (name)
                {
                    case "AccountType":
                        text = "SELECT * FROM [Common].[vw_AccountType_List]";
                        break;
                    case "Disclosures":
                        text = "SELECT * FROM [Common].[vw_Disclosures_List]";
                        break;
                    case "Jurisdictions":
                        text = "SELECT * FROM [Common].[vw_Jurisdiction_List]";
                        break;
                    case "TAC":
                        text = "SELECT * FROM [Common].[vw_TAC_List]";
                        break;
                    case "TNC":
                        text = "SELECT * FROM [Common].[vw_TNC_List]";
                        break;
                    case "Account":
                        text = "SELECT * FROM [Core].[vw_Account_List]";
                        break;
                    case "Entity":
                        text = "SELECT * FROM [Core].[vw_Entity_List]";
                        break;
                    case "FileValidation":
                        text = "SELECT * FROM [Core].[vw_FileValidation_List]";
                        break;
                    case "Period":
                        text = "SELECT * FROM [Core].[vw_EntityPeriodMapping_List]";
                        break;
                    case "AssetClass":
                        text = "SELECT * FROM [Common].[vw_AssetClass_List]";
                        break;
                    case "Year":
                        text = "SELECT * FROM [Common].[vw_Year_List]";
                        break;
                    case "Consol_Period":
                        text = "SELECT * FROM [Core].[vw_Consol_EntityPeriodMapping_List]";
                        break;
                    case "Consol_Entity":
                        text = "SELECT * FROM [Core].[vw_Consol_Entity_List]";
                        break;
                    case "TaxSensitivity":
                        text = "SELECT * FROM [Common].[vw_TaxSensitivity_List]";
                        break;
                    case "TaxAdjustmentCategory":
                        text = "SELECT * FROM [Common].[vw_TaxAdjustmentCategory_List]";
                        break;
                    case "COATBMappingName":
                        text = "SELECT * FROM [Core].[vw_ChartOfAccount_TBMappingName_List]";
                        break;
                    case "TACMappingName":
                        text = "SELECT * FROM [Common].[vw_TAC_MappingName_List]";
                        break;
                    case "EntityGroup":
                        text = "SELECT * FROM [Core].[vw_EntityGroup_List]";
                        break;
                    case "EntityOwnership":
                        text = "SELECT * FROM [Core].[vw_EntityOwnership_List]";
                        break;
                    case "EntityTAC":
                        text = "SELECT * FROM [Core].[vw_EntityTACMapping_List]";
                        break;
                    case "EntityCOA":
                        text = "SELECT * FROM [Core].[vw_EntityCOAMapping_List]";
                        break;
                    case "EntityPeriod":
                        text = "SELECT * FROM [Core].[vw_EntityPeriodMapping]";
                        break;
                    case "Currency":
                        text = "SELECT * FROM [Common].[vw_Currency_List]";
                        break;
                    case "HeadEntity":
                        text = "SELECT * FROM [Core].[vw_Consol_HeadEntity_List]";
                        break;
                    case "CalculationStatus":
                        text = "SELECT * FROM [Common].[vw_CalculationStatus_List]";
                        break;
                    case "TaxSensitivityType":
                        text = "SELECT * FROM [Common].[vw_TS_TACMapping_List]";
                        break;
                    case "EntityVolume":
                        text = "SELECT * FROM [Common].[vw_EntityVolume_List]";
                        break;
                    case "Process":
                        text = "SELECT * FROM [Common].[vw_Process_List]";
                        break;
                    case "TBFileList":
                        text = "SELECT * FROM [Core].[vw_TB_Calc_List]";
                        break;
                    case "Month":
                        text = "SELECT * FROM [Common].[vw_Month_List]";
                        break;
                    case "Frequency":
                        text = "SELECT * FROM [Common].[vw_PeriodFrequency_List]";
                        break;
                    case "GlobalFilterEntity":
                        text = "SELECT * FROM [Core].[vw_GlobalFilter_Entity]";
                        break;
                    case "GlobalFilterPeriod":
                        text = "SELECT * FROM [Core].[vw_GlobalFilter_Period] ORDER BY [Year] DESC, [PeriodRefId] ASC";
                        break;
                    case "GlobalFilterProcess":
                        text = "SELECT * FROM [Core].[vw_GlobalFilter_Process]";
                        break;
                    case "ConsolGlobalFilterEntity":
                        text = "SELECT * FROM [Core].[vw_GlobalFilter_Consol_Entity]";
                        break;
                    case "ConsolGlobalFilterPeriod":
                        text = "SELECT * FROM [Core].[vw_GlobalFilter_Consol_Period] ORDER BY [Year] DESC, [PeriodRefId] ASC";
                        break;
                    case "ConsolGlobalFilterProcess":
                        text = "SELECT * FROM [Core].[vw_GlobalFilter_Consol_Process]";
                        break;
                    case "ConsolGlobalFilterGroupingName":
                        text = "SELECT * FROM [Core].[vw_GlobalFilter_Consol_GroupingName]";
                        break;
                    default:
                        return new BadRequestObjectResult("Parameter 'name' provided is invalid!");
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

            DataTable dataTable = new DataTable { TableName = "MasterData" };

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
