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
	public class busPaymentScheduleLookupGen : busMainBase
	{
		public Collection<busPaymentSchedule> iclbPaymentSchedule { get; set; }



		public virtual void LoadPaymentSchedules(DataTable adtbSearchResult)
		{
			iclbPaymentSchedule = GetCollection<busPaymentSchedule>(adtbSearchResult, "icdoPaymentSchedule");
		}
	}
}
