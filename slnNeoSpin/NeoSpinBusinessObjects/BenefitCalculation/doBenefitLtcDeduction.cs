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
    public class doBenefitLtcDeduction : doBase
    {
         public doBenefitLtcDeduction() : base()
         {
         }
		private int _benefit_ltc_deduction_id;
		public int benefit_ltc_deduction_id
		{
			get
			{
				return _benefit_ltc_deduction_id;
			}

			set
			{
				_benefit_ltc_deduction_id = value;
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

		private int _ltc_relationship_id;
		public int ltc_relationship_id
		{
			get
			{
				return _ltc_relationship_id;
			}

			set
			{
				_ltc_relationship_id = value;
			}
		}

		private string _ltc_relationship_description;
		public string ltc_relationship_description
		{
			get
			{
				return _ltc_relationship_description;
			}

			set
			{
				_ltc_relationship_description = value;
			}
		}

		private string _ltc_relationship_value;
		public string ltc_relationship_value
		{
			get
			{
				return _ltc_relationship_value;
			}

			set
			{
				_ltc_relationship_value = value;
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

    }
}

