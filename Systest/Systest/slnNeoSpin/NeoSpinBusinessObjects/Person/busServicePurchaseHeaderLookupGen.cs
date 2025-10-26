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
	public class busServicePurchaseHeaderLookupGen : busMainBase
	{

		private Collection<busServicePurchaseHeader> _icolServicePurchaseHeader;
		public Collection<busServicePurchaseHeader> icolServicePurchaseHeader
		{
			get
			{
				return _icolServicePurchaseHeader;
			}

			set
			{
				_icolServicePurchaseHeader = value;
			}
		}

		public void LoadServicePurchaseHeader(DataTable adtbSearchResult)
		{
			_icolServicePurchaseHeader = GetCollection<busServicePurchaseHeader>(adtbSearchResult, "icdoServicePurchaseHeader");
		}
	}
}
