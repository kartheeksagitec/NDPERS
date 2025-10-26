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
    public class doBenefitGhdvDeduction : doBase
    {
         public doBenefitGhdvDeduction() : base()
         {
         }
		private int _benefit_ghdv_deduction_id;
		public int benefit_ghdv_deduction_id
		{
			get
			{
				return _benefit_ghdv_deduction_id;
			}

			set
			{
				_benefit_ghdv_deduction_id = value;
			}
		}

		private int _benefit_calculation_id;
		public int benefit_calculation_id
		{
			get
			{
				return _benefit_calculation_id;
			}

			set
			{
				_benefit_calculation_id = value;
			}
		}

		private int _person_id;
		public int person_id
		{
			get
			{
				return _person_id;
			}

			set
			{
				_person_id = value;
			}
		}

		private int _health_insurance_type_id;
		public int health_insurance_type_id
		{
			get
			{
				return _health_insurance_type_id;
			}

			set
			{
				_health_insurance_type_id = value;
			}
		}

		private string _health_insurance_type_description;
		public string health_insurance_type_description
		{
			get
			{
				return _health_insurance_type_description;
			}

			set
			{
				_health_insurance_type_description = value;
			}
		}

		private string _health_insurance_type_value;
		public string health_insurance_type_value
		{
			get
			{
				return _health_insurance_type_value;
			}

			set
			{
				_health_insurance_type_value = value;
			}
		}

		private int _dental_insurance_type_id;
		public int dental_insurance_type_id
		{
			get
			{
				return _dental_insurance_type_id;
			}

			set
			{
				_dental_insurance_type_id = value;
			}
		}

		private string _dental_insurance_type_description;
		public string dental_insurance_type_description
		{
			get
			{
				return _dental_insurance_type_description;
			}

			set
			{
				_dental_insurance_type_description = value;
			}
		}

		private string _dental_insurance_type_value;
		public string dental_insurance_type_value
		{
			get
			{
				return _dental_insurance_type_value;
			}

			set
			{
				_dental_insurance_type_value = value;
			}
		}

		private int _vision_insurance_type_id;
		public int vision_insurance_type_id
		{
			get
			{
				return _vision_insurance_type_id;
			}

			set
			{
				_vision_insurance_type_id = value;
			}
		}

		private string _vision_insurance_type_description;
		public string vision_insurance_type_description
		{
			get
			{
				return _vision_insurance_type_description;
			}

			set
			{
				_vision_insurance_type_description = value;
			}
		}

		private string _vision_insurance_type_value;
		public string vision_insurance_type_value
		{
			get
			{
				return _vision_insurance_type_value;
			}

			set
			{
				_vision_insurance_type_value = value;
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

		private string _coverage_code;
		public string coverage_code
		{
			get
			{
				return _coverage_code;
			}

			set
			{
				_coverage_code = value;
			}
		}

		private int _plan_option_id;
		public int plan_option_id
		{
			get
			{
				return _plan_option_id;
			}

			set
			{
				_plan_option_id = value;
			}
		}

		private string _plan_option_description;
		public string plan_option_description
		{
			get
			{
				return _plan_option_description;
			}

			set
			{
				_plan_option_description = value;
			}
		}

		private string _plan_option_value;
		public string plan_option_value
		{
			get
			{
				return _plan_option_value;
			}

			set
			{
				_plan_option_value = value;
			}
		}

		private int _cobra_type_id;
		public int cobra_type_id
		{
			get
			{
				return _cobra_type_id;
			}

			set
			{
				_cobra_type_id = value;
			}
		}

		private string _cobra_type_description;
		public string cobra_type_description
		{
			get
			{
				return _cobra_type_description;
			}

			set
			{
				_cobra_type_description = value;
			}
		}

		private string _cobra_type_value;
		public string cobra_type_value
		{
			get
			{
				return _cobra_type_value;
			}

			set
			{
				_cobra_type_value = value;
			}
		}

		private decimal _low_income_credit;
		public decimal low_income_credit
		{
			get
			{
				return _low_income_credit;
			}

			set
			{
				_low_income_credit = value;
			}
		}

		private decimal _computed_premium_amount;
		public decimal computed_premium_amount
		{
			get
			{
				return _computed_premium_amount;
			}

			set
			{
				_computed_premium_amount = value;
			}
		}

		private int _benefit_deduction_ghdv_id;
		public int benefit_deduction_ghdv_id
		{
			get
			{
				return _benefit_deduction_ghdv_id;
			}

			set
			{
				_benefit_deduction_ghdv_id = value;
			}
		}

		private string _benefit_deduction_ghdv_description;
		public string benefit_deduction_ghdv_description
		{
			get
			{
				return _benefit_deduction_ghdv_description;
			}

			set
			{
				_benefit_deduction_ghdv_description = value;
			}
		}

		private string _benefit_deduction_ghdv_value;
		public string benefit_deduction_ghdv_value
		{
			get
			{
				return _benefit_deduction_ghdv_value;
			}

			set
			{
				_benefit_deduction_ghdv_value = value;
			}
		}

    }
}

