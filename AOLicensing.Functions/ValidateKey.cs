using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AOLicensing.Functions.Extensions;
using AOLicensing.Functions.Models;
using Microsoft.Extensions.Configuration;
using AOLicensing.Shared.Models;

namespace AOLicensing.Functions
{
    public static class ValidateKey
    {
        [FunctionName("ValidateKey")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string requestInfo = null;
            string errorContext = null;

            try
            {
                errorContext = "inspecting request";
                var key = req.BindGet<LicenseKey>();

                errorContext = "searching for key";
                var config = context.GetConfig();
                var storageOptions = new StorageAccountOptions();
                config.Bind("StorageAccount", storageOptions);
                var keyStore = new KeyStore(storageOptions);
                var result = await keyStore.ValidateKeyAsync(key);

                return new OkObjectResult(new ValidateResult()
                {
                    Success = result.success,
                    Message = result.message
                });
            }
            catch (Exception exc)
            {
                log.LogError(exc, $"{exc.Message} while {errorContext}: {requestInfo}");
                return new BadRequestObjectResult(exc.Message);
            }
        }
    }
}
