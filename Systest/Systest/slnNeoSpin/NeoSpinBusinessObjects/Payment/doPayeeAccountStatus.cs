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
    public class doPayeeAccountStatus : doBase
    {
         
         public doPayeeAccountStatus() : base()
         {
         }
		private int _payee_account_status_id;
		public int payee_account_status_id
		{
			get
			{
				return _payee_account_status_id;
			}

			set
			{
				_payee_account_status_id = value;
			}
		}

		private int _payee_account_id;
		public int payee_account_id
		{
			get
			{
				return _payee_account_id;
			}

			set
			{
				_payee_account_id = value;
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

		private DateTime _status_effective_date;
		public DateTime status_effective_date
		{
			get
			{
				return _status_effective_date;
			}

			set
			{
				_status_effective_date = value;
			}
		}

		private int _suspension_status_reason_id;
		public int suspension_status_reason_id
		{
			get
			{
				return _suspension_status_reason_id;
			}

			set
			{
				_suspension_status_reason_id = value;
			}
		}

		private string _suspension_status_reason_description;
		public string suspension_status_reason_description
		{
			get
			{
				return _suspension_status_reason_description;
			}

			set
			{
				_suspension_status_reason_description = value;
			}
		}

		private string _suspension_status_reason_value;
		public string suspension_status_reason_value
		{
			get
			{
				return _suspension_status_reason_value;
			}

			set
			{
				_suspension_status_reason_value = value;
			}
		}

		private int _terminated_status_reason_id;
		public int terminated_status_reason_id
		{
			get
			{
				return _terminated_status_reason_id;
			}

			set
			{
				_terminated_status_reason_id = value;
			}
		}

		private string _terminated_status_reason_description;
		public string terminated_status_reason_description
		{
			get
			{
				return _terminated_status_reason_description;
			}

			set
			{
				_terminated_status_reason_description = value;
			}
		}

		private string _terminated_status_reason_value;
		public string terminated_status_reason_value
		{
			get
			{
				return _terminated_status_reason_value;
			}

			set
			{
				_terminated_status_reason_value = value;
			}
		}

    }
}

