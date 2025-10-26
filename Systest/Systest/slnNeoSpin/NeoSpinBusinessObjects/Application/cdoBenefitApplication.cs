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
    public class cdoBenefitApplication : doBenefitApplication
    {
        public cdoBenefitApplication()
            : base()
        {
        }

        private int _iintPersonAccountID;
        public int iintPersonAccountID
        {
            get { return _iintPersonAccountID; }
            set { _iintPersonAccountID = value; }
        }

        private decimal _idecRHICAmount;
        public decimal idecRHICAmount
        {
            get { return _idecRHICAmount; }
            set { _idecRHICAmount = value; }
        }

        private DateTime _idtNormalRetirementDate;
        public DateTime idtNormalRetirementDate
        {
            get { return _idtNormalRetirementDate; }
            set { _idtNormalRetirementDate = value; }
        }

        private Decimal _idecTVSC;
        public Decimal idecTVSC
        {
            get { return _idecTVSC; }
            set { _idecTVSC = value; }
        }

        private int _benefit_calculation_id;
        public int benefit_calculation_id
        {
            get { return _benefit_calculation_id; }
            set { _benefit_calculation_id = value; }
        }

        private string _istrOrgCode;
        public string istrOrgCode
        {
            get { return _istrOrgCode; }
            set { _istrOrgCode = value; }
        }

        //this variable is used to check if validations in Other Disbaility Benefits
        private bool _iblnIsStartDateBlankOthrDisBenefit;
        public bool iblnIsStartDateBlankOthrDisBenefit
        {
            get { return _iblnIsStartDateBlankOthrDisBenefit; }
            set { _iblnIsStartDateBlankOthrDisBenefit = value; }
        }
       
        private bool _iblnIsBenefitAmountBlankOthrDisBenefit;
        public bool iblnIsBenefitAmountBlankOthrDisBenefit
        {
            get { return _iblnIsBenefitAmountBlankOthrDisBenefit; }
            set { _iblnIsBenefitAmountBlankOthrDisBenefit = value; }
        }
        
        private bool _iblnIsBenefitAmountNegativeOthrDisBenefit;
        public bool iblnIsBenefitAmountNegativeOthrDisBenefit
        {
            get { return _iblnIsBenefitAmountNegativeOthrDisBenefit; }
            set { _iblnIsBenefitAmountNegativeOthrDisBenefit = value; }
        }
        
        private bool _iblnIsStartDateGreaterThanEndDateOthrDisBenefit;
        public bool iblnIsStartDateGreaterThanEndDateOthrDisBenefit
        {
            get { return _iblnIsStartDateGreaterThanEndDateOthrDisBenefit; }
            set { _iblnIsStartDateGreaterThanEndDateOthrDisBenefit = value; }
        }

        private string _istrIsPersonVested;
        public string istrIsPersonVested
        {
            get { return _istrIsPersonVested; }
            set { _istrIsPersonVested = value; }
        }

        private string _istrPersonName;
        public string istrPersonName
        {
            get { return _istrPersonName; }
            set { _istrPersonName = value; }
        }

        public string istrRecipientPersonID
        {
            get 
            {
                string lstrRecipientPersonID = String.Empty;
                if (recipient_person_id != 0)
                    lstrRecipientPersonID = recipient_person_id.ToString();
                return lstrRecipientPersonID; 
            }         
        }       

        //this property is used the Post retirement death application Load other application grid
        //to display Person name and Org name as beneficiary name
        public string istrBeneficiaryName { get; set; }

        public string istrApplicationStatus 
        {
            get
            {
                string lstrApplicationStatus = string.Empty;
                if ((!String.IsNullOrEmpty(action_status_description))
                    && (!String.IsNullOrEmpty(status_value)))
                {
                    lstrApplicationStatus = action_status_description + " / " + status_description;
                }
                return lstrApplicationStatus;
            }
        }

        # region Correspondence
        //these properties are used in corresspondence
        public int iintTodaysDay
        {
            get
            {
                return DateTime.Today.Day;
            }
        }
        public string iintTodaysMonth
        {
            get
            {
                return DateTime.Today.ToString("MMMM");
            }
        }
        public int iintTodaysYear
        {
            get
            {
                return DateTime.Today.Year;
            }
        }

        private int _iintMemberAgeYearPart;
        public int iintMemberAgeYearPart
        {
            get { return _iintMemberAgeYearPart; }
            set { _iintMemberAgeYearPart = value; }
        }

        private int _iintMemberAgeMonthPart;
        public int iintMemberAgeMonthPart
        {
            get { return _iintMemberAgeMonthPart; }
            set { _iintMemberAgeMonthPart = value; }
        }

        //UCS - 081 Property to contain Benefit Option Desc
        public string istrBenefitOptionDescNotNull
        {
            get
            {
                return benefit_option_description == null ? busConstant.BenefitOptionNotApplied : benefit_option_description;
            }
        }

        public string istrUniversityCodeCaps
        {
            get
            {
                return nd_univ_code_description.ToUpper();
            }
        }
        # endregion

        public bool IsTermCertainBenefitOption()
        {
            if ((benefit_option_value == "L05C") ||
                (benefit_option_value == "L10C") ||
                (benefit_option_value == "L20C") ||
                (benefit_option_value == "LA10") ||
                (benefit_option_value == "LA15") ||
                (benefit_option_value == "LA20") ||
                (benefit_option_value == "LB05") ||
                (benefit_option_value == "LB10") ||
                (benefit_option_value == "LB15") ||
                (benefit_option_value == "LB20") ||
                (benefit_option_value == "T10C") ||
                (benefit_option_value == "T15C") ||
                (benefit_option_value == "T20C") ||
                (benefit_option_value == "5YTL"))
                return true;
            return false;
        }

        //prop used in the bene to bene
        public int iintDROApplicationId { get; set; }
        public int iintApplicationId { get; set; }

        public string retirement_long_date
        {
            get
            {
                return retirement_date.ToString(busConstant.DateFormatLongDate);
            }
        }
        //PIR 1566 fix
        public string received_date_long
        {
            get
            {
                if (received_date != DateTime.MinValue)
                    return received_date.ToString(busConstant.DateFormatLongDate);
                return string.Empty;
            }
        }
        public string payment_date_long
        {
            get
            {
                if (payment_date != DateTime.MinValue)
                    return payment_date.ToString(busConstant.DateFormatLongDate);
                return string.Empty;
            }
        }
        public string Plan_Name { get; set; } //PIR 18493 - To Display Plan Name on MSS - View App Status
        public string mss_display_status { get; set; }
    }
}
