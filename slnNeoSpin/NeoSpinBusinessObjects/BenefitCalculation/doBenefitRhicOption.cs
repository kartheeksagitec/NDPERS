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
    public class doBenefitRhicOption : doBase
    {
         
         public doBenefitRhicOption() : base()
         {
         }
		private int _benefit_rhic_option_id;
		public int benefit_rhic_option_id
		{
			get
			{
				return _benefit_rhic_option_id;
			}

			set
			{
				_benefit_rhic_option_id = value;
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

		private int _rhic_option_id;
		public int rhic_option_id
		{
			get
			{
				return _rhic_option_id;
			}

			set
			{
				_rhic_option_id = value;
			}
		}

		private string _rhic_option_description;
		public string rhic_option_description
		{
			get
			{
				return _rhic_option_description;
			}

			set
			{
				_rhic_option_description = value;
			}
		}

		private string _rhic_option_value;
		public string rhic_option_value
		{
			get
			{
				return _rhic_option_value;
			}

			set
			{
				_rhic_option_value = value;
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

		private decimal _member_rhic_amount;
		public decimal member_rhic_amount
		{
			get
			{
				return _member_rhic_amount;
			}

			set
			{
				_member_rhic_amount = value;
			}
		}

		private decimal _spouse_rhic_percentage;
		public decimal spouse_rhic_percentage
		{
			get
			{
				return _spouse_rhic_percentage;
			}

			set
			{
				_spouse_rhic_percentage = value;
			}
		}

		private decimal _spouse_rhic_amount;
		public decimal spouse_rhic_amount
		{
			get
			{
				return _spouse_rhic_amount;
			}

			set
			{
				_spouse_rhic_amount = value;
			}
		}

    }
}

