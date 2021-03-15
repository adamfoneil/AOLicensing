using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace AOLicensing.Functions.Extensions
{
    public static class ConfigHelper
    {
        public static IConfiguration GetConfig(this ExecutionContext context)
        {
            return new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
