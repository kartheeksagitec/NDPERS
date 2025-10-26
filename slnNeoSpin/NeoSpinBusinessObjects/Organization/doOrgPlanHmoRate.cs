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
    public class doOrgPlanHmoRate : doBase
    {
         
         public doOrgPlanHmoRate() : base()
         {
         }
		private int _org_plan_hmo_rate_id;
		public int org_plan_hmo_rate_id
		{
			get
			{
				return _org_plan_hmo_rate_id;
			}

			set
			{
				_org_plan_hmo_rate_id = value;
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

		private int _hmo_insurance_type_id;
		public int hmo_insurance_type_id
		{
			get
			{
				return _hmo_insurance_type_id;
			}

			set
			{
				_hmo_insurance_type_id = value;
			}
		}

		private string _hmo_insurance_type_description;
		public string hmo_insurance_type_description
		{
			get
			{
				return _hmo_insurance_type_description;
			}

			set
			{
				_hmo_insurance_type_description = value;
			}
		}

		private string _hmo_insurance_type_value;
		public string hmo_insurance_type_value
		{
			get
			{
				return _hmo_insurance_type_value;
			}

			set
			{
				_hmo_insurance_type_value = value;
			}
		}

		private int _emp_category_id;
		public int emp_category_id
		{
			get
			{
				return _emp_category_id;
			}

			set
			{
				_emp_category_id = value;
			}
		}

		private string _emp_category_description;
		public string emp_category_description
		{
			get
			{
				return _emp_category_description;
			}

			set
			{
				_emp_category_description = value;
			}
		}

		private string _emp_category_value;
		public string emp_category_value
		{
			get
			{
				return _emp_category_value;
			}

			set
			{
				_emp_category_value = value;
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

