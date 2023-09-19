using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace ITPFunctions
{
    public static class DownloadFile
    {
        [FunctionName("DownloadFile")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DownloadTemplates")] HttpRequest req,
        ILogger log)
        {

            string fileDownload = req.Query["Template"];
            string contentType = string.Empty;

            try
            {
                string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorageKey");
                string containerName = Environment.GetEnvironmentVariable("ContainerName");

                BlobContainerClient blobContainerClient = new BlobContainerClient(Connection, containerName);

                var blobs = blobContainerClient.GetBlobs(prefix: "Templates/");
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        foreach (var blob in blobs)
                        {
                            var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                            string blobFileName = blob.Name.Substring(0, blob.Name.LastIndexOf(".")).Replace("Templates/", "");
                            
                            if (blobFileName == fileDownload)
                            {
                                string ext = Path.GetExtension(blob.Name);
                                switch (ext)
                                {
                                    case ".xlsx":
                                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                        break;
                                    case ".xls":
                                        contentType = "application/vnd.ms-excel";
                                        break;
                                    case ".csv":
                                        contentType = "text/csv";
                                        break;
                                    default:
                                        return new BadRequestObjectResult("Invalid template!");
                                }

                                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Connection);
                                CloudBlobClient blobItem = storageAccount.CreateCloudBlobClient();
                                CloudBlobContainer container = blobItem.GetContainerReference(containerName);
                                CloudBlockBlob blobFile = container.GetBlockBlobReference("Templates/" + blobFileName + ext);
                                var blobStream = await blobFile.OpenReadAsync().ConfigureAwait(false);
                                //return new FileStreamResult(blobStream, "application/octet-stream");
                                return new FileStreamResult(blobStream, contentType);
                            }
                        }

                        // File not found
                        return new OkObjectResult("Template not found.");
                    }
                    catch (Exception ex)
                    {
                        return new BadRequestObjectResult(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}