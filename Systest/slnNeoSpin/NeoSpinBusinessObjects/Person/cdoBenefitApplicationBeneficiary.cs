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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoBenefitApplicationBeneficiary:
	/// Inherited from doBenefitApplicationBeneficiary, the class is used to customize the database object doBenefitApplicationBeneficiary.
	/// </summary>
    [Serializable]
	public class cdoBenefitApplicationBeneficiary : doBenefitApplicationBeneficiary
	{
		public cdoBenefitApplicationBeneficiary() : base()
		{
		}

        public string istrBenefitAccountType { get; set; }
        public string istrBenefitAccountTypeValue { get; set; }
        public string istrBenefitOption { get; set; }
        public string istrPlanName { get; set; }
        public string istrAccountRelationship { get; set; }
        public DateTime idtRetirementDate { get; set; }
        public bool lblnValueEntered { get; set; }
        public bool IsEnteredInNewMode { get; set; }
        public int iintApplicationId { get; set; }

        public DateTime end_date_no_null
        {
            get
            {
                if (end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                else
                    return end_date;
            }
        }

        public int sort_order
        {
            get
            {
                if ((beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary) &&
                    (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, start_date, end_date_no_null)))
                    return 1;
                else if ((beneficiary_type_value == BusinessObjects.busConstant.BeneficiaryMemberTypeContingent) &&
                        (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, start_date, end_date_no_null)))
                    return 2;
                else if ((beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary) &&
                        (!(busGlobalFunctions.CheckDateOverlapping(DateTime.Now, start_date, end_date_no_null))))
                    return 3;
                else if ((beneficiary_type_value == busConstant.BeneficiaryMemberTypeContingent) &&
                        (!(busGlobalFunctions.CheckDateOverlapping(DateTime.Now, start_date, end_date_no_null))))
                    return 4;
                return 0;
            }
        }
    } 
} 
