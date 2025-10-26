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
    public class doAccountReference : doBase
    {
         public doAccountReference() : base()
         {
         }
		private int _account_reference_id;
		public int account_reference_id
		{
			get
			{
				return _account_reference_id;
			}

			set
			{
				_account_reference_id = value;
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

		private int _fund_id;
		public int fund_id
		{
			get
			{
				return _fund_id;
			}

			set
			{
				_fund_id = value;
			}
		}

		private string _fund_description;
		public string fund_description
		{
			get
			{
				return _fund_description;
			}

			set
			{
				_fund_description = value;
			}
		}

		private string _fund_value;
		public string fund_value
		{
			get
			{
				return _fund_value;
			}

			set
			{
				_fund_value = value;
			}
		}

		private int _dept_id;
		public int dept_id
		{
			get
			{
				return _dept_id;
			}

			set
			{
				_dept_id = value;
			}
		}

		private string _dept_description;
		public string dept_description
		{
			get
			{
				return _dept_description;
			}

			set
			{
				_dept_description = value;
			}
		}

		private string _dept_value;
		public string dept_value
		{
			get
			{
				return _dept_value;
			}

			set
			{
				_dept_value = value;
			}
		}

		private int _item_type_id;
		public int item_type_id
		{
			get
			{
				return _item_type_id;
			}

			set
			{
				_item_type_id = value;
			}
		}

		private string _item_type_description;
		public string item_type_description
		{
			get
			{
				return _item_type_description;
			}

			set
			{
				_item_type_description = value;
			}
		}

		private string _item_type_value;
		public string item_type_value
		{
			get
			{
				return _item_type_value;
			}

			set
			{
				_item_type_value = value;
			}
		}

		private int _from_item_type_id;
		public int from_item_type_id
		{
			get
			{
				return _from_item_type_id;
			}

			set
			{
				_from_item_type_id = value;
			}
		}

		private string _from_item_type_description;
		public string from_item_type_description
		{
			get
			{
				return _from_item_type_description;
			}

			set
			{
				_from_item_type_description = value;
			}
		}

		private string _from_item_type_value;
		public string from_item_type_value
		{
			get
			{
				return _from_item_type_value;
			}

			set
			{
				_from_item_type_value = value;
			}
		}

		private int _to_item_type_id;
		public int to_item_type_id
		{
			get
			{
				return _to_item_type_id;
			}

			set
			{
				_to_item_type_id = value;
			}
		}

		private string _to_item_type_description;
		public string to_item_type_description
		{
			get
			{
				return _to_item_type_description;
			}

			set
			{
				_to_item_type_description = value;
			}
		}

		private string _to_item_type_value;
		public string to_item_type_value
		{
			get
			{
				return _to_item_type_value;
			}

			set
			{
				_to_item_type_value = value;
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

		private int _status_transition_id;
		public int status_transition_id
		{
			get
			{
				return _status_transition_id;
			}

			set
			{
				_status_transition_id = value;
			}
		}

		private string _status_transition_description;
		public string status_transition_description
		{
			get
			{
				return _status_transition_description;
			}

			set
			{
				_status_transition_description = value;
			}
		}

		private string _status_transition_value;
		public string status_transition_value
		{
			get
			{
				return _status_transition_value;
			}

			set
			{
				_status_transition_value = value;
			}
		}

		private int _source_type_id;
		public int source_type_id
		{
			get
			{
				return _source_type_id;
			}

			set
			{
				_source_type_id = value;
			}
		}

		private string _source_type_description;
		public string source_type_description
		{
			get
			{
				return _source_type_description;
			}

			set
			{
				_source_type_description = value;
			}
		}

		private string _source_type_value;
		public string source_type_value
		{
			get
			{
				return _source_type_value;
			}

			set
			{
				_source_type_value = value;
			}
		}

		private string _generate_gl_flag;
		public string generate_gl_flag
		{
			get
			{
				return _generate_gl_flag;
			}

			set
			{
				_generate_gl_flag = value;
			}
		}

		private int _debit_account_id;
		public int debit_account_id
		{
			get
			{
				return _debit_account_id;
			}

			set
			{
				_debit_account_id = value;
			}
		}

		private int _credit_account_id;
		public int credit_account_id
		{
			get
			{
				return _credit_account_id;
			}

			set
			{
				_credit_account_id = value;
			}
		}

		private string _journal_description;
		public string journal_description
		{
			get
			{
				return _journal_description;
			}

			set
			{
				_journal_description = value;
			}
		}

    }
}

