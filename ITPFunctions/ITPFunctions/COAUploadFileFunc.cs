using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;
using OfficeOpenXml;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Serialization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Collections;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using ExcelDataReader;

namespace ITPFunctions
{
    public static class COAUploadFileFunc
    {
        [FunctionName("COAUploadFileFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "COAUploadFile")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var Version = req.Headers["Version"];
            var UserID = req.Headers["UserID"];
            var Jurisdiction = req.Headers["Jurisdiction"];
            var Source = req.Headers["Source"];

            DateTime dta = DateTime.Now;

            string Jsonresults;
            string responseMessage = "";
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorageKey");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            Stream myBlob = new MemoryStream();
            var file = req.Form.Files["File"];

            myBlob = file.OpenReadStream();

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Delete the existing blob if it exists
            BlobClient existingBlobClient = containerClient.GetBlobClient("Account/" + file.FileName);
            existingBlobClient.DeleteIfExists();

            var blobClient = new BlobContainerClient(connectionString, containerName);
            var COAblob = blobClient.GetBlobClient("Account/" + file.FileName);
            await COAblob.UploadAsync(myBlob);


            BlobClient blobClients = containerClient.GetBlobClient(file.FileName);
            List<COAUploadFile> COA = new List<COAUploadFile>();

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                try
                {
                    COA = RetrieveBlobDataFromAzureStorage(connectionString, containerName, file.FileName);
                }
                catch (Exception ex)
                {

                    return new BadRequestObjectResult(ex.Message);
                }

                if (COA.Count > 0)
                {

                    var isValid = "";

                    DataTable ChartOfAccount = new DataTable();
                    //Creating Table    
                    DataTable fileValidationTable = new DataTable();

                    // Adding Columns    
                    DataColumn COLUMN = new DataColumn();
                    COLUMN.ColumnName = "GAAPID";
                    COLUMN.DataType = typeof(int);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "TaxYear";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "Period";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "FileType";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "FileName";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "TemplateName";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "SourceSystem";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "Modules";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "EntityVolume";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "Status";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "Errors";
                    COLUMN.DataType = typeof(string);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "CreatedBy";
                    COLUMN.DataType = typeof(string);

                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "CreatedOn";
                    COLUMN.DataType = typeof(DateTime);

                    fileValidationTable.Columns.Add(COLUMN);


                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "ModifiedBy";
                    COLUMN.DataType = typeof(string);

                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "ModifiedOn";
                    COLUMN.DataType = typeof(DateTime);



                    fileValidationTable.Columns.Add(COLUMN);
                    try
                    {
                        DataRow DR = fileValidationTable.NewRow();

                        DR["GAAPID"] = 1;
                        DR["TaxYear"] = null;
                        DR["Period"] = null;

                        DR["FileType"] = null;
                        DR["FileName"] = file.FileName;
                        DR["TemplateName"] = null;
                        DR["SourceSystem"] = "SourceSystem";
                        DR["Modules"] = "Tax Provision";
                        DR["EntityVolume"] = null;

                        if (COA.Count > 0)
                        {
                            //Vald input
                            //all columns are existing, update file validation
                            DR["Status"] = "Processed";
                            DR["Errors"] = "0";
                        }
                        else
                        {

                            //Invalid output
                            DR["Status"] = "Not Validated";
                            DR["Errors"] = isValid;

                        }

                        DR["CreatedBy"] = UserID;
                        DR["CreatedOn"] = DateTime.Now;
                        DR["ModifiedBy"] = UserID;
                        DR["ModifiedOn"] = DateTime.Now;


                        fileValidationTable.Rows.Add(DR);
                    }
                    catch (Exception ex)
                    {
                        return new BadRequestObjectResult(ex.Message);

                    }

                    //Parameter declaration    
                    SqlParameter Parameter = new SqlParameter();
                    Parameter.ParameterName = "@tbl_FileValidation";
                    Parameter.SqlDbType = SqlDbType.Structured;
                    Parameter.Direction = ParameterDirection.Input;
                    Parameter.Value = fileValidationTable;


                    SqlParameter Parameter1 = new SqlParameter();
                    Parameter1.ParameterName = "@ReturnStatus";
                    Parameter1.Direction = ParameterDirection.ReturnValue;

                    // Open the connection and execute the command
                    //command.Connection.Open();
                    //int rowsAffected = command.ExecuteNonQuery();

                    using (SqlCommand cmd = new SqlCommand("[Core].[sp_FileValidation_BulkSave]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(Parameter);
                        cmd.Parameters.Add(Parameter1);

                        //Executing Procedure  

                        try
                        {
                            int res = cmd.ExecuteNonQuery();
                            //return new OkObjectResult(cmd.Parameters[1].Value);
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                            return new BadRequestObjectResult(ex.Message);
                        }
                    }




                    // Adding Columns    
                    DataColumn COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "Account";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "AccountName";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "Risk";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "AccountingTreatment";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "TaxTreatment";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "AccountType";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "ITRMappingCategory";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "FormCDisclosure";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "AdjustmentCategory";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "Jurisdiction";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "VersionName";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);


                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "Source";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "CreatedBy";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "CreatedOn";
                    COACOLUMN.DataType = typeof(DateTime);
                    ChartOfAccount.Columns.Add(COACOLUMN);


                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "ModifiedBy";
                    COACOLUMN.DataType = typeof(string);
                    ChartOfAccount.Columns.Add(COACOLUMN);

                    COACOLUMN = new DataColumn();
                    COACOLUMN.ColumnName = "ModifiedOn";
                    COACOLUMN.DataType = typeof(DateTime);
                    ChartOfAccount.Columns.Add(COACOLUMN);


                    foreach (COAUploadFile rowItem in COA)
                    {
                        try
                        {
                            DataRow DR = ChartOfAccount.NewRow();
                            DR["Account"] = rowItem.Account;
                            DR["AccountName"] = rowItem.AccountName;
                            DR["Risk"] = rowItem.Risk;
                            DR["AccountingTreatment"] = rowItem.AccountingTreatment;
                            DR["TaxTreatment"] = rowItem.TaxTreatment;
                            DR["AccountType"] = rowItem.AccountType == null ? DBNull.Value : (object)rowItem.AccountType;
                            DR["ITRMappingCategory"] = rowItem.TaxSensitivity == null ? DBNull.Value : (object)rowItem.TaxSensitivity;
                            DR["FormCDisclosure"] = rowItem.FormCDisclosure == null ? DBNull.Value : (object)rowItem.FormCDisclosure;
                            DR["AdjustmentCategory"] = rowItem.AdjustmentCategory == null ? DBNull.Value : (object)rowItem.AdjustmentCategory;
                            DR["Jurisdiction"] = Jurisdiction;
                            DR["VersionName"] = Version;
                            DR["Source"] = Source;
                            DR["CreatedBy"] = UserID;
                            DR["CreatedOn"] = dta;
                            DR["ModifiedBy"] = UserID;
                            DR["ModifiedOn"] = dta;
                            ChartOfAccount.Rows.Add(DR);
                        }
                        catch (Exception ex)
                        {
                            return new BadRequestObjectResult(ex.Message);

                        }



                    }
                    //Parameter declaration    
                    SqlParameter COAParameter = new SqlParameter();
                    COAParameter.ParameterName = "@tbl_ChartOfAccount_Import";
                    COAParameter.SqlDbType = SqlDbType.Structured;
                    COAParameter.Direction = ParameterDirection.Input;
                    COAParameter.Value = ChartOfAccount;

                    SqlParameter COAParameter1 = new SqlParameter();
                    COAParameter1.ParameterName = "@ReturnStatus";
                    COAParameter1.Direction = ParameterDirection.ReturnValue;

                    using (SqlCommand cmd = new SqlCommand("[Core].[sp_ChartOfAccount_Import]", conn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(COAParameter);
                        cmd.Parameters.Add(COAParameter1);
                        //Executing Procedure  
                        try
                        {
                            int res = cmd.ExecuteNonQuery();


                            string queryop = "";


                            using (SqlDataReader reader = cmd.ExecuteReader())

                            {

                                queryop = sqlDatoToJson(reader);

                            }


                            responseMessage = (queryop);
                            if (responseMessage == string.Empty)
                            {


                                return new OkObjectResult(cmd.Parameters[1].Value);
                            }
                            else
                            {

                                return new BadRequestObjectResult(responseMessage);
                            }

                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                            return new BadRequestObjectResult(ex.Message);
                        }

                    }

                }



            }
            return new OkObjectResult("file uploaded successfully");
        }
        static String sqlDatoToJson(SqlDataReader dataReader)

        // transform the returned data to JSON

        {

            var result = "";
            DataTable dataTable = new DataTable { TableName = "ConsolTEASummaryFooter" };

            dataTable.Load(dataReader);



            string JSONString = string.Empty;


            JSONString = JsonConvert.SerializeObject(dataTable);

            if (dataTable.Rows.Count != 0)
            {
                result = "[{\"validationType\": \"Data\", \"validation\":" + JSONString + "}]";
            }

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
            return result;

        }


        private static List<COAUploadFile> RetrieveBlobDataFromAzureStorage(string connectionString, string containerName, string blobName)
        {
            List<COAUploadFile> list = new List<COAUploadFile>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference("Account/" + blobName);
            var Jsonresults = "";
            var ErrorList = "";

            ArrayList ErrorArray = new ArrayList();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                blob.DownloadToStreamAsync(memoryStream).GetAwaiter().GetResult();
                memoryStream.Position = 0;

                var fileExtension = Path.GetExtension(blobName).ToLower();

                if (fileExtension == ".xlsx")
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        var headerRow = worksheet.Cells
                            .FirstOrDefault(cell => cell.Start.Row == 1)
                            ?.Start
                            ?.Row;

                        if (headerRow != null)
                        {
                            var columnHeaders = worksheet.Cells[headerRow.Value, 1, headerRow.Value, worksheet.Dimension.End.Column]
                                .Select(cell => cell.Value?.ToString())
                                .ToList();

                            for (int row = headerRow.Value + 1; row <= worksheet.Dimension.End.Row; row++)
                            {
                                var myData = new COAUploadFile();

                                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                                {
                                    string columnHeader = columnHeaders[col - 1]?.Replace(" ", "");
                                    string cellValue = worksheet.Cells[row, col].Value?.ToString();

                                    var property = typeof(COAUploadFile).GetProperty(columnHeader);
                                    if (property != null)
                                    {
                                        property.SetValue(myData, cellValue);
                                    }
                                    else
                                    {
                                        ErrorArray.Add("Invalid column name: " + columnHeader);
                                    }
                                }

                                list.Add(myData);
                            }
                        }
                    }
                }
                else if (fileExtension == ".xls")
                {
                    using (var reader = ExcelReaderFactory.CreateBinaryReader(memoryStream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });

                        var dataTable = result.Tables[0];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var myData = new COAUploadFile();

                            foreach (DataColumn column in dataTable.Columns)
                            {
                                string columnHeader = column.ColumnName?.Replace(" ", ""); ;
                                string cellValue = row[column]?.ToString();

                                var property = typeof(COAUploadFile).GetProperty(columnHeader);
                                if (property != null)
                                {
                                    property.SetValue(myData, cellValue);
                                }
                                else
                                {
                                    ErrorArray.Add("Invalid column name: " + columnHeader);
                                }
                            }

                            list.Add(myData);
                        }
                    }

