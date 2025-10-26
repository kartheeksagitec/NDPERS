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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountGhdv:
	/// Inherited from doWssPersonAccountGhdv, the class is used to customize the database object doWssPersonAccountGhdv.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountGhdv : doWssPersonAccountGhdv
	{
		public cdoWssPersonAccountGhdv() : base()
		{
		}

        public string is_eligible_dependent_exists_flag_desc
        {
            get
            {
                if (is_eligible_dependent_exists_flag == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

        public string keeping_other_coverage_flag_desc
        {
            get
            {
                if (keeping_other_coverage_flag == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

        public string dependent_receiving_workers_compensation_flag_desc
        {
            get
            {
                if (dependent_receiving_workers_compensation_flag == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

        public string dependent_receiving_no_fault_benefits_flag_desc
        {
            get
            {
                if (dependent_receiving_no_fault_benefits_flag == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

        public string is_dependent_medicare_eligible_desc
        {
            get
            {
                if (is_dependent_medicare_eligible == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

        public string is_dependent_medicare_esrd_desc
        {
            get
            {
                if (is_dependent_medicare_esrd == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

        public string is_premium_tax_deduction_desc
        {
            get
            {
                if (pre_tax_payroll_deduction_flag == BusinessObjects.busConstant.Flag_Yes)
                    return BusinessObjects.busConstant.Flag_Yes_Value;
                return BusinessObjects.busConstant.Flag_No_Value;
            }
        }

        // PIR 9812
        public string type_of_coverage_for_mss
        {
            get
            {
                if (type_of_coverage_value == BusinessObjects.busConstant.AlternateStructureCodeHDHP)
                    return BusinessObjects.busGlobalFunctions.GetDescriptionByCodeValue(5015, type_of_coverage_value, iobjPassInfo);
                return "PPO/Basic";
            }
        }
        public int request_plan_id { get; set; } //PIR 18493
    } 
} 
