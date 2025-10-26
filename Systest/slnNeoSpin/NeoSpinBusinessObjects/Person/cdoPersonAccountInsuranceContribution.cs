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
    [Serializable]
	public class cdoPersonAccountInsuranceContribution : doPersonAccountInsuranceContribution
	{
        
		public cdoPersonAccountInsuranceContribution() : base()
		{
		}
        public string strOrg { get; set; }//PIR 7586

        private string  _PremiumMonth;

        public string PremiumMonth
        {
            get { return _PremiumMonth; }
            set { _PremiumMonth = value; }
        }
        private decimal _balance_amount;

        public decimal balance_amount
        {
            get { return _balance_amount; }
            set { _balance_amount = value; }
        }

        public string PremiumMonthForOrderby
        {
            get
            {
                string[] lstrPremiumMonth = PremiumMonth.Split('/');
                return lstrPremiumMonth[1] + lstrPremiumMonth[0];
            }
        }

        // to display Month/Year in UCS-041
        public string istrPostedMonthYear
        {
            get
            {
                string lstrMonthYear = String.Empty;
                lstrMonthYear = transaction_date.ToString("MMM") + "/" + transaction_date.Year.ToString();
                return lstrMonthYear;
            }
        }

        //this is used in 41 to select records
        public string istrSelected { get; set; }

        //used for ucs - 038 addendum, to get the order in which remittance will be allocated
        public string rec_order { get; set; }

        //PIR 8502
        public string istrEditedSubsystem
        {
            get
            {
                if (subsystem_description.StartsWith("IBS"))
                    return subsystem_description.Substring(4);
                return subsystem_description;
            }

        }

        //PIR 15870
        public decimal idecTotalDuePremiumAmount { get; set; }
        public decimal idecTotalPaidPremiumAmount { get; set; }

    } 
} 
