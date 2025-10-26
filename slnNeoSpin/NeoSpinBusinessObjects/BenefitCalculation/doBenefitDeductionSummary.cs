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
    public class doBenefitDeductionSummary : doBase
    {
         public doBenefitDeductionSummary() : base()
         {
         }
		private int _benefit_deduction_summary_id;
		public int benefit_deduction_summary_id
		{
			get
			{
				return _benefit_deduction_summary_id;
			}

			set
			{
				_benefit_deduction_summary_id = value;
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

		private decimal _gross_monthly_benefit_amount;
		public decimal gross_monthly_benefit_amount
		{
			get
			{
				return _gross_monthly_benefit_amount;
			}

			set
			{
				_gross_monthly_benefit_amount = value;
			}
		}

		private decimal _fit_overridden_amount;
		public decimal fit_overridden_amount
		{
			get
			{
				return _fit_overridden_amount;
			}

			set
			{
				_fit_overridden_amount = value;
			}
		}

		private decimal _ndit_overridden_amount;
		public decimal ndit_overridden_amount
		{
			get
			{
				return _ndit_overridden_amount;
			}

			set
			{
				_ndit_overridden_amount = value;
			}
		}

		private decimal _health_overridden_amount;
		public decimal health_overridden_amount
		{
			get
			{
				return _health_overridden_amount;
			}

			set
			{
				_health_overridden_amount = value;
			}
		}

		private decimal _rhic_overridden_amount;
		public decimal rhic_overridden_amount
		{
			get
			{
				return _rhic_overridden_amount;
			}

			set
			{
				_rhic_overridden_amount = value;
			}
		}

		private decimal _net_health_insurance_premium_amount;
		public decimal net_health_insurance_premium_amount
		{
			get
			{
				return _net_health_insurance_premium_amount;
			}

			set
			{
				_net_health_insurance_premium_amount = value;
			}
		}

		private decimal _dental_overridden_amount;
		public decimal dental_overridden_amount
		{
			get
			{
				return _dental_overridden_amount;
			}

			set
			{
				_dental_overridden_amount = value;
			}
		}

		private decimal _vision_overridden_amount;
		public decimal vision_overridden_amount
		{
			get
			{
				return _vision_overridden_amount;
			}

			set
			{
				_vision_overridden_amount = value;
			}
		}

		private decimal _life_overridden_amount;
		public decimal life_overridden_amount
		{
			get
			{
				return _life_overridden_amount;
			}

			set
			{
				_life_overridden_amount = value;
			}
		}

		private decimal _ltc_overridden_amount;
		public decimal ltc_overridden_amount
		{
			get
			{
				return _ltc_overridden_amount;
			}

			set
			{
				_ltc_overridden_amount = value;
			}
		}

		private decimal _miscellaneous_deduction_amount;
		public decimal miscellaneous_deduction_amount
		{
			get
			{
				return _miscellaneous_deduction_amount;
			}

			set
			{
				_miscellaneous_deduction_amount = value;
			}
		}

		private string _miscellaneous_deduction_comments;
		public string miscellaneous_deduction_comments
		{
			get
			{
				return _miscellaneous_deduction_comments;
			}

			set
			{
				_miscellaneous_deduction_comments = value;
			}
		}

		private decimal _net_monthly_pension_benefit_amount;
		public decimal net_monthly_pension_benefit_amount
		{
			get
			{
				return _net_monthly_pension_benefit_amount;
			}

			set
			{
				_net_monthly_pension_benefit_amount = value;
			}
		}
        private int _medicare;
        public int medicare
        {
            get
            {
                return _medicare;
            }

            set
            {
                _medicare = value;
            }
        }

    }
}

