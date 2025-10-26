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
    public class doDepositTape : doBase
    {
         
         public doDepositTape() : base()
         {
         }
		private int _deposit_tape_id;
		public int deposit_tape_id
		{
			get
			{
				return _deposit_tape_id;
			}

			set
			{
				_deposit_tape_id = value;
			}
		}

		private DateTime _deposit_date;
		public DateTime deposit_date
		{
			get
			{
				return _deposit_date;
			}

			set
			{
				_deposit_date = value;
			}
		}

		private int _status_id;
		public int status_id
		{
			get
			{
				return _status_id;
			}

			set
			{
				_status_id = value;
			}
		}

		private string _status_description;
		public string status_description
		{
			get
			{
				return _status_description;
			}

			set
			{
				_status_description = value;
			}
		}

		private string _status_value;
		public string status_value
		{
			get
			{
				return _status_value;
			}

			set
			{
				_status_value = value;
			}
		}

		private int _deposit_method_id;
		public int deposit_method_id
		{
			get
			{
				return _deposit_method_id;
			}

			set
			{
				_deposit_method_id = value;
			}
		}

		private string _deposit_method_description;
		public string deposit_method_description
		{
			get
			{
				return _deposit_method_description;
			}

			set
			{
				_deposit_method_description = value;
			}
		}

		private string _deposit_method_value;
		public string deposit_method_value
		{
			get
			{
				return _deposit_method_value;
			}

			set
			{
				_deposit_method_value = value;
			}
		}

		private int _bank_account_id;
		public int bank_account_id
		{
			get
			{
				return _bank_account_id;
			}

			set
			{
				_bank_account_id = value;
			}
		}

		private string _bank_account_description;
		public string bank_account_description
		{
			get
			{
				return _bank_account_description;
			}

			set
			{
				_bank_account_description = value;
			}
		}

		private string _bank_account_value;
		public string bank_account_value
		{
			get
			{
				return _bank_account_value;
			}

			set
			{
				_bank_account_value = value;
			}
		}

		private decimal _total_amount;
		public decimal total_amount
		{
			get
			{
				return _total_amount;
			}

			set
			{
				_total_amount = value;
			}
		}

		private int _pull_ach_status_id;
		public int pull_ach_status_id
		{
			get
			{
				return _pull_ach_status_id;
			}

			set
			{
				_pull_ach_status_id = value;
			}
		}

		private string _pull_ach_status_description;
		public string pull_ach_status_description
		{
			get
			{
				return _pull_ach_status_description;
			}

			set
			{
				_pull_ach_status_description = value;
			}
		}

		private string _pull_ach_status_value;
		public string pull_ach_status_value
		{
			get
			{
				return _pull_ach_status_value;
			}

			set
			{
				_pull_ach_status_value = value;
			}
		}

    }
}

