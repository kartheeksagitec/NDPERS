#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoBenefitLtcDeduction : doBenefitLtcDeduction
	{
		public cdoBenefitLtcDeduction() : base()
		{
		}

        private bool _iblnValueEntered;
        public bool iblnValueEntered
        {
            get
            {
                return _iblnValueEntered;
            }
            set
            {
                _iblnValueEntered = value;
            }
        }

        public string member_insurance_type { get; set; }

        public string member_insurance_type_desc
        {
            get
            {
                if(!string.IsNullOrEmpty(member_insurance_type))
                    return busGlobalFunctions.GetDescriptionByCodeValue(339, member_insurance_type, iobjPassInfo);
                return string.Empty;
            }
        }

        public string spouse_insurance_type { get; set; }

        public string spouse_insurance_type_desc
        {
            get
            {
                if (!string.IsNullOrEmpty(spouse_insurance_type))
                    return busGlobalFunctions.GetDescriptionByCodeValue(339, spouse_insurance_type, iobjPassInfo);
                return string.Empty;
            }
        }
    } 
} 
