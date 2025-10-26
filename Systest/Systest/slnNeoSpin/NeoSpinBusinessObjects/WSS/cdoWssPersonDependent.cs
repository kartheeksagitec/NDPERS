#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonDependent:
	/// Inherited from doWssPersonDependent, the class is used to customize the database object doWssPersonDependent.
	/// </summary>
    [Serializable]
	public class cdoWssPersonDependent : doWssPersonDependent
	{
		public cdoWssPersonDependent() : base()
		{
		}

        public bool iblnResult { get; set; } //PIR 11841
        public int iintPlanId { get; set; }
        public DateTime idtDateOfChange { get; set; } //PIR 11905
        public string istrDependentName { get; set; }
        public int iintDependentPersonId { get; set; }
        public int iintDependentSeqCount { get; set; }

        public string LastFourDigitsOfSSN
        {
            get
            {
                if ((ssn != null) && (ssn.Length == 9))
                {
                    return ssn.Substring(5);
                }
                return string.Empty;
            }
        }

        public int iintPersonID { get; set; }

        public string level_of_coverage_value { get; set; }

        public int iintDependentAge { get; set; } //PIR 11905

        public String FullName
        {
            get
            {
                string lstrName = String.Empty;
                if (!String.IsNullOrEmpty(first_name))
                {
                    lstrName = first_name.Trim();
                }
                if (!String.IsNullOrEmpty(middle_name))
                {
                    lstrName += " " + middle_name.Trim();
                }
                if (!String.IsNullOrEmpty(last_name))
                    lstrName += " " + last_name.Trim();
                return lstrName;
            }
        }
        public int is_health_enrolled_already { get; set; }
        public int is_dental_enrolled_already { get; set; }
        public int is_vision_enrolled_already { get; set; }

        public string is_health_enrolled { get; set; }
        public string is_dental_enrolled { get; set; }
        public string is_vision_enrolled { get; set; }
        public int enroll_req_plan_id { get; set; }
        public bool iblnIsHealthEnrltAsCobra { get; set; }
        public bool iblnIsDentalEnrltAsCobra { get; set; }
        public bool iblnIsVisionEnrltAsCobra { get; set; }
        public bool iblnIsHealthEnrltAsRetiree { get; set; }
        public bool iblnIsDentalEnrltAsRetiree { get; set; }
        public bool iblnIsVisionEnrltAsRetiree { get; set; }

        public bool iblnHealthCovEnrollFamily { get; set; }
        public bool iblnVisionCovEnrollFamily { get; set; }
        public bool iblnDentalCovEnrollFamily { get; set; }
        public bool iblnDentalCovEnrollIndSpouse { get; set; }
        public bool iblnDentalCovEnrollIndChild { get; set; }
        public bool iblnVisionCovEnrollIndSpouse { get; set; }
        public bool iblnVisionCovEnrollIndChild { get; set; }

    } 
} 
