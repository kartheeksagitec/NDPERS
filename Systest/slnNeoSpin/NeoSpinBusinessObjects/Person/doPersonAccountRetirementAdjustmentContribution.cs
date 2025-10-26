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
    public class doPersonAccountRetirementAdjustmentContribution : doBase
    {
         
         public doPersonAccountRetirementAdjustmentContribution() : base()
         {
         }
		private int _retirement_adjustment_contribution_id;
		public int retirement_adjustment_contribution_id
		{
			get
			{
				return _retirement_adjustment_contribution_id;
			}

			set
			{
				_retirement_adjustment_contribution_id = value;
			}
		}

		private int _person_account_adjustment_id;
		public int person_account_adjustment_id
		{
			get
			{
				return _person_account_adjustment_id;
			}

			set
			{
				_person_account_adjustment_id = value;
			}
		}

		private int _subsystem_id;
		public int subsystem_id
		{
			get
			{
				return _subsystem_id;
			}

			set
			{
				_subsystem_id = value;
			}
		}

		private string _subsystem_description;
		public string subsystem_description
		{
			get
			{
				return _subsystem_description;
			}

			set
			{
				_subsystem_description = value;
			}
		}

		private string _subsystem_value;
		public string subsystem_value
		{
			get
			{
				return _subsystem_value;
			}

			set
			{
				_subsystem_value = value;
			}
		}

		private int _subsystem_ref_id;
		public int subsystem_ref_id
		{
			get
			{
				return _subsystem_ref_id;
			}

			set
			{
				_subsystem_ref_id = value;
			}
		}

		private DateTime _transaction_date;
		public DateTime transaction_date
		{
			get
			{
				return _transaction_date;
			}

			set
			{
				_transaction_date = value;
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

		private int _pay_period_month;
		public int pay_period_month
		{
			get
			{
				return _pay_period_month;
			}

			set
			{
				_pay_period_month = value;
			}
		}

		private int _pay_period_year;
		public int pay_period_year
		{
			get
			{
				return _pay_period_year;
			}

			set
			{
				_pay_period_year = value;
			}
		}

		private int _person_employment_dtl_id;
		public int person_employment_dtl_id
		{
			get
			{
				return _person_employment_dtl_id;
			}

			set
			{
				_person_employment_dtl_id = value;
			}
		}

		private int _transaction_type_id;
		public int transaction_type_id
		{
			get
			{
				return _transaction_type_id;
			}

			set
			{
				_transaction_type_id = value;
			}
		}

		private string _transaction_type_description;
		public string transaction_type_description
		{
			get
			{
				return _transaction_type_description;
			}

			set
			{
				_transaction_type_description = value;
			}
		}

		private string _transaction_type_value;
		public string transaction_type_value
		{
			get
			{
				return _transaction_type_value;
			}

			set
			{
				_transaction_type_value = value;
			}
		}

		private decimal _salary_amount;
		public decimal salary_amount
		{
			get
			{
				return _salary_amount;
			}

			set
			{
				_salary_amount = value;
			}
		}

		private decimal _post_tax_er_amount;
		public decimal post_tax_er_amount
		{
			get
			{
				return _post_tax_er_amount;
			}

			set
			{
				_post_tax_er_amount = value;
			}
		}

		private decimal _post_tax_ee_amount;
		public decimal post_tax_ee_amount
		{
			get
			{
				return _post_tax_ee_amount;
			}

			set
			{
				_post_tax_ee_amount = value;
			}
		}

		private decimal _pre_tax_er_amount;
		public decimal pre_tax_er_amount
		{
			get
			{
				return _pre_tax_er_amount;
			}

			set
			{
				_pre_tax_er_amount = value;
			}
		}

		private decimal _pre_tax_ee_amount;
		public decimal pre_tax_ee_amount
		{
			get
			{
				return _pre_tax_ee_amount;
			}

			set
			{
				_pre_tax_ee_amount = value;
			}
		}

		private decimal _ee_rhic_amount;
		public decimal ee_rhic_amount
		{
			get
			{
				return _ee_rhic_amount;
			}

			set
			{
				_ee_rhic_amount = value;
			}
		}

		private decimal _er_rhic_amount;
		public decimal er_rhic_amount
		{
			get
			{
				return _er_rhic_amount;
			}

			set
			{
				_er_rhic_amount = value;
			}
		}

		private decimal _ee_er_pickup_amount;
		public decimal ee_er_pickup_amount
		{
			get
			{
				return _ee_er_pickup_amount;
			}

			set
			{
				_ee_er_pickup_amount = value;
			}
		}

		private decimal _er_vested_amount;
		public decimal er_vested_amount
		{
			get
			{
				return _er_vested_amount;
			}

			set
			{
				_er_vested_amount = value;
			}
		}

		private decimal _interest_amount;
		public decimal interest_amount
		{
			get
			{
				return _interest_amount;
			}

			set
			{
				_interest_amount = value;
			}
		}

		private decimal _vested_service_credit;
		public decimal vested_service_credit
		{
			get
			{
				return _vested_service_credit;
			}

			set
			{
				_vested_service_credit = value;
			}
		}

		private decimal _pension_service_credit;
		public decimal pension_service_credit
		{
			get
			{
				return _pension_service_credit;
			}

			set
			{
				_pension_service_credit = value;
			}
		}

		private int _retirement_contribution_id;
		public int retirement_contribution_id
		{
			get
			{
				return _retirement_contribution_id;
			}

			set
			{
				_retirement_contribution_id = value;
			}
		}

    }
}

