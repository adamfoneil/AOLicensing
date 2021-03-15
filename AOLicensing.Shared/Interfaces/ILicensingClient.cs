using AOLicensing.Shared.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AOLicensing.Shared.Interfaces
{
    public interface ILicensingClient
    {
        [Post("/api/CreateKey")]
        Task<LicenseKey> CreateAsync(CreateKey key);

        [Get("/api/ValidateKey")]
        Task<ValidateResult> ValidateAsync(LicenseKey key);

        Task<IReadOnlyList<string>> QueryAsync(CreateKey key);
    }
}
