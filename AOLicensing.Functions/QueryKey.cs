using AOLicensing.Functions.Extensions;
using AOLicensing.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AOLicensing.Functions
{
    public static class QueryKey
    {
        [FunctionName("QueryKey")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string requestInfo = null;
            string errorContext = null;

            try
            {
                errorContext = "inspecting request";
                var key = req.BindGet<Shared.Models.CreateKey>();

                errorContext = "searching for key";
                var config = context.GetConfig();
                var storageOptions = new StorageAccountOptions();
                config.Bind("StorageAccount", storageOptions);
                var keyStore = new KeyStore(storageOptions);

                var find = await keyStore.FindKeyAsync(key);
                return new OkObjectResult(find.data.Select(keyInfo => keyInfo.Key));
            }
            catch (Exception exc)
            {
                log.LogError(exc, $"{exc.Message} while {errorContext}: {requestInfo}");
                return new BadRequestObjectResult(exc.Message);
            }
        }
    }
}
