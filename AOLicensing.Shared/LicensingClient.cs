using AOLicensing.Shared.Interfaces;
using AOLicensing.Shared.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AOLicensing.Shared
{
    public class LicensingClient
    {
        private readonly ILicensingClient _client;
        private readonly string _adminCode;

        public LicensingClient(string url, string adminCode = null)
        {
            _adminCode = adminCode;
            _client = RestService.For<ILicensingClient>(url, new RefitSettings()
            {
                AuthorizationHeaderValueWithParamGetter = async (msg) =>
                {
                    return await Task.FromResult($"?code={_adminCode}");
                }
            });
        }

        public async Task<LicenseKey> CreateKeyAsync(CreateKey key) => await _client.CreateAsync(key);

        public async Task<ValidateResult> ValidateAsync(LicenseKey key) => await _client.ValidateAsync(key);

        public async Task<IReadOnlyList<string>> QueryAsync(CreateKey key) => await _client.QueryAsync(key);
    }
}
