#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doOrgPlanLifeRate : doBase
    {
         
         public doOrgPlanLifeRate() : base()
         {
         }
		private int _org_plan_life_rate_id;
		public int org_plan_life_rate_id
		{
			get
			{
				return _org_plan_life_rate_id;
			}

			set
			{
				_org_plan_life_rate_id = value;
			}
		}

		private int _org_plan_id;
		public int org_plan_id
		{
			get
			{
				return _org_plan_id;
			}

			set
			{
				_org_plan_id = value;
			}
		}

		private DateTime _effective_date;
		public DateTime effective_date
		{
			get
			{
				return _effective_date;
			}

			set
			{
				_effective_date = value;
			}
		}

		private int _life_insurance_type_id;
		public int life_insurance_type_id
		{
			get
			{
				return _life_insurance_type_id;
			}

			set
			{
				_life_insurance_type_id = value;
			}
		}

		private string _life_insurance_type_description;
		public string life_insurance_type_description
		{
			get
			{
				return _life_insurance_type_description;
			}

			set
			{
				_life_insurance_type_description = value;
			}
		}

		private string _life_insurance_type_value;
		public string life_insurance_type_value
		{
			get
			{
				return _life_insurance_type_value;
			}

			set
			{
				_life_insurance_type_value = value;
			}
		}

		private int _level_of_coverage_id;
		public int level_of_coverage_id
		{
			get
			{
				return _level_of_coverage_id;
			}

			set
			{
				_level_of_coverage_id = value;
			}
		}

		private string _level_of_coverage_description;
		public string level_of_coverage_description
		{
			get
			{
				return _level_of_coverage_description;
			}

			set
			{
				_level_of_coverage_description = value;
			}
		}

		private string _level_of_coverage_value;
		public string level_of_coverage_value
		{
			get
			{
				return _level_of_coverage_value;
			}

			set
			{
				_level_of_coverage_value = value;
			}
		}

		private int _min_age_yrs;
		public int min_age_yrs
		{
			get
			{
				return _min_age_yrs;
			}

			set
			{
				_min_age_yrs = value;
			}
		}

		private int _max_age_yrs;
		public int max_age_yrs
		{
			get
			{
				return _max_age_yrs;
			}

			set
			{
				_max_age_yrs = value;
			}
		}

		private decimal _basic_coverage_amt;
		public decimal basic_coverage_amt
		{
			get
			{
				return _basic_coverage_amt;
			}

			set
			{
				_basic_coverage_amt = value;
			}
		}

		private decimal _basic_employee_premium_amt;
		public decimal basic_employee_premium_amt
		{
			get
			{
				return _basic_employee_premium_amt;
			}

			set
			{
				_basic_employee_premium_amt = value;
			}
		}

		private decimal _basic_employer_premium_amt;
		public decimal basic_employer_premium_amt
		{
			get
			{
				return _basic_employer_premium_amt;
			}

			set
			{
				_basic_employer_premium_amt = value;
			}
		}

		private decimal _ad_and_d_basic_premium_rate;
		public decimal ad_and_d_basic_premium_rate
		{
			get
			{
				return _ad_and_d_basic_premium_rate;
			}

			set
			{
				_ad_and_d_basic_premium_rate = value;
			}
		}

		private int _supplemental_rate_criteria_id;
		public int supplemental_rate_criteria_id
		{
			get
			{
				return _supplemental_rate_criteria_id;
			}

			set
			{
				_supplemental_rate_criteria_id = value;
			}
		}

		private string _supplemental_rate_criteria_description;
		public string supplemental_rate_criteria_description
		{
			get
			{
				return _supplemental_rate_criteria_description;
			}

			set
			{
				_supplemental_rate_criteria_description = value;
			}
		}

		private string _supplemental_rate_criteria_value;
		public string supplemental_rate_criteria_value
		{
			get
			{
				return _supplemental_rate_criteria_value;
			}

			set
			{
				_supplemental_rate_criteria_value = value;
			}
		}

		private decimal _supplemental_premium_rate_amt;
		public decimal supplemental_premium_rate_amt
		{
			get
			{
				return _supplemental_premium_rate_amt;
			}

			set
			{
				_supplemental_premium_rate_amt = value;
			}
		}

		private decimal _supplemental_coverage_amt;
		public decimal supplemental_coverage_amt
		{
			get
			{
				return _supplemental_coverage_amt;
			}

			set
			{
				_supplemental_coverage_amt = value;
			}
		}

		private decimal _supplemental_premium_amt;
		public decimal supplemental_premium_amt
		{
			get
			{
				return _supplemental_premium_amt;
			}

			set
			{
				_supplemental_premium_amt = value;
			}
		}

		private decimal _ad_and_d_supplemental_premium_rate;
		public decimal ad_and_d_supplemental_premium_rate
		{
			get
			{
				return _ad_and_d_supplemental_premium_rate;
			}

			set
			{
				_ad_and_d_supplemental_premium_rate = value;
			}
		}

		private string _client_description;
		public string client_description
		{
			get
			{
				return _client_description;
			}

			set
			{
				_client_description = value;
			}
		}

    }
}

