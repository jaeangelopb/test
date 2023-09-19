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
using System.Collections;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using ExcelDataReader;

namespace ITPFunctions
{
    public static class TACUploadFileFunc
    {
        [FunctionName("TACUploadFileFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "TACUploadFile")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var Version = req.Headers["Version"];
            var UserID = req.Headers["UserID"];

            DateTime dta = DateTime.Now;
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorageKey");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            Stream myBlob = new MemoryStream();
            var file = req.Form.Files["File"];

            myBlob = file.OpenReadStream();
            string responseMessage = "";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Delete the existing blob if it exists
            BlobClient existingBlobClient = containerClient.GetBlobClient("Category/" + file.FileName);
            existingBlobClient.DeleteIfExists();

            var blobClient = new BlobContainerClient(connectionString, containerName);
            var COAblob = blobClient.GetBlobClient("Category/" + file.FileName);
            await COAblob.UploadAsync(myBlob);


            BlobClient blobClients = containerClient.GetBlobClient(file.FileName);
            List<TACUploadFile> TACList = new List<TACUploadFile>();

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    TACList = RetrieveBlobDataFromAzureStorage(connectionString, containerName, file.FileName);

                }
                catch (Exception ex)
                {

                return new BadRequestObjectResult(ex.Message);
                }


            if (TACList.Count > 0)
                {
                    var isValid = "";

                    DataTable TAC = new DataTable();
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

                        if (TACList.Count > 0)
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
                    DataColumn TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "Type";
                    TACCOLUMN.DataType = typeof(string);
                    TAC.Columns.Add(TACCOLUMN);

                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "Adjustments";
                    TACCOLUMN.DataType = typeof(string);
                    TAC.Columns.Add(TACCOLUMN);

                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "TNCDescription";
                    TACCOLUMN.DataType = typeof(string);
                    TAC.Columns.Add(TACCOLUMN);

                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "FormCSTICategory";
                    TACCOLUMN.DataType = typeof(string);
                    TAC.Columns.Add(TACCOLUMN);

                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "Source";
                    TACCOLUMN.DataType = typeof(string);
                    TAC.Columns.Add(TACCOLUMN);

                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "Default";
                    TACCOLUMN.DataType = typeof(bool);
                    TAC.Columns.Add(TACCOLUMN);

                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "Sorting";
                    TACCOLUMN.DataType = typeof(string);
                    TAC.Columns.Add(TACCOLUMN);


                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "CreatedBy";
                    TACCOLUMN.DataType = typeof(string);
                    TAC.Columns.Add(TACCOLUMN);

                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "CreatedOn";
                    TACCOLUMN.DataType = typeof(DateTime);
                    TAC.Columns.Add(TACCOLUMN);


                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "ModifiedBy";
                    TACCOLUMN.DataType = typeof(string);
                    TAC.Columns.Add(TACCOLUMN);

                    TACCOLUMN = new DataColumn();
                    TACCOLUMN.ColumnName = "ModifiedOn";
                    TACCOLUMN.DataType = typeof(DateTime);
                    TAC.Columns.Add(TACCOLUMN);
                    

                    foreach (TACUploadFile rowItem in TACList)
                    {
                        try
                        {
                            DataRow DR = TAC.NewRow();
                            DR["Type"] = rowItem.Type;
                            DR["Adjustments"] = rowItem.Adjustments;
                            DR["TNCDescription"] = rowItem.TaxNoteReportingCategories == null ? DBNull.Value : (object)rowItem.TaxNoteReportingCategories;
                            DR["FormCSTICategory"] = rowItem.FormCSTICategory == null ? DBNull.Value : (object)rowItem.FormCSTICategory;
                            DR["Source"] =Version;
                            DR["Default"] = false;
                            DR["Sorting"] ="1";
                            DR["CreatedBy"] = UserID;
                            DR["CreatedOn"] = dta;
                            DR["ModifiedBy"] = UserID;
                            DR["ModifiedOn"] = dta;
                            TAC.Rows.Add(DR);
                        }
                        catch (Exception ex)
                        {
                            return new BadRequestObjectResult(ex.Message);

                        }



                    }
                    //Parameter declaration    
                    SqlParameter COAParameter = new SqlParameter();
                    COAParameter.ParameterName = "@tbl_TaxAdjustmentCategory_Insert";
                    COAParameter.SqlDbType = SqlDbType.Structured;
                    COAParameter.Direction = ParameterDirection.Input;
                    COAParameter.Value = TAC;

                    SqlParameter COAParameter1 = new SqlParameter();
                    COAParameter1.ParameterName = "@ReturnStatus";
                    COAParameter1.Direction = ParameterDirection.ReturnValue;

                    using (SqlCommand cmd = new SqlCommand("[Common].[sp_TaxAdjustmentCategory_BulkSave_Import]", conn))
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
            var result ="";


            DataTable dataTable = new DataTable { TableName = "ConsolTEASummaryFooter" };

            dataTable.Load(dataReader);



            string JSONString = string.Empty;

            JSONString = JsonConvert.SerializeObject(dataTable);

            if (dataTable.Rows.Count != 0)
            {
                 result = "[{\"validationType\": \"Data\", \"validation\":" + JSONString + "}]";
            }
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

        private static List<TACUploadFile> RetrieveBlobDataFromAzureStorage(string connectionString, string containerName, string blobName)
        {
            List<TACUploadFile> list = new List<TACUploadFile>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference("Category/" + blobName);
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
                                var myData = new TACUploadFile();

                                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                                {
                                    string columnHeader = columnHeaders[col - 1]?.Replace(" ", "");
                                    string cellValue = worksheet.Cells[row, col].Value?.ToString();

                                    var property = typeof(TACUploadFile).GetProperty(columnHeader);
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
                            var myData = new TACUploadFile();

                            foreach (DataColumn column in dataTable.Columns)
                            {
                                string columnHeader = column.ColumnName;
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
                                var myData = new TACUploadFile();

                                foreach (var columnHeader in columnHeaders)
                                {
                                    string cellValue = csvReader.GetField(columnHeader);

                                    var property = typeof(TACUploadFile).GetProperty(columnHeader.Replace(" ", ""));
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
