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
	public class cdoPersonAccountGhdv : doPersonAccountGhdv
	{
		public cdoPersonAccountGhdv() : base()
		{
		}

        //Start PIR 8518
        private string _CobraTypeValue;
        public string CobraTypeValue
        {
            get { return _CobraTypeValue; }
            set { _CobraTypeValue = value; }
        }
        //End PIR 8518

        private decimal _MonthlyPremiumAmount;
        public decimal MonthlyPremiumAmount
        {
            get { return _MonthlyPremiumAmount; }
            set { _MonthlyPremiumAmount = value; }
        }

        private decimal _BuydownAmount;
        public decimal BuydownAmount
        {
            get { return _BuydownAmount; }
            set { _BuydownAmount = value; }
        }

        //PIR 14271
        private decimal _MedicarePartDAmount;
        public decimal MedicarePartDAmount
        {
            get { return _MedicarePartDAmount; }
            set { _MedicarePartDAmount = value; }
        }

        private decimal _PremiumExcludingFeeAmount;
        public decimal PremiumExcludingFeeAmount
        {
            get { return _PremiumExcludingFeeAmount; }
            set { _PremiumExcludingFeeAmount = value; }
        }

        private decimal _FeeAmount;
        public decimal FeeAmount
        {
            get { return _FeeAmount; }
            set { _FeeAmount = value; }
        }

        /// Used in Corresondence
        private string _istr_FullTime_Student;
        public string istr_FullTime_Student
        {
            get { return _istr_FullTime_Student; }
            set { _istr_FullTime_Student = value; }
        }

        /// Used in Correspondence
        private int _lint_Dependent_Person_ID;
        public int lint_Dependent_Person_ID
        {
            get { return _lint_Dependent_Person_ID; }
            set { _lint_Dependent_Person_ID = value; }
        }
       
        private int _Rate_Ref_ID;
        public int Rate_Ref_ID
        {
            get { return _Rate_Ref_ID; }
            set { _Rate_Ref_ID = value; }
        }

        private string _rate_structure_code;
        public string rate_structure_code
        {
            get { return _rate_structure_code; }
            set { _rate_structure_code = value; }
        }

        private int _Coverage_Ref_ID;
        public int Coverage_Ref_ID
        {
            get { return _Coverage_Ref_ID; }
            set { _Coverage_Ref_ID = value; }
        }

        public decimal total_rhic_amount { get; set; }
        public decimal js_rhic_amount { get; set; }
        public decimal other_rhic_amount { get; set; }

        public bool is_health_cobra
        {
            get
            {
                //PIR 25083 - cobra_type_value != null was always returning true for temporary employees as the cobra value is set to string.Empty when enrolled from MSS.
                if (!string.IsNullOrEmpty(cobra_type_value)) 
                    return true;
                return false;
            }
        }

        public string derived_rate_structure_code
        {
            get
            {
                if (!string.IsNullOrEmpty(overridden_structure_code))
                    return overridden_structure_code;
                else
                    return rate_structure_code;
            }
        }

        public string is_level_of_coverage_modified { get; set; }

        public string is_reinstatement { get; set; }

        public int iintMedicareIn { get; set; }

        #region PROD PIR 933

        public string splitted_coverage_code { get; set; }

        public bool is_medicare_split
        {
            get
            {
                if(string.IsNullOrEmpty(medicare_claim_no))
                    return false;
                return true;
            }
        }

        #endregion

        public decimal idecHSAAmount { get; set; }
        public decimal idecHSAVendorAmount { get; set; }
        public string current_dental_insurance_type_value { get; set; }
        public string current_level_of_coverage_value { get; set; }
        public DateTime current_plan_start_date_from_history { get; set; }
        public DateTime current_plan_end_date_from_history { get; set; }
        public DateTime previous_history_start_date { get; set; }
        // PIR 9812
        public string type_of_coverage_for_mss
        {
            get
            {
                if (alternate_structure_code_value == BusinessObjects.busConstant.AlternateStructureCodeHDHP)
                    return alternate_structure_code_description;
                return "PPO/Basic";
            }
        }
    }
}
