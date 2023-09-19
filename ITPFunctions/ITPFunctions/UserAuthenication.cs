using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ITPFunctions
{
    public static class UserAuthenication
    {
        [FunctionName("UserAuthenication")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth")]  UserCredentials userCredentials,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";


            bool authenticated = userCredentials?.User.Equals("Shankar", StringComparison.InvariantCultureIgnoreCase) ?? false;
            if (!authenticated)
            {
                return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
            }
            else
            {
                GenerateJWTToken generateJWTToken = new GenerateJWTToken();
                string token = generateJWTToken.IssuingJWT(userCredentials.User);
                return await Task.FromResult(new OkObjectResult(token)).ConfigureAwait(false);
               // return await Task.FromResult(new OkObjectResult("User is Authenticated")).ConfigureAwait(false);
            }

           // return new OkObjectResult(responseMessage);
        }
    }


    public class UserCredentials
{
    public string User
    {
        get;
        set;
    }
    public string Password
    {
        get;
        set;
    }
}

}

