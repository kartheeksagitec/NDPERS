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
	public class busCaseFinancialHardshipProviderDetailGen : busExtendBase
    {
		public busCaseFinancialHardshipProviderDetailGen()
		{

		}

		public cdoCaseFinancialHardshipProviderDetail icdoCaseFinancialHardshipProviderDetail { get; set; }




		public virtual bool FindCaseFinancialHardshipProviderDetail(int Aintcasefinancialhardshipproviderdetailid)
		{
			bool lblnResult = false;
			if (icdoCaseFinancialHardshipProviderDetail == null)
			{
				icdoCaseFinancialHardshipProviderDetail = new cdoCaseFinancialHardshipProviderDetail();
			}
			if (icdoCaseFinancialHardshipProviderDetail.SelectRow(new object[1] { Aintcasefinancialhardshipproviderdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
