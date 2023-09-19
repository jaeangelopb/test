using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ITPFunctions
{
    public static class DownloadTemplate
    {
        [FunctionName("DownloadTemplate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorageKey");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            BlobContainerClient blobContainerClient = new BlobContainerClient(Connection, containerName);

            List<Dictionary<string, string>> fileDataList = new List<Dictionary<string, string>>();

            string templateFolderPrefix = "Templates/";

            await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync(prefix: templateFolderPrefix))
            {

                string fileNameWithExtension = blobItem.Name.Substring(templateFolderPrefix.Length);
                string fileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);

                Dictionary<string, string> fileData = new Dictionary<string, string>
                {
                    { "Template", fileName },
                    { "DownloadLink", GetDownloadLink(blobContainerClient.Uri.ToString(), blobItem.Name) }
                };
                fileDataList.Add(fileData);
            }

            return new OkObjectResult(fileDataList);
        }

        private static string GetDownloadLink(string containerUri, string fileName)
        {
            return $"{containerUri}/{fileName}";
        }
    }
}
