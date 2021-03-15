using AOLicensing.Functions.Extensions;
using AOLicensing.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StringIdLibrary;
using System;
using System.Threading.Tasks;

namespace AOLicensing.Functions
{
    public static class CreateKey
    {
        [FunctionName("CreateKey")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string requestInfo = null;
            string errorContext = null;

            try
            {
                errorContext = "inspecting request";

                // generate key for a product
                var json = await req.ReadAsStringAsync();
                requestInfo = json;                
                var createKey = JsonConvert.DeserializeObject<Shared.Models.CreateKey>(json);
                var key = new LicenseKey()
                {
                    Email = createKey.Email,
                    Product = createKey.Product,
                    Key = new StringIdBuilder()
                        .Add(4, StringIdRanges.Upper | StringIdRanges.Numeric)
                        .Add("-")
                        .Add(4, StringIdRanges.Upper | StringIdRanges.Numeric)
                        .Add("-")
                        .Add(4, StringIdRanges.Upper | StringIdRanges.Numeric)
                        .Build()
                };

                // save to storage account
                errorContext = "saving key";
                var config = context.GetConfig();
                var keyStore = new KeyStore(config["StorageAccount:ConnectionString"], config["StorageAccount:ContainerName"]);
                await keyStore.SaveKeyAsync(key);

                // send to user            
                errorContext = "sending key to user";

                log.LogInformation(JsonConvert.SerializeObject(key));
                return new OkObjectResult(key);
            }
            catch (Exception exc)
            {                
                log.LogError(exc, $"{exc.Message} while {errorContext}: {requestInfo}");
                return new BadRequestObjectResult(exc.Message);
            }
        }
    }
}
