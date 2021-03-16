using AOLicensing.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOLicensing.KeyManager
{
    internal enum ActionType
    {
        Query,
        Create,
        Validate
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddUserSecrets("5a664fcb-5e4e-479d-872f-f47dbbb60ade").Build();
            var client = new LicensingClient("https://aolicensing.azurewebsites.net", config["Codes:Master"]);
            
            Console.WriteLine("AOLicensing KeyManager");

            Dictionary<string, (string, ActionType, Func<string, string, Task>)> actions = new Dictionary<string, (string, ActionType, Func<string, string, Task>)>()
            {
                ["q"] = ("Query", ActionType.Query, async (email, product) => await QueryKeyAsync(client, product, email)),
                ["c"] = ("Create Key", ActionType.Create, async (email, product) => await CreateKeyAsync(client, product, email))
            };

            do
            {
                Console.WriteLine($"Action ({string.Join(", ", actions.Select(kp => $"{kp.Key} = {kp.Value.Item1}"))})");
                var actionKey = Console.ReadLine();
                if (!actions.ContainsKey(actionKey)) continue;

                var action = actions[actionKey];

                Console.WriteLine($"{action.Item1} - customer email:");
                var email = Console.ReadLine();

                Console.WriteLine("Product:");
                var product = Console.ReadLine();

                await action.Item3.Invoke(email, product);
            } while (true);

        }

        private async static Task QueryKeyAsync(LicensingClient client, string product, string email)
        {
            var result = await client.QueryAsync(new Shared.Models.CreateKey()
            {
                Product = product,
                Email = email
            });

            Console.WriteLine($"Found {result.Count} result(s):");
            foreach (var item in result)
            {
                Console.WriteLine(" " + item);
            }
            Console.WriteLine();
        }

        private async static Task CreateKeyAsync(LicensingClient client, string product, string email)
        {
            var result = await client.CreateKeyAsync(new Shared.Models.CreateKey()
            {
                Email = email,
                Product = product
            });

            Console.WriteLine($"Created {result.Product} key {result.Key} for {result.Email}");
            Console.WriteLine();
        }
    }
}
