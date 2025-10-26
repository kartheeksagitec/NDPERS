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
	public class busPaymentHistoryHeaderLookupGen : busMainBase
	{
		public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }



		public virtual void LoadPaymentHistoryHeaders(DataTable adtbSearchResult)
		{
			iclbPaymentHistoryHeader = GetCollection<busPaymentHistoryHeader>(adtbSearchResult, "icdoPaymentHistoryHeader");
		}
	}
}
