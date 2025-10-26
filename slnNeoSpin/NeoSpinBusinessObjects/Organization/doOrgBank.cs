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
    public class doOrgBank : doBase
    {
        
         public doOrgBank() : base()
         {
         }
		private int _org_bank_id;
		public int org_bank_id
		{
			get
			{
				return _org_bank_id;
			}

			set
			{
				_org_bank_id = value;
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

		private int _bank_org_id;
		public int bank_org_id
		{
			get
			{
				return _bank_org_id;
			}

			set
			{
				_bank_org_id = value;
			}
		}

		private string _account_no;
		public string account_no
		{
			get
			{
				return _account_no;
			}

			set
			{
				_account_no = value;
			}
		}

		private int _usage_id;
		public int usage_id
		{
			get
			{
				return _usage_id;
			}

			set
			{
				_usage_id = value;
			}
		}

		private string _usage_description;
		public string usage_description
		{
			get
			{
				return _usage_description;
			}

			set
			{
				_usage_description = value;
			}
		}

		private string _usage_value;
		public string usage_value
		{
			get
			{
				return _usage_value;
			}

			set
			{
				_usage_value = value;
			}
		}

		private int _account_type_id;
		public int account_type_id
		{
			get
			{
				return _account_type_id;
			}

			set
			{
				_account_type_id = value;
			}
		}

		private string _account_type_description;
		public string account_type_description
		{
			get
			{
				return _account_type_description;
			}

			set
			{
				_account_type_description = value;
			}
		}

		private string _account_type_value;
		public string account_type_value
		{
			get
			{
				return _account_type_value;
			}

			set
			{
				_account_type_value = value;
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

    }
}

