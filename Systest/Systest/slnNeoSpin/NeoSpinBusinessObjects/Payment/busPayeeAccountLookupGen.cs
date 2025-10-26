#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busPayeeAccountLookupGen : busMainBase
	{

		private Collection<busPayeeAccount> _iclbPayeeAccount;
		public Collection<busPayeeAccount> iclbPayeeAccount
		{
			get
			{
				return _iclbPayeeAccount;
			}
			set
			{
				_iclbPayeeAccount = value;
			}
		}

		public void LoadPayeeAccounts(DataTable adtbSearchResult)
		{
			_iclbPayeeAccount = GetCollection<busPayeeAccount>(adtbSearchResult, "icdoPayeeAccount");
		}       
	}
}
