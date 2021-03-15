using AOLicensing.Shared;
using AOLicensing.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing
{
    [TestClass]
    public class ApiClientTests
    {
        const string HostUrl = "https://aolicensing.azurewebsites.net";

        [TestMethod]
        public void CreateKey()
        {
            var client = new LicensingClient(HostUrl, "hello");
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
    }
}
