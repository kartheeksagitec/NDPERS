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
    public class cdoPersonAccountRetirement : doPersonAccountRetirement
    {
        public cdoPersonAccountRetirement()
            : base()
        {
        }

        private int _LetterNo;
        public int LetterNo
        {
            get
            {
                _LetterNo = 0;
                //PIR 11483
                if ((dc_trasnfer_reminder_letter1_date) == DateTime.MinValue)
                    _LetterNo = 1;
                if (((dc_trasnfer_reminder_letter1_date).AddDays(120) < DateTime.Today) && (dc_trasnfer_reminder_letter1_date != DateTime.MinValue))
                    _LetterNo = 2;
                return _LetterNo;
            }
        }

        private int _DateDifference;
        public int DateDifference
        {
            get { return _DateDifference; }
            set { _DateDifference = value; }
        }

        // PIR ID 2189
        public string dc_transfer_first_reminder_long_date
        {
            get
            {
                return dc_trasnfer_reminder_letter1_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }

        public string dc_eligibility_long_date { get; set; }

        public DateTime dc_eligibility_no_null
        {
            get
            {
                if (dc_eligibility_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                return dc_eligibility_date;
            }
        }

        # region UCS 24 contribution summary properties
        public int iintMSSPlanId { get; set; }
        public decimal idecMSSMemberContribution { get; set; }
        public string istrMSSPlanName { get; set; }
        public decimal idecMSSVestedEmployerContribution { get; set; }
        public decimal idecMSSInterest { get; set; }
        public decimal idecMSSTotalNonTaxable { get; set; }
        public decimal idecMSSTotalTaxable { get; set; }
        public decimal idecMSSRHICEEContribution { get; set; }
        public decimal idecMSSRHICServiceCreditContribution { get; set; }
        public decimal idecMSSEEPretax { get; set; }
        public decimal idecMSSEEPostTax { get; set; }
        public decimal idecMSSEEPickup { get; set; }
        # endregion

        public string view_accrued_benefit
        {
            get
            {
                return "View Accrued Benefit";
            }
        }
        public string benefit_tier_description_display
        {
            get
            {
			    // PIR 26317 
                //if(string.IsNullOrEmpty(benefit_tier_value))
                //    return busGlobalFunctions.GetDescriptionByCodeValue(7003, busConstant.MainBenefit1997Tier, iobjPassInfo);
                return busGlobalFunctions.GetDescriptionByCodeValue(7003, benefit_tier_value, iobjPassInfo);
            }
        }
        public bool is_from_mss { get; set; }
    }
}
