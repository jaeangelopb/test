using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.Linq;
using ITPFunctions.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using OfficeOpenXml;
using System.Collections;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using ExcelDataReader;

namespace ITPFunctions.CalculationManagement
{
    public static class SaveCalculationFunc
    {
        [FunctionName("SaveCalculationFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Calculation")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorageKey");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            int TrialBalanceHeaderId = 0;

            var UserID = req.Headers.GetValues("UserId").FirstOrDefault();
            dynamic body = await req.Content.ReadAsStringAsync();
            var clc = JsonConvert.DeserializeObject<IEnumerable<Calculation>>(body as string);
            Calculation Calculation= new Calculation();
            //<BlobTrialBalance> lists = new  List<BlobTrialBalance>();   


            var str = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                try
                {
                    foreach (var trial in clc)
                    {
                        var period = trial.Period;
                        var taxyear = trial.Period.Substring(3);
                        var TrialBalanceFileName = trial.TrialBalanceFileName;
                        var entity = trial.EntityId;

                        if (TrialBalanceFileName != null)
                        {
                            List<BlobTrialBalance> lists = RetrieveBlobDataFromAzureStorage(connectionString, containerName, trial.TrialBalanceFileName);
                            //Creating Table    
                            DataTable BlobTrialBalance = new DataTable();

                            // Adding Columns
                            DataColumn TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "TaxYear";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "Period";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "EntityCode";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "EntityName";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "AccountCode";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "AccountName";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "Amount";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "FileName";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "Active";
                            TBCOLUMN.DataType = typeof(Boolean);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "Delete";
                            TBCOLUMN.DataType = typeof(Boolean);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "CreatedBy";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "CreatedOn";
                            TBCOLUMN.DataType = typeof(DateTime);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "ModifiedBy";
                            TBCOLUMN.DataType = typeof(string);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            TBCOLUMN = new DataColumn();
                            TBCOLUMN.ColumnName = "ModifiedOn";
                            TBCOLUMN.DataType = typeof(DateTime);
                            BlobTrialBalance.Columns.Add(TBCOLUMN);

                            //  DateTime timestamp = DateTime.Now;
                            DateTime dt = DateTime.Now;

                            foreach (BlobTrialBalance rowItem in lists)
                            {
                                DataRow DR = BlobTrialBalance.NewRow();
                                DR["Period"] = period;
                                DR["TaxYear"] = taxyear;
                                DR["EntityCode"] = rowItem.EntityCode;
                                DR["EntityName"] = rowItem.EntityName;
                                DR["AccountCode"] = rowItem.AccountCode;
                                DR["AccountName"] = rowItem.AccountDescription;
                                DR["Amount"] = rowItem.Amount == null ? 0 : (object)rowItem.Amount;
                                DR["FileName"] = trial.TrialBalanceFileName;
                                DR["Active"] = true;
                                DR["Delete"] = false;
                                DR["CreatedBy"] = UserID;
                                DR["CreatedOn"] = dt;
                                DR["ModifiedBy"] = UserID;
                                DR["ModifiedOn"] = dt;
                                BlobTrialBalance.Rows.Add(DR);
                            }

                            //Parameter declaration    
                            SqlParameter tbParameter = new SqlParameter();
                            tbParameter.ParameterName = "@tbl_TrialBalanceLinking_BulkSave";
                            tbParameter.SqlDbType = SqlDbType.Structured;
                            tbParameter.Direction = ParameterDirection.Input;
                            tbParameter.Value = BlobTrialBalance;

                            SqlParameter EntityParam = new SqlParameter();
                            EntityParam.SqlDbType = SqlDbType.Int;
                            EntityParam.ParameterName = "@Entity";
                            EntityParam.Direction = ParameterDirection.Input;
                            EntityParam.Value = entity;

                            SqlParameter tbParameter1 = new SqlParameter();
                            tbParameter1.ParameterName = "@ReturnStatus";
                            tbParameter1.Direction = ParameterDirection.ReturnValue;

                            SqlParameter TBHeaderID = new SqlParameter();
                            TBHeaderID.ParameterName = "@TrialBalanceHeaderId";
                            TBHeaderID.SqlDbType = SqlDbType.Int;
                            TBHeaderID.Direction = ParameterDirection.Output;

                            using (SqlCommand cmd = new SqlCommand("[Core].[sp_TrialBalanceLinking_BulkSave]", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(EntityParam);
                                cmd.Parameters.Add(tbParameter);
                                cmd.Parameters.Add(tbParameter1);
                                cmd.Parameters.Add(TBHeaderID);

                                try
                                {
                                    int res = cmd.ExecuteNonQuery();
                                    TrialBalanceHeaderId = Convert.ToInt32(cmd.Parameters["@TrialBalanceHeaderId"].Value);
                                }
                                catch (Exception ex)
                                {
                                    return new BadRequestObjectResult(ex.Message);
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                    return new BadRequestObjectResult(ex.Message);
                }



                //Creating Table    
                DataTable calculation = new DataTable();

                // Adding Columns
                DataColumn COLUMN = new DataColumn();
                COLUMN.ColumnName = "CalculationId";
                COLUMN.DataType = typeof(int);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityId";
                COLUMN.DataType = typeof(int);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Period";
                COLUMN.DataType = typeof(string);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "EntityVolume";
                COLUMN.DataType = typeof(string);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Status";
                COLUMN.DataType = typeof(string);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "IngestionType";
                COLUMN.DataType = typeof(string);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "Process";
                COLUMN.DataType = typeof(string);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TrialBalanceFileName";
                COLUMN.DataType = typeof(string);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "TrialBalanceHeaderId";
                COLUMN.DataType = typeof(int);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedBy";
                COLUMN.DataType = typeof(string);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "CreatedOn";
                COLUMN.DataType = typeof(DateTime);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedBy";
                COLUMN.DataType = typeof(string);
                calculation.Columns.Add(COLUMN);

                COLUMN = new DataColumn();
                COLUMN.ColumnName = "ModifiedOn";
                COLUMN.DataType = typeof(DateTime);
                calculation.Columns.Add(COLUMN);

                DateTime timestamp = DateTime.Now;

                foreach (Calculation rowItem in clc)
                {
                    DataRow DR = calculation.NewRow();
                    DR["CalculationId"] = rowItem.CalculationId == null ? DBNull.Value : (object) rowItem.CalculationId;
                    DR["EntityId"] = rowItem.EntityId;
                    DR["Period"] = rowItem.Period;                 
                    DR["EntityVolume"] = rowItem.EntityVolume;
                    DR["Status"] = rowItem.Status;
                    DR["IngestionType"] = rowItem.IngestionType;
                    DR["Process"] = rowItem.Process;
                    DR["TrialBalanceFileName"] = rowItem.TrialBalanceFileName == null ? DBNull.Value : (object) rowItem.TrialBalanceFileName ;
                    DR["TrialBalanceHeaderId"] = TrialBalanceHeaderId == 0 ? DBNull.Value : (object)TrialBalanceHeaderId;
                    DR["CreatedBy"] = UserID;
                    DR["CreatedOn"] = timestamp;
                    DR["ModifiedBy"] = UserID;
                    DR["ModifiedOn"] = timestamp;

                    calculation.Rows.Add(DR);
                }
                //Parameter declaration    
                SqlParameter Parameter = new SqlParameter();
                Parameter.ParameterName = "@tbl_Calculation";
                Parameter.SqlDbType = SqlDbType.Structured;
                Parameter.Direction = ParameterDirection.Input;
                Parameter.Value = calculation;

                SqlParameter Parameter1 = new SqlParameter();
                Parameter1.ParameterName = "@ReturnStatus";
                Parameter1.Direction = ParameterDirection.ReturnValue;

                using (SqlCommand cmd = new SqlCommand("[Core].[sp_Calculation_BulkSave]", conn))
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
                    catch (Exception ex)
                    {
                        return new BadRequestObjectResult(ex.Message);
                    }
                }
            }
        }

        private static List<BlobTrialBalance> RetrieveBlobDataFromAzureStorage(string connectionString, string containerName, string blobName)
        {
            List<BlobTrialBalance> list = new List<BlobTrialBalance>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference("Trial Balance/" + blobName);
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
                                var myData = new BlobTrialBalance();

                                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                                {
                                    string columnHeader = columnHeaders[col - 1]?.Replace(" ", "");
                                    string cellValue = worksheet.Cells[row, col].Value?.ToString();

                                    var property = typeof(BlobTrialBalance).GetProperty(columnHeader);
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
                            var myData = new BlobTrialBalance();

                            foreach (DataColumn column in dataTable.Columns)
                            {
                                string columnHeader = column.ColumnName?.Replace(" ", ""); ;
                                string cellValue = row[column]?.ToString();

                                var property = typeof(BlobTrialBalance).GetProperty(columnHeader);
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
                                var myData = new BlobTrialBalance();

                                foreach (var columnHeader in columnHeaders)
                                {
                                    string cellValue = csvReader.GetField(columnHeader);

                                    var property = typeof(BlobTrialBalance).GetProperty(columnHeader.Replace(" ", ""));
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
