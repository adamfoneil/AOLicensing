using AOLicensing.Shared.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AOLicensing.Shared.Interfaces
{
    internal interface ILicensingClient
    {
        [Post("/api/CreateKey")]
        Task<LicenseKey> CreateAsync(CreateKey key);

        [Get("/api/ValidateKey?email={key.Email}&product={key.Product}&key={key.Key}")]
        Task<ValidateResult> ValidateAsync(LicenseKey key);

        [Get("/api/QueryKey?email={key.Email}&product={key.Product}")]
        Task<IReadOnlyList<string>> QueryAsync(CreateKey key);
    }
}
