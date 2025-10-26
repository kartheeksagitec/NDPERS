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
    public class doOrgPlanLtcRate : doBase
    {
         
         public doOrgPlanLtcRate() : base()
         {
         }
		private int _org_plan_ltc_rate_id;
		public int org_plan_ltc_rate_id
		{
			get
			{
				return _org_plan_ltc_rate_id;
			}

			set
			{
				_org_plan_ltc_rate_id = value;
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

		private int _ltc_insurance_type_id;
		public int ltc_insurance_type_id
		{
			get
			{
				return _ltc_insurance_type_id;
			}

			set
			{
				_ltc_insurance_type_id = value;
			}
		}

		private string _ltc_insurance_type_description;
		public string ltc_insurance_type_description
		{
			get
			{
				return _ltc_insurance_type_description;
			}

			set
			{
				_ltc_insurance_type_description = value;
			}
		}

		private string _ltc_insurance_type_value;
		public string ltc_insurance_type_value
		{
			get
			{
				return _ltc_insurance_type_value;
			}

			set
			{
				_ltc_insurance_type_value = value;
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

		private decimal _premium_amt;
		public decimal premium_amt
		{
			get
			{
				return _premium_amt;
			}

			set
			{
				_premium_amt = value;
			}
		}

    }
}

