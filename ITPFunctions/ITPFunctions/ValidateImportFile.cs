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
using Azure.Storage.Blobs.Models;
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
    public static class ValidateImportFile
    {
        [FunctionName("ValidateImportFile")]

        public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ValidateImport")] HttpRequest req,
          ILogger log)

        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var UserID = req.Headers["UserID"];
            var Jsonresults = "";
            var ErrorStr = "";
            var FinalErrorStr = "";

            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorageKey");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            ArrayList FinalErrorArray = new ArrayList();
            ArrayList ValTypeArray = new ArrayList();
            ArrayList FileNameArray = new ArrayList();

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Define the source and destination folder names
            string sourceFolderName = "Temp Folder";
            string destinationFolderName = "Validated Files with Error";

            // Get the list of existing blobs in the source folder
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: sourceFolderName + "/"))
            {
                // Extract the blob name from the full path
                string blobName = blobItem.Name;

                // Move the existing blob to the destination folder
                BlobClient sourceBlobClient = containerClient.GetBlobClient(blobName);
                string destinationBlobName = blobName.Replace(sourceFolderName, destinationFolderName);
                BlobClient destinationBlobClient = containerClient.GetBlobClient(destinationBlobName);

                // Perform the move operation
                await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
                await sourceBlobClient.DeleteAsync();
            }

            // Upload multiple files to the source folder
            try
            {
                foreach (var file in req.Form.Files)
                {
                    var str = Environment.GetEnvironmentVariable("SqlConnectionString");
                    string fileName = file.FileName;
                    string templateName = file.Name;
                    string validationStatus = string.Empty;
                    string validationError = string.Empty;
                    string newDestinationFolderName = string.Empty;
                    string fileNameOnly = file.FileName.Substring(0, file.FileName.LastIndexOf("."));
                    string timeStamp = "_" + $"{DateTime.Now:yyyy-MM-dd_HH:mm:ss}";
                    string ext = Path.GetExtension(file.FileName);
                    string finalFileName = fileNameOnly + timeStamp + ext;
                    int entityVolume = 0;

                    Stream myBlob = new MemoryStream();
                    myBlob = file.OpenReadStream();

                    // Upload the new file to the source folder
                    var blobClient = new BlobContainerClient(connectionString, containerName);
                    var tempBlob = blobClient.GetBlobClient(sourceFolderName + "/" + file.FileName);
                    await tempBlob.UploadAsync(myBlob);

                    BlobClient blobClients = containerClient.GetBlobClient(file.FileName);

                    // Confirm that templates exist in templates folder - update when necessary
                    if (templateName == "Trial Balance" | templateName == "Template 2" | templateName == "Template 3")
                    {
                        //Create template table(s)
                        DataTable TBdata = new DataTable();
                        // Adding Columns to TB Table 
                        DataColumn tbCOLUMN = new DataColumn();
                        tbCOLUMN.ColumnName = "EntityCode";
                        tbCOLUMN.DataType = typeof(string);
                        TBdata.Columns.Add(tbCOLUMN);

                        tbCOLUMN = new DataColumn();
                        tbCOLUMN.ColumnName = "EntityName";
                        tbCOLUMN.DataType = typeof(string);
                        TBdata.Columns.Add(tbCOLUMN);

                        tbCOLUMN = new DataColumn();
                        tbCOLUMN.ColumnName = "AccountCode";
                        tbCOLUMN.DataType = typeof(string);
                        TBdata.Columns.Add(tbCOLUMN);

                        tbCOLUMN = new DataColumn();
                        tbCOLUMN.ColumnName = "AccountDescription";
                        tbCOLUMN.DataType = typeof(string);
                        TBdata.Columns.Add(tbCOLUMN);

                        tbCOLUMN = new DataColumn();
                        tbCOLUMN.ColumnName = "Amount";
                        tbCOLUMN.DataType = typeof(string);
                        TBdata.Columns.Add(tbCOLUMN);

                        int res;
                        string responseMessage = "";
                        string procName = string.Empty;
                        string parameterName = string.Empty;

                        using (SqlConnection conn = new SqlConnection(str))
                        {
                            conn.Open();

                            (ArrayList TB, List<TBUploadFile> XL) = RetrieveBlobDataFromAzureStorage(connectionString, containerName, sourceFolderName, file.FileName);

                            if (TB.Count > 0)
                            {
                                var ErrorList = "";
                                ErrorList = JsonConvert.SerializeObject(TB);
                                validationStatus = "Not Processed";
                                validationError = "Invalid column(s)";
                                newDestinationFolderName = destinationFolderName;
                                ErrorStr = "{\"file\": \"" + fileName + "\", \"validationType\": \"Header\", \"validation\":" + ErrorList + "}";
                                if (FinalErrorStr == "")
                                {
                                    FinalErrorStr = ErrorStr; //Save error
                                }
                                else
                                {
                                    FinalErrorStr = FinalErrorStr + "," + ErrorStr; //Combine errors
                                }
                            }

                            else if (TB.Count == 0)
                            {
                                if (XL.Count == 0)
                                {
                                    validationStatus = "Not Processed";
                                    validationError = "Uploaded file has no data";
                                    newDestinationFolderName = destinationFolderName;
                                    ErrorStr = "{\"file\": \"" + fileName + "\", \"validationType\": \"Data\", \"validation\": \"Uploaded file has no data\"}";
                                    if (FinalErrorStr == "")
                                    {
                                        FinalErrorStr = ErrorStr; //Save error
                                    }
                                    else
                                    {
                                        FinalErrorStr = FinalErrorStr + "," + ErrorStr; //Combine errors
                                    }
                                }
                                else
                                {
                                    entityVolume = XL.Count;

                                    // Trial Balance template
                                    if (templateName.Equals("Trial Balance"))
                                    {
                                        procName = "[Core].[sp_TrialBalance_Validation]";
                                        parameterName = "@tbl_TrialBalance_Validation";

                                        foreach (TBUploadFile rowItem in XL)
                                        {
                                            try
                                            {
                                                DataRow row = TBdata.NewRow();
                                                row["EntityCode"] = rowItem.EntityCode;
                                                row["EntityName"] = rowItem.EntityName;
                                                row["AccountCode"] = rowItem.AccountCode;
                                                row["AccountDescription"] = rowItem.AccountDescription;
                                                row["Amount"] = rowItem.Amount;
                                                TBdata.Rows.Add(row);
                                            }
                                            catch (Exception ex)
                                            {
                                                return new BadRequestObjectResult(ex.Message);

                                            }
                                        }
                                    }

                                    // Update for new template
                                    else if (templateName.Equals("Template 2"))
                                    {
                                        procName = "[Core].[sp_TrialBalance_Validation]";
                                        parameterName = "@tbl_TrialBalance_Validation";

                                        foreach (TBUploadFile rowItem in XL)
                                        {
                                            try
                                            {
                                                DataRow row = TBdata.NewRow();
                                                row["EntityCode"] = rowItem.EntityCode;
                                                row["EntityName"] = rowItem.EntityName;
                                                row["AccountCode"] = rowItem.AccountCode;
                                                row["AccountDescription"] = rowItem.AccountDescription;
                                                row["Amount"] = rowItem.Amount;
                                                TBdata.Rows.Add(row);
                                            }
                                            catch (Exception ex)
                                            {
                                                return new BadRequestObjectResult(ex.Message);

                                            }
                                        }
                                    }

                                    //Update for new template
                                    else if (templateName.Equals("Template 3"))
                                    {
                                        procName = "[Core].[sp_TrialBalance_Validation]";
                                        parameterName = "@tbl_TrialBalance_Validation";

                                        foreach (TBUploadFile rowItem in XL)
                                        {
                                            try
                                            {
                                                DataRow row = TBdata.NewRow();
                                                row["EntityCode"] = rowItem.EntityCode;
                                                row["EntityName"] = rowItem.EntityName;
                                                row["AccountCode"] = rowItem.AccountCode;
                                                row["AccountDescription"] = rowItem.AccountDescription;
                                                row["Amount"] = rowItem.Amount;
                                                TBdata.Rows.Add(row);
                                            }
                                            catch (Exception ex)
                                            {
                                                return new BadRequestObjectResult(ex.Message);
                                            }
                                        }
                                    }

                                    // Parameter declaration    
                                    SqlParameter Param = new SqlParameter();
                                    Param.ParameterName = parameterName;
                                    Param.SqlDbType = SqlDbType.Structured;
                                    Param.Direction = ParameterDirection.Input;
                                    Param.Value = TBdata;

                                    SqlParameter Parameter1 = new SqlParameter();
                                    Parameter1.ParameterName = "@ReturnStatus";
                                    Parameter1.Direction = ParameterDirection.ReturnValue;

                                    using (SqlCommand cmd = new SqlCommand(procName, conn))
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.Add(Param);
                                        cmd.Parameters.Add(Parameter1);

                                        // Executing Procedure  
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

                                        if (((System.Data.InternalDataCollectionBase)(dataset.Tables)).Count != 0)
                                        {
                                            var varList = System.Text.Json.JsonSerializer.Deserialize<IList<TrialBalanceJSON>>(responseMessage);
                                            
                                            if (varList.Count > 0)
                                            {
                                                validationStatus = "Not Processed";
                                                validationError = "Invalid Data";
                                                newDestinationFolderName = destinationFolderName;
                                                ErrorStr = "{\"file\": \"" + fileName + "\", \"validationType\": \"Data\", \"validation\":" + responseMessage + "}";
                                                if (FinalErrorStr == "")
                                                {
                                                    FinalErrorStr = ErrorStr; //Save error
                                                }
                                                else
                                                {
                                                    FinalErrorStr = FinalErrorStr + "," + ErrorStr; //Combine errors
                                                }
                                            }
                                            else
                                            {
                                                validationStatus = "Processed";
                                                validationError = "None";
                                                newDestinationFolderName = templateName; //final destination folder name should be the same with template name
                                                ErrorStr = "{\"file\": \"" + fileName + "\", \"validationType\": \"Header | Data\", \"validation\": \"Successful!\"}";
                                                if (FinalErrorStr == "")
                                                {
                                                    FinalErrorStr = ErrorStr; //Save error
                                                }
                                                else
                                                {
                                                    FinalErrorStr = FinalErrorStr + "," + ErrorStr; //Combine errors
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else //Template does not exist in storage
                    {
                        validationStatus = "Not Processed";
                        validationError = "Invalid template";
                        newDestinationFolderName = destinationFolderName;
                        ErrorStr = "{\"file\": \"" + fileName + "\", \"validationType\": \"Template\", \"validation\": \"Invalid template\"}";
                        if (FinalErrorStr == "")
                        {
                            FinalErrorStr = ErrorStr; //Save error
                        }
                        else
                        {
                            FinalErrorStr = FinalErrorStr + "," + ErrorStr; //Combine errors
                        }
                    }

                    //move file(s) to final folder
                    BlobClient sourceBlobClient = containerClient.GetBlobClient(sourceFolderName + "/" + file.FileName);
                    BlobClient destinationBlobClient = containerClient.GetBlobClient(newDestinationFolderName + "/" + finalFileName);
                    //BlobClient destinationBlobClient = containerClient.GetBlobClient(newDestinationFolderName + "/" + file.FileName);
                    

                    // Perform the move operation
                    await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
                    await sourceBlobClient.DeleteAsync();

                    //Save validation details to File Validation table

                    // Create File Validation Table
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
                    DataRow DR = fileValidationTable.NewRow();

                    DR["GAAPID"] = 1;
                    DR["TaxYear"] = null;
                    DR["Period"] = null;
                    DR["FileType"] = templateName;
                    DR["FileName"] = finalFileName;
                    //DR["FileName"] = file.FileName;
                    DR["TemplateName"] = file.Name;
                    DR["SourceSystem"] = "SourceSystem";
                    DR["Modules"] = "Tax Provision";
                    //DR["EntityVolume"] = entityVolume;
                    DR["EntityVolume"] = "Single";
                    DR["Status"] = validationStatus;
                    DR["Errors"] = validationError;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = DateTime.Now;
                    fileValidationTable.Rows.Add(DR);

                    using (SqlConnection conn = new SqlConnection(str))
                    {
                        conn.Open();

                        //Parameter declaration    
                        SqlParameter Parameter = new SqlParameter();
                        Parameter.ParameterName = "@tbl_FileValidation";
                        Parameter.SqlDbType = SqlDbType.Structured;
                        Parameter.Direction = ParameterDirection.Input;
                        Parameter.Value = fileValidationTable;

                        SqlParameter Parameter2 = new SqlParameter();
                        Parameter2.ParameterName = "@ReturnStatus";
                        Parameter2.Direction = ParameterDirection.ReturnValue;

                        using (SqlCommand cmd = new SqlCommand("[Core].[sp_FileValidation_BulkSave]", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(Parameter);
                            cmd.Parameters.Add(Parameter2);

                            //Executing Procedure  

                            try
                            {
                                int res1 = cmd.ExecuteNonQuery();
                                //return new OkObjectResult(cmd.Parameters[1].Value);
                            }
                            catch (System.Data.SqlClient.SqlException ex)
                            {
                                return new BadRequestObjectResult(ex.Message);
                            }
                        }
                    }

                }

                //Return Error Details to FrontEnd
                Jsonresults = "[" + FinalErrorStr + "]";
                return new OkObjectResult(Jsonresults);
            }

            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        static (ArrayList, List<TBUploadFile>) RetrieveBlobDataFromAzureStorage(string connectionString, string containerName, string sourceFolderName, string blobName)

        {
            List<TBUploadFile> list = new List<TBUploadFile>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(sourceFolderName + "/" + blobName);
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
                        // Assume the Excel file has a single worksheet, and you want to read data from the first worksheet
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        // Find the header row
                        var headerRow = worksheet.Cells
                            .FirstOrDefault(cell => cell.Start.Row == 1)
                            ?.Start
                            ?.Row;

                        // Get the column headers
                        if (headerRow != null)
                        {
                            var columnHeaders = worksheet.Cells[headerRow.Value, 1, headerRow.Value, worksheet.Dimension.End.Column]
                            .Select(cell => cell.Value?.ToString())
                            .ToList();

                            // Iterate over the rows, starting from the second row (assuming the first row is the header row)
                            for (int row = headerRow.Value + 1; row <= worksheet.Dimension.End.Row; row++)
                            {
                                // Create an instance of your class
                                var myData = new TBUploadFile();

                                // Read data dynamically based on column mappings
                                for (int col = 1; col <= columnHeaders.Count; col++)
                                {
                                    string columnHeader = columnHeaders[col - 1].Replace(" ", "");
                                    string cellValue = worksheet.Cells[row, col].Value?.ToString();

                                    var property = typeof(TBUploadFile).GetProperty(columnHeader);
                                    if (property != null)
                                    {
                                        property.SetValue(myData, cellValue);
                                    }
                                    else
                                    {
                                        string errItem = "Invalid column name: " + columnHeader;
                                        bool alreadyExist = ErrorArray.Contains(errItem);
                                        if (!alreadyExist) { ErrorArray.Add(errItem); }
                                    }
                                }
                                // Add the class instance to the list
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
                            var myData = new TBUploadFile();

                            foreach (DataColumn column in dataTable.Columns)
                            {
                                string columnHeader = column.ColumnName?.Replace(" ", ""); ;
                                string cellValue = row[column]?.ToString();

                                var property = typeof(TBUploadFile).GetProperty(columnHeader);
                                if (property != null)
                                {
                                    property.SetValue(myData, cellValue);
                                }
                                else
                                {
                                    string errItem = "Invalid column name: " + columnHeader;
                                    bool alreadyExist = ErrorArray.Contains(errItem);
                                    if (!alreadyExist) { ErrorArray.Add(errItem); }
                                }
                            }
                            // Add the class instance to the list
                            list.Add(myData);
                        }
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
                                var myData = new TBUploadFile();

                                foreach (var columnHeader in columnHeaders)
                                {
                                    string cellValue = csvReader.GetField(columnHeader);

                                    var property = typeof(TBUploadFile).GetProperty(columnHeader.Replace(" ", ""));
                                    if (property != null)
                                    {
                                        property.SetValue(myData, cellValue);
                                    }
                                    else
                                    {
                                        string errItem = "Invalid column name: " + columnHeader;
                                        bool alreadyExist = ErrorArray.Contains(errItem);
                                        if (!alreadyExist) { ErrorArray.Add(errItem); }
                                    }
                                }
                                // Add the class instance to the list
                                list.Add(myData);
                            }
                        }
                    }
                }
            }

            // Return errors and excel data
            return (ErrorArray, list);
        }

        // transform the returned data to JSON
        static String sqlDatoToJson(SqlDataReader dataReader)
        {
            DataTable dataTable = new DataTable { TableName = "ValidationData" };

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