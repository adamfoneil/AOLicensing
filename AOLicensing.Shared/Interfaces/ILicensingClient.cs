using AOLicensing.Shared.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AOLicensing.Shared.Interfaces
{
    internal interface ILicensingClient
    {
        [Post("/api/CreateKey?code={code}")]
        Task<LicenseKey> CreateAsync(CreateKey key, string code);

        [Get("/api/ValidateKey")]
        Task<ValidateResult> ValidateAsync([Body]LicenseKey key);

        [Get("/api/QueryKey?email={key.Email}&product={key.Product}&code={code}")]
        Task<IReadOnlyList<string>> QueryAsync(CreateKey key, string code);
    }
}
