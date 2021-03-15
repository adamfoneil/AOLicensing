using CommandLine;
using System;
using System.Net.Http;

namespace AOLicensing.KeyManager
{
    class Program
    {
        private static HttpClient _client = new HttpClient();

        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    switch (options.Action)
                    {
                        case Action.Create:
                            break;

                        case Action.Query:
                            break;

                        case Action.Validate:
                            // this is for interactive validation. I don't see this being used very much
                            break;
                    }
                });                
        }
    }
}