                    ErrorList = JsonConvert.SerializeObject(ErrorArray);
                    Jsonresults = "[{\"validationType\": \"Header\", \"validation\":" + ErrorList + "}]";

                    if (ErrorArray.Count > 0)
                    {
                        throw new ArgumentException(Jsonresults);
                    }


                }
                else if (fileExtension == ".csv")
                {
                    using (var reader = new StreamReader(memoryStream))
                    {
                        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HasHeaderRecord = true
                        };

                        using (var csvReader = new CsvReader(reader, csvConfig))
                        {
                            csvReader.Read();
                            csvReader.ReadHeader();

                            var columnHeaders = csvReader.HeaderRecord.ToList();

                            while (csvReader.Read())
                            {
                                var myData = new COAUploadFile();

                                foreach (var columnHeader in columnHeaders)
                                {
                                    string cellValue = csvReader.GetField(columnHeader);

                                    var property = typeof(COAUploadFile).GetProperty(columnHeader);
                                    if (property != null)
                                    {
                                        property.SetValue(myData, cellValue);
                                    }
                                    else
                                    {
                                        ErrorArray.Add("Invalid column name: " + columnHeader);
                                    }
                                }

                                list.Add(myData);
                            }
                        }
                    }
                }

                ErrorList = JsonConvert.SerializeObject(ErrorArray);
                Jsonresults = "[{\"validationType\": \"Header\", \"validation\":" + ErrorList + "}]";

                if (ErrorArray.Count > 0)
                {
                    throw new ArgumentException(Jsonresults);
                }
            }

            return list;
        }




    }
}
