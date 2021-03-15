using AOLicensing.Desktop.Models;
using AOLicensing.Shared;
using AOLicensing.Shared.Models;
using JsonSettings;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AOLicensing.Desktop
{
    public abstract class Gatekeeper
    {
        private readonly LicensingClient _client;

        public Gatekeeper(string url)
        {
            _client = new LicensingClient(url);
        }

        public abstract string CompanyName { get; }

        public abstract string ProductId { get; }

        public abstract int TrialDays { get; }

        public abstract string PurchaseUrl { get; }

        public abstract Task<decimal> GetPriceAsync();

        /// <summary>
        /// Your app calls this to check trial period end date. App should exit if this returns false
        /// If app is registered, then there is no expiration date, and this returns true
        /// If app has never been run, then client-side Registration with expiration date is generated
        /// </summary>
        public bool StartLicensing()
        {
            var licenseInfo = GetLocalLicense();

            if (licenseInfo.IsActivated)
            {
                return true;
            }
            else
            {
                return ShowActivationDialog(licenseInfo);
            }
        }

        public bool IsActivated()
        {
            var license = GetLocalLicense();
            return license.IsActivated;
        }

        public double TrialPeriodRemaining()
        {
            var license = GetLocalLicense();
            return license.TrialPeriodRemaining;
        }

        private LicenseInfo GetLocalLicense()
        {
            string licenseFile = LocalLicenseFilename();

            if (!File.Exists(licenseFile))
            {
                var info = new LicenseInfo(TrialDays);
                var newTrial = LicenseWrapper.FromInfo(info);
                JsonFile.Save(licenseFile, newTrial);
            }

            var wrapper = JsonFile.Load<LicenseWrapper>(licenseFile);
            return JsonConvert.DeserializeObject<LicenseInfo>(wrapper.Info);
        }

        private string LocalLicenseFilename() => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CompanyName, ProductId + ".json");
        
        protected abstract bool ShowActivationDialog(LicenseInfo licenseInfo);

        /// <summary>
        /// Called when user enters license key in UI to complete app purchase,
        /// removes expiration date from client-side registration info
        /// </summary>
        public async Task<ValidateResult> ActivateAsync(string email, string key)
        {
            var result = await _client.ValidateAsync(new LicenseKey()
            {
                Product = ProductId,
                Email = email,
                Key = key
            });
           
            if (result.Success)
            {
                var license = GetLocalLicense();
                license.Activate();
                JsonFile.Save(LocalLicenseFilename(), LicenseWrapper.FromInfo(license));
            }

            return result;
        }
    }
}
