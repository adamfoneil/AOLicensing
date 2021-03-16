using AOLicensing.Shared;
using AOLicensing.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing
{
    [TestClass]
    public class ApiClientTests
    {
        const string HostUrl = "https://aolicensing.azurewebsites.net";
        //const string HostUrl = "https://aosoftware.ngrok.io";

        [TestMethod]
        public void CreateKey()
        {
            var config = GetConfig();

            var client = new LicensingClient(HostUrl, config["Codes:Master"]);
            var result = client.CreateKeyAsync(new CreateKey()
            {
                Email = "adamosoftware@gmail.com",
                Product = "SampleProduct"
            }).Result;

            Assert.IsTrue(!string.IsNullOrEmpty(result.Key));
        }

        [TestMethod]
        public void ValidateKey()
        {
            var client = new LicensingClient(HostUrl);            
            var result = client.ValidateAsync(new LicenseKey()
            {
                Email = "adamosoftware@gmail.com",
                Product = "SampleProduct",
                Key = "TUJQ-89Q8-IZA3"
            }).Result;

            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void QueryKey()
        {
            var config = GetConfig();

            var client = new LicensingClient(HostUrl, config["Codes:Master"]);
            var result = client.QueryAsync(new CreateKey()
            {
                Email = "adamosoftware@gmail.com",
                Product = "SampleProduct"
            }).Result;

            Assert.IsTrue(result.Count > 0);
        }

        private static IConfiguration GetConfig() => new ConfigurationBuilder()
            .AddJsonFile("Config\\config.json", false)
            .Build();
    }
}
