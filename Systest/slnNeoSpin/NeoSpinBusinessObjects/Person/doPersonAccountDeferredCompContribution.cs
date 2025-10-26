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
    public class doPersonAccountDeferredCompContribution : doBase
    {
         
         public doPersonAccountDeferredCompContribution() : base()
         {
         }
		private int _deferred_comp_contribution_id;
		public int deferred_comp_contribution_id
		{
			get
			{
				return _deferred_comp_contribution_id;
			}

			set
			{
				_deferred_comp_contribution_id = value;
			}
		}

		private int _person_account_id;
		public int person_account_id
		{
			get
			{
				return _person_account_id;
			}

			set
			{
				_person_account_id = value;
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

		private DateTime _pay_period_start_date;
		public DateTime pay_period_start_date
		{
			get
			{
				return _pay_period_start_date;
			}

			set
			{
				_pay_period_start_date = value;
			}
		}

		private DateTime _pay_period_end_date;
		public DateTime pay_period_end_date
		{
			get
			{
				return _pay_period_end_date;
			}

			set
			{
				_pay_period_end_date = value;
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

		private DateTime _paid_date;
		public DateTime paid_date
		{
			get
			{
				return _paid_date;
			}

			set
			{
				_paid_date = value;
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

		private int _provider_org_id;
		public int provider_org_id
		{
			get
			{
				return _provider_org_id;
			}

			set
			{
				_provider_org_id = value;
			}
		}

		private decimal _pay_period_contribution_amount;
		public decimal pay_period_contribution_amount
		{
			get
			{
				return _pay_period_contribution_amount;
			}

			set
			{
				_pay_period_contribution_amount = value;
			}
		}
        public decimal employer_match_amount { get; set; }		//PIR 25920 New Plan DC 2025

    }
}

