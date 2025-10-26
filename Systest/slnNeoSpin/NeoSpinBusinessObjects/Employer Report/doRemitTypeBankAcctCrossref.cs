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
    public class doRemitTypeBankAcctCrossref : doBase
    {
         public doRemitTypeBankAcctCrossref() : base()
         {
         }
		private int _remit_bank_acct_id;
		public int remit_bank_acct_id
		{
			get
			{
				return _remit_bank_acct_id;
			}

			set
			{
				_remit_bank_acct_id = value;
			}
		}

		private int _remittance_type_id;
		public int remittance_type_id
		{
			get
			{
				return _remittance_type_id;
			}

			set
			{
				_remittance_type_id = value;
			}
		}

		private string _remittance_type_description;
		public string remittance_type_description
		{
			get
			{
				return _remittance_type_description;
			}

			set
			{
				_remittance_type_description = value;
			}
		}

		private string _remittance_type_value;
		public string remittance_type_value
		{
			get
			{
				return _remittance_type_value;
			}

			set
			{
				_remittance_type_value = value;
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

    }
}

