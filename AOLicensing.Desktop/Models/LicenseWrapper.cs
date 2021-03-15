using JsonSettings;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace AOLicensing.Desktop.Models
{
    /// <summary>
    /// Wrapper class used to encrypt registration status and trial expiration date on client computer
    /// </summary>
    public class LicenseWrapper
	{
		public LicenseWrapper()
		{
		}

		internal static LicenseWrapper FromInfo(LicenseInfo info)
		{
			return new LicenseWrapper()
			{
				Info = JsonConvert.SerializeObject(info)
			};
		}

		[JsonProtect(DataProtectionScope.LocalMachine)]
		public string Info { get; set; }
	}
}
