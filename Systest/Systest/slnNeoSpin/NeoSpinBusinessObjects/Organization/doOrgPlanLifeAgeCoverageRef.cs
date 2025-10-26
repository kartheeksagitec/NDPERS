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
    public class doOrgPlanLifeAgeCoverageRef : doBase
    {
         
         public doOrgPlanLifeAgeCoverageRef() : base()
         {
         }
		private int _org_plan_life_age_coverage_ref_id;
		public int org_plan_life_age_coverage_ref_id
		{
			get
			{
				return _org_plan_life_age_coverage_ref_id;
			}

			set
			{
				_org_plan_life_age_coverage_ref_id = value;
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

		private decimal _full_coverage_amt;
		public decimal full_coverage_amt
		{
			get
			{
				return _full_coverage_amt;
			}

			set
			{
				_full_coverage_amt = value;
			}
		}

    }
}

