using AOLicensing.Functions.Extensions;
using AOLicensing.Functions.Models;
using AOLicensing.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using StringIdLibrary;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using Markdig;
using System.Linq;

namespace AOLicensing.Functions
{
    public static class CreateKey
    {
        [FunctionName("CreateKey")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)] HttpRequest req,
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

                var config = context.GetConfig();

                // save to storage account
                errorContext = "saving key";
                var storageOptions = new StorageAccountOptions();
                config.Bind("StorageAccount", storageOptions);
                var keyStore = new KeyStore(storageOptions);
                await keyStore.SaveKeyAsync(key);

                // send to user            
                errorContext = "sending key to user";
                var sendGridOptions = new SendGridOptions();
                config.GetSection("SendGrid").Bind(sendGridOptions);
                await SendConfirmationEmailAsync(key, sendGridOptions);

                log.LogInformation(JsonConvert.SerializeObject(key));
                return new OkObjectResult(key);
            }
            catch (Exception exc)
            {                
                log.LogError(exc, $"{exc.Message} while {errorContext}: {requestInfo}");
                return new BadRequestObjectResult(exc.Message);
            }
        }

        private async static Task SendConfirmationEmailAsync(LicenseKey key, SendGridOptions options)
        {
            var client = new SendGridClient(options.ApiKey);
            var from = new EmailAddress(options.SenderEmail, options.SenderName);
            var to = new EmailAddress(key.Email);

            string stringContent = $"Thank you for your purchase of {key.Product}!\r\n\r\nYour license key is below:\r\n\r\n{key.Key}";
            string htmlContent = Markdown.ToHtml(stringContent);

            var message = MailHelper.CreateSingleEmailToMultipleRecipients(from,
                new EmailAddress[] { to, from }.ToList(),
                "Your license key from aosoftware.net", stringContent, htmlContent);

            await client.SendEmailAsync(message);
        }
    }
}
