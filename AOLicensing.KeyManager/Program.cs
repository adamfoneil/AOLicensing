using AOLicensing.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOLicensing.KeyManager
{
    internal enum Action
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

            Dictionary<string, (string, Action)> actions = new Dictionary<string, (string, Action)>()
            {
                ["q"] = ("Query", Action.Query),
                ["c"] = ("Create Key", Action.Create)
            };

            do
            {
                Console.WriteLine($"Action ({string.Join(", ", actions.Select(kp => $"{kp.Key} = {kp.Value.Item1}"))}");
                var actionKey = Console.ReadLine();
                var action = actions[actionKey];

                Console.WriteLine($"{action.Item1} - customer email:");
                var email = Console.ReadLine();

                Console.WriteLine("Product:");
                var product = Console.ReadLine();

                switch (action.Item2)
                {
                    case Action.Create:
                        await CreateKeyAsync(client, product, email);
                        break;

                    case Action.Query:
                        await QueryKeyAsync(client, product, email);
                        break;
                }
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
