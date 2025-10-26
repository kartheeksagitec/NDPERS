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
    public class doServiceCreditPlanFormulaRef : doBase
    {
         
         public doServiceCreditPlanFormulaRef() : base()
         {
         }
		private int _service_credit_plan_formula_ref_id;
		public int service_credit_plan_formula_ref_id
		{
			get
			{
				return _service_credit_plan_formula_ref_id;
			}

			set
			{
				_service_credit_plan_formula_ref_id = value;
			}
		}

		private int _service_credit_type_id;
		public int service_credit_type_id
		{
			get
			{
				return _service_credit_type_id;
			}

			set
			{
				_service_credit_type_id = value;
			}
		}

		private string _service_credit_type_description;
		public string service_credit_type_description
		{
			get
			{
				return _service_credit_type_description;
			}

			set
			{
				_service_credit_type_description = value;
			}
		}

		private string _service_credit_type_value;
		public string service_credit_type_value
		{
			get
			{
				return _service_credit_type_value;
			}

			set
			{
				_service_credit_type_value = value;
			}
		}

		private int _plan_id;
		public int plan_id
		{
			get
			{
				return _plan_id;
			}

			set
			{
				_plan_id = value;
			}
		}

		private int _payor_id;
		public int payor_id
		{
			get
			{
				return _payor_id;
			}

			set
			{
				_payor_id = value;
			}
		}

		private string _payor_description;
		public string payor_description
		{
			get
			{
				return _payor_description;
			}

			set
			{
				_payor_description = value;
			}
		}

		private string _payor_value;
		public string payor_value
		{
			get
			{
				return _payor_value;
			}

			set
			{
				_payor_value = value;
			}
		}

		private int _age_for_enra_calculation;
		public int age_for_enra_calculation
		{
			get
			{
				return _age_for_enra_calculation;
			}

			set
			{
				_age_for_enra_calculation = value;
			}
		}

		private int _age_ceiling_for_enra;
		public int age_ceiling_for_enra
		{
			get
			{
				return _age_ceiling_for_enra;
			}

			set
			{
				_age_ceiling_for_enra = value;
			}
		}

		private int _actuarial_table_reference_id;
		public int actuarial_table_reference_id
		{
			get
			{
				return _actuarial_table_reference_id;
			}

			set
			{
				_actuarial_table_reference_id = value;
			}
		}

		private string _actuarial_table_reference_description;
		public string actuarial_table_reference_description
		{
			get
			{
				return _actuarial_table_reference_description;
			}

			set
			{
				_actuarial_table_reference_description = value;
			}
		}

		private string _actuarial_table_reference_value;
		public string actuarial_table_reference_value
		{
			get
			{
				return _actuarial_table_reference_value;
			}

			set
			{
				_actuarial_table_reference_value = value;
			}
		}

		private string _applicable_to_pension_flag;
		public string applicable_to_pension_flag
		{
			get
			{
				return _applicable_to_pension_flag;
			}

			set
			{
				_applicable_to_pension_flag = value;
			}
		}

		private string _applicable_to_vesting_flag;
		public string applicable_to_vesting_flag
		{
			get
			{
				return _applicable_to_vesting_flag;
			}

			set
			{
				_applicable_to_vesting_flag = value;
			}
		}

    }
}

