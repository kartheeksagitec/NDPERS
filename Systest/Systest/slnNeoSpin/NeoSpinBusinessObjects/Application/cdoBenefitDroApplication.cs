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
	public class cdoBenefitDroApplication : doBenefitDroApplication
	{
		public cdoBenefitDroApplication() : base()
		{
		}
      
        private decimal _idecTVSC;

        public decimal idecTVSC
        {
            get { return _idecTVSC; }
            set { _idecTVSC = value; }
        }

        //Property to store member person id, for audit log purpose
        public int person_id { get; set; }

        #region Correspondence Properties

        //UCS - 086 : Property to store Approve date in Month Year format
        public string ApprovedDateFormatted
        {
            get
            {
                return approved_date.ToString("MMMM") + " " + approved_date.Year.ToString();
            }
        }

        public string istrBenefitDurationFullDesc { get; set; }

        public DateTime idtBenefitReceiptDate { get; set; }

        //UCS -086 : Propety to store MemberGrossMonthlyAmount
        public decimal idecGrossMonthlyAmount
        {
            get
            {
                return (overridden_member_gross_monthly_amount == 0.0M ?
                computed_member_gross_monthly_amount : overridden_member_gross_monthly_amount);
            }
        }
        #endregion

        // Used in BR-054-014
        public bool IsTermCertainBenefitOption()
        {
            if ((benefit_duration_option_value == "10AP") ||
                (benefit_duration_option_value == "RJ10") ||
                (benefit_duration_option_value == "10FM") ||
                (benefit_duration_option_value == "10RJ") ||
                (benefit_duration_option_value == "10AD") ||
                (benefit_duration_option_value == "OJ10") ||
                (benefit_duration_option_value == "15JA") ||
                (benefit_duration_option_value == "RJ15") ||
                (benefit_duration_option_value == "20JA") ||
                (benefit_duration_option_value == "RJ20") ||
                (benefit_duration_option_value == "20RJ") ||
                (benefit_duration_option_value == "20AD") ||
                (benefit_duration_option_value == "5YFM") ||
                (benefit_duration_option_value == "10JS") ||
                (benefit_duration_option_value == "10DB") ||
                (benefit_duration_option_value == "LB10") ||
                (benefit_duration_option_value == "20JS") ||
                (benefit_duration_option_value == "20DB") ||
                (benefit_duration_option_value == "OJ20") ||
                (benefit_duration_option_value == "5LFM") ||
                (benefit_duration_option_value == "OJ15"))
                return true;
            return false;
        }
        //uat pir 913
        public string istrBenefitReceiptDate
        {
            get
            {
                if (benefit_receipt_date != DateTime.MinValue)
                    return benefit_receipt_date.ToString("MMMM") + " " + benefit_receipt_date.Day.ToString() + ", " + benefit_receipt_date.Year.ToString();
                else
                    return string.Empty;
            }
        }
        //uat pir 913
        public string istrDivorceDate
        {
            get
            {
                if (date_of_divorce != DateTime.MinValue)
                    return date_of_divorce.ToString("MMMM") + " " + date_of_divorce.Day.ToString() + ", " + date_of_divorce.Year.ToString();
                else
                    return string.Empty;
            }
        }
        //uat pir 914
        public string istrApprovedDate
        {
            get
            {
                if (approved_date != DateTime.MinValue)
                    return approved_date.ToString("MMMM") + " " + approved_date.Day.ToString() + ", " + approved_date.Year.ToString();
                else
                    return string.Empty;
            }
        }
        //uat pir 914
        public string istrQualifiedDate
        {
            get
            {
                if (qualified_date != DateTime.MinValue)
                    return qualified_date.ToString("MMMM") + " " + qualified_date.Day.ToString() + ", " + qualified_date.Year.ToString();
                else
                    return string.Empty;
            }
        }
        //uat pir 1826,1827
        public string istrMarriageDate
        {
            get
            {
                if (date_of_marriage != DateTime.MinValue)
                    return date_of_marriage.ToString(busConstant.DateFormatLongDate);
                else
                    return string.Empty;
            }
        }
    } 
} 
