using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using OfficeOpenXml;
using Azure.Storage.Blobs.Models;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace ITPFunctions
{
    public static class GetFilefromBlobStorage
    {
        [FunctionName("GetFilefromBlobStorage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getFileFromBlobStorage")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string UserId = req.Headers["UserId"];
            //var UserID = req.Headers.GetValues("UserId").FirstOrDefault();

            ValidateJWT auth = new ValidateJWT(req);
            if (!auth.IsValid)
            {
                return new UnauthorizedResult(); // No authentication info.
            }

            string file = req.Query["Filename"];
            string fileType = req.Query["FileType"];
            // var file = req.Form.Keys["File"];
            // ;
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(file);
            

            var str = Environment.GetEnvironmentVariable("SqlConnectionString");

            if (fileType.Equals("Trial Balance") && !string.IsNullOrEmpty(file))
            {

                byte[] blobData = RetrieveBlobDataFromAzureStorage(connectionString, containerName,file);

                if (blobData != null)
                {
                    // Validate the blob data against the SQL table schema

                    var isValid = "";
                    if (fileType.Equals("Trial Balance"))
                    {
                        isValid = ValidateBlobDataAgainstSqlTableSchema(connectionString, fileType, blobData);
                    }

                    //Creating Table    
                    DataTable fileValidationTable = new DataTable();

                    // Adding Columns    
                    DataColumn COLUMN = new DataColumn();
                    COLUMN.ColumnName = "GAAPID";
                    COLUMN.DataType = typeof(int);
                    fileValidationTable.Columns.Add(COLUMN);

                    COLUMN = new DataColumn();
                    COLUMN.ColumnName = "TaxYear";
                    COLUMN.DataType = typeof(int);
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
                    DR["GAAPID"] = DBNull.Value;
                    DR["TaxYear"] = DBNull.Value;
                    DR["FileType"] = fileType;
                    DR["FileName"] = file;
                    DR["TemplateName"] = DBNull.Value;
                    DR["SourceSystem"] = DBNull.Value;
                    DR["Modules"] = DBNull.Value;
                    DR["EntityVolume"] = DBNull.Value;

                    if (isValid == "Valid")
                    {
                        //Vald input
                        //all columns are existing, update file validation
                        DR["Status"] = "Processed";
                        DR["Errors"] = DBNull.Value;
                    }
                    else
                    {

                        //Invalid output
                        DR["Status"] = "Not Validated";
                        DR["Errors"] = isValid;

                    }

                    DR["CreatedBy"] = UserId;
                    DR["CreatedOn"] = DateTime.Now;
                    DR["ModifiedBy"] = UserId;
                    DR["ModifiedOn"] = DateTime.Now;


                    fileValidationTable.Rows.Add(DR);

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


                    using (SqlCommand cmd = new SqlCommand("[Workpaper].[sp_FileValidation_BulkSave]", new SqlConnection(str)))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(Parameter);
                        cmd.Parameters.Add(Parameter1);

                        //Executing Procedure  

                        try
                        {
                            int res = cmd.ExecuteNonQuery();
                            return new OkObjectResult(cmd.Parameters[1].Value);
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                            return new BadRequestObjectResult(ex.Message);
                        }
                    }
                }
            }
            

            return new OkObjectResult("File Retrieved with Errors");

        }

        static byte[] RetrieveBlobDataFromAzureStorage(string connectionString, string containerName, string blobName)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            using (MemoryStream stream = new MemoryStream())
            {
                blob.DownloadToStreamAsync(stream);
                return stream.ToArray();
            }
        }

        static string ValidateBlobDataAgainstSqlTableSchema(string connectionString, string tableName, byte[] blobData)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the database connection
                connection.Open();

                // Get the schema of the SQL table
                DataTable schemaTable = new DataTable();


                string sqlCommand = "";
                if (tableName == "Trial Balance")
                {
                    sqlCommand = "SELECT TOP 0 [EntityId] as 'EntityCode', [Entity Name] as 'EntityName', Left([Account],5) as 'AccountCode',SUBSTRING([Account], 9, LEN([Account]) - 9 + 1) as 'AccountDescription', [Amount] FROM [Core].[vw_TrialBalance_List]";
                }

                using (SqlCommand schemaCommand = new SqlCommand(sqlCommand, connection))
                {
                    schemaTable.Load(schemaCommand.ExecuteReader(CommandBehavior.SchemaOnly));
                }

                // Find the blob column in the schema
                string blobColumnName = ""; // Replace with the actual blob column name
                DataColumn blobColumn = schemaTable.Columns[blobColumnName];

                if (blobColumn == null)
                {
                    //Blob column does not exist in the SQL table
                    string errorMessage = "Column does not exist.";
                    return errorMessage;
                }

                // Check if the data type is correct
                if (blobColumn.DataType != typeof(byte[]))
                {
                    //Blob column has an incorrect data type
                    string errorMessage = "Invalid data type.";
                    return errorMessage;
                }

                // Check if the data length exceeds the limit
                if (blobColumn.MaxLength > 0 && blobData.Length > blobColumn.MaxLength)
                {
                    //Blob data length exceeds the character limit
                    string errorMessage = "Data exceeds the character limit.";
                    return errorMessage;
                }
            }

            return "Valid";
        }

    }

}
