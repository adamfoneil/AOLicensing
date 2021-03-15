using Newtonsoft.Json;
using System;

namespace AOLicensing.Desktop.Models
{
    /// <summary>
    /// serialized and encrypted into the License.Info property
    /// </summary>
    public class LicenseInfo
	{
		public LicenseInfo()
		{
		}

		public LicenseInfo(int trialDays)
		{
			TrialEndDate = DateTime.Today.AddDays(trialDays);
		}

		public const string TrialStatus = "trial";
		public const string ActivatedStatus = "activated";
		public string Status { get; set; } = TrialStatus;
		public DateTime TrialEndDate { get; set; }

		[JsonIgnore]
		public bool IsActivated { get { return Status.Equals(ActivatedStatus); } }

		[JsonIgnore]
		public bool AllowContinue { get { return DateTime.Today <= TrialEndDate; } }

		[JsonIgnore]
		public double TrialPeriodRemaining { get { return TrialEndDate.Subtract(DateTime.Today).TotalDays; } }

		public void Activate() { Status = ActivatedStatus; }
	}
}
