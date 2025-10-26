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
    public class doBenefitPayeeTaxWithholding : doBase
    {
         
         public doBenefitPayeeTaxWithholding() : base()
         {
         }
		private int _benefit_payee_tax_withholding_id;
		public int benefit_payee_tax_withholding_id
		{
			get
			{
				return _benefit_payee_tax_withholding_id;
			}

			set
			{
				_benefit_payee_tax_withholding_id = value;
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

		private int _tax_identifier_id;
		public int tax_identifier_id
		{
			get
			{
				return _tax_identifier_id;
			}

			set
			{
				_tax_identifier_id = value;
			}
		}

		private string _tax_identifier_description;
		public string tax_identifier_description
		{
			get
			{
				return _tax_identifier_description;
			}

			set
			{
				_tax_identifier_description = value;
			}
		}

		private string _tax_identifier_value;
		public string tax_identifier_value
		{
			get
			{
				return _tax_identifier_value;
			}

			set
			{
				_tax_identifier_value = value;
			}
		}

		private int _benefit_distribution_type_id;
		public int benefit_distribution_type_id
		{
			get
			{
				return _benefit_distribution_type_id;
			}

			set
			{
				_benefit_distribution_type_id = value;
			}
		}

		private string _benefit_distribution_type_description;
		public string benefit_distribution_type_description
		{
			get
			{
				return _benefit_distribution_type_description;
			}

			set
			{
				_benefit_distribution_type_description = value;
			}
		}

		private string _benefit_distribution_type_value;
		public string benefit_distribution_type_value
		{
			get
			{
				return _benefit_distribution_type_value;
			}

			set
			{
				_benefit_distribution_type_value = value;
			}
		}

		private int _tax_option_id;
		public int tax_option_id
		{
			get
			{
				return _tax_option_id;
			}

			set
			{
				_tax_option_id = value;
			}
		}

		private string _tax_option_description;
		public string tax_option_description
		{
			get
			{
				return _tax_option_description;
			}

			set
			{
				_tax_option_description = value;
			}
		}

		private string _tax_option_value;
		public string tax_option_value
		{
			get
			{
				return _tax_option_value;
			}

			set
			{
				_tax_option_value = value;
			}
		}

		private int _tax_allowance;
		public int tax_allowance
		{
			get
			{
				return _tax_allowance;
			}

			set
			{
				_tax_allowance = value;
			}
		}

		private int _marital_status_id;
		public int marital_status_id
		{
			get
			{
				return _marital_status_id;
			}

			set
			{
				_marital_status_id = value;
			}
		}

		private string _marital_status_description;
		public string marital_status_description
		{
			get
			{
				return _marital_status_description;
			}

			set
			{
				_marital_status_description = value;
			}
		}

		private string _marital_status_value;
		public string marital_status_value
		{
			get
			{
				return _marital_status_value;
			}

			set
			{
				_marital_status_value = value;
			}
		}

		private decimal _additional_tax_amount;
		public decimal additional_tax_amount
		{
			get
			{
				return _additional_tax_amount;
			}

			set
			{
				_additional_tax_amount = value;
			}
		}

		private decimal _computed_tax_amount;
		public decimal computed_tax_amount
		{
			get
			{
				return _computed_tax_amount;
			}

			set
			{
				_computed_tax_amount = value;
			}
		}

    }
}

