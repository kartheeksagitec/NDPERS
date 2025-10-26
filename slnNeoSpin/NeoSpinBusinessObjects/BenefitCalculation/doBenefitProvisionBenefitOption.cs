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
    public class doBenefitProvisionBenefitOption : doBase
    {
         
         public doBenefitProvisionBenefitOption() : base()
         {
         }
		private int _benefit_provision_benefit_option_id;
		public int benefit_provision_benefit_option_id
		{
			get
			{
				return _benefit_provision_benefit_option_id;
			}

			set
			{
				_benefit_provision_benefit_option_id = value;
			}
		}

		private int _benefit_provision_id;
		public int benefit_provision_id
		{
			get
			{
				return _benefit_provision_id;
			}

			set
			{
				_benefit_provision_id = value;
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

		private int _benefit_account_type_id;
		public int benefit_account_type_id
		{
			get
			{
				return _benefit_account_type_id;
			}

			set
			{
				_benefit_account_type_id = value;
			}
		}

		private string _benefit_account_type_description;
		public string benefit_account_type_description
		{
			get
			{
				return _benefit_account_type_description;
			}

			set
			{
				_benefit_account_type_description = value;
			}
		}

		private string _benefit_account_type_value;
		public string benefit_account_type_value
		{
			get
			{
				return _benefit_account_type_value;
			}

			set
			{
				_benefit_account_type_value = value;
			}
		}

		private int _benefit_option_id;
		public int benefit_option_id
		{
			get
			{
				return _benefit_option_id;
			}

			set
			{
				_benefit_option_id = value;
			}
		}

		private string _benefit_option_description;
		public string benefit_option_description
		{
			get
			{
				return _benefit_option_description;
			}

			set
			{
				_benefit_option_description = value;
			}
		}

		private string _benefit_option_value;
		public string benefit_option_value
		{
			get
			{
				return _benefit_option_value;
			}

			set
			{
				_benefit_option_value = value;
			}
		}

		private int _factor_method_id;
		public int factor_method_id
		{
			get
			{
				return _factor_method_id;
			}

			set
			{
				_factor_method_id = value;
			}
		}

		private string _factor_method_description;
		public string factor_method_description
		{
			get
			{
				return _factor_method_description;
			}

			set
			{
				_factor_method_description = value;
			}
		}

		private string _factor_method_value;
		public string factor_method_value
		{
			get
			{
				return _factor_method_value;
			}

			set
			{
				_factor_method_value = value;
			}
		}

		private decimal _option_factor;
		public decimal option_factor
		{
			get
			{
				return _option_factor;
			}

			set
			{
				_option_factor = value;
			}
		}

		private decimal _spouse_factor;
		public decimal spouse_factor
		{
			get
			{
				return _spouse_factor;
			}

			set
			{
				_spouse_factor = value;
			}
		}

		private string _ssli_flag;
		public string ssli_flag
		{
			get
			{
				return _ssli_flag;
			}

			set
			{
				_ssli_flag = value;
			}
		}

		private int _ssli_factor_method_id;
		public int ssli_factor_method_id
		{
			get
			{
				return _ssli_factor_method_id;
			}

			set
			{
				_ssli_factor_method_id = value;
			}
		}

		private string _ssli_factor_method_description;
		public string ssli_factor_method_description
		{
			get
			{
				return _ssli_factor_method_description;
			}

			set
			{
				_ssli_factor_method_description = value;
			}
		}

		private string _ssli_factor_method_value;
		public string ssli_factor_method_value
		{
			get
			{
				return _ssli_factor_method_value;
			}

			set
			{
				_ssli_factor_method_value = value;
			}
		}

		private decimal _ssli_factor;
		public decimal ssli_factor
		{
			get
			{
				return _ssli_factor;
			}

			set
			{
				_ssli_factor = value;
			}
		}

		private int _exclusive_calc_payment_type_id;
		public int exclusive_calc_payment_type_id
		{
			get
			{
				return _exclusive_calc_payment_type_id;
			}

			set
			{
				_exclusive_calc_payment_type_id = value;
			}
		}

		private string _exclusive_calc_payment_type_description;
		public string exclusive_calc_payment_type_description
		{
			get
			{
				return _exclusive_calc_payment_type_description;
			}

			set
			{
				_exclusive_calc_payment_type_description = value;
			}
		}

		private string _exclusive_calc_payment_type_value;
		public string exclusive_calc_payment_type_value
		{
			get
			{
				return _exclusive_calc_payment_type_value;
			}

			set
			{
				_exclusive_calc_payment_type_value = value;
			}
		}

    }
}

