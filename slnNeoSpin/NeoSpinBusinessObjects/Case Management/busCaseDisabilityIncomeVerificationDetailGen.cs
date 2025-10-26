#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busCaseDisabilityIncomeVerificationDetailGen : busExtendBase
    {
		public busCaseDisabilityIncomeVerificationDetailGen()
		{

		}

		public cdoCaseDisabilityIncomeVerificationDetail icdoCaseDisabilityIncomeVerificationDetail { get; set; }




		public virtual bool FindCaseDisabilityIncomeVerificationDetail(int Aintcasedisabilityincomeverificationdetailid)
		{
			bool lblnResult = false;
			if (icdoCaseDisabilityIncomeVerificationDetail == null)
			{
				icdoCaseDisabilityIncomeVerificationDetail = new cdoCaseDisabilityIncomeVerificationDetail();
			}
			if (icdoCaseDisabilityIncomeVerificationDetail.SelectRow(new object[1] { Aintcasedisabilityincomeverificationdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
