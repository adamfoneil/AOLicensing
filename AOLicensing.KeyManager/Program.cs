using AOLicensing.Shared;
using CommandLine;
using System;
using System.Threading.Tasks;

namespace AOLicensing.KeyManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new LicensingClient("https://aosoftware.ngrok.io");

            await Parser.Default
                .ParseArguments<Options>(args)
                .WithParsedAsync(async options =>
                {
                    switch (options.Action)
                    {
                        case Action.Create:
                            await CreateKeyAsync(client, options);
                            break;

                        case Action.Query:
                            await QueryKeyAsync(client, options);
                            break;

                        case Action.Validate:
                            // this is for interactive validation. I don't see this being used very much
                            break;
                    }
                });                
        }

        private async static Task QueryKeyAsync(LicensingClient client, Options options)
        {
            var result = await client.QueryAsync(new Shared.Models.CreateKey()
            {
                Product = options.Product,
                Email = options.Email
            });

            Console.WriteLine($"Found {result.Count} result(s):");
            foreach (var item in result)
            {
                Console.WriteLine(" " + item);
            }
        }

        private async static Task CreateKeyAsync(LicensingClient client, Options options)
        {
            var result = await client.CreateKeyAsync(new Shared.Models.CreateKey()
            {
                Email = options.Email,
                Product = options.Product
            });

            Console.WriteLine($"Created key {result.Key} for {result.Email}");
        }
    }
}
