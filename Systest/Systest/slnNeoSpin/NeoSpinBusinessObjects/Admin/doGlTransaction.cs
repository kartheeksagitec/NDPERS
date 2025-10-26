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
    public class doGlTransaction : doBase
    {
         public doGlTransaction() : base()
         {
         }
		private int _gl_transaction_id;
		public int gl_transaction_id
		{
			get
			{
				return _gl_transaction_id;
			}

			set
			{
				_gl_transaction_id = value;
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

		private int _account_id;
		public int account_id
		{
			get
			{
				return _account_id;
			}

			set
			{
				_account_id = value;
			}
		}

		private int _org_id;
		public int org_id
		{
			get
			{
				return _org_id;
			}

			set
			{
				_org_id = value;
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

		private int _source_id;
		public int source_id
		{
			get
			{
				return _source_id;
			}

			set
			{
				_source_id = value;
			}
		}

		private decimal _debit_amount;
		public decimal debit_amount
		{
			get
			{
				return _debit_amount;
			}

			set
			{
				_debit_amount = value;
			}
		}

		private decimal _credit_amount;
		public decimal credit_amount
		{
			get
			{
				return _credit_amount;
			}

			set
			{
				_credit_amount = value;
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

		private DateTime _posting_date;
		public DateTime posting_date
		{
			get
			{
				return _posting_date;
			}

			set
			{
				_posting_date = value;
			}
		}

		private DateTime _extract_date;
		public DateTime extract_date
		{
			get
			{
				return _extract_date;
			}

			set
			{
				_extract_date = value;
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

		private string _extract_file_name;
		public string extract_file_name
		{
			get
			{
				return _extract_file_name;
			}

			set
			{
				_extract_file_name = value;
			}
		}

    }
}

