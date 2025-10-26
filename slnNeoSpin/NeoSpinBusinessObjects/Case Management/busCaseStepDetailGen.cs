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
	public class busCaseStepDetailGen : busExtendBase
    {
		public busCaseStepDetailGen()
		{

		}

		public cdoCaseStepDetail icdoCaseStepDetail { get; set; }




		public virtual bool FindCaseStepDetail(int Aintcasestepdetailid)
		{
			bool lblnResult = false;
			if (icdoCaseStepDetail == null)
			{
				icdoCaseStepDetail = new cdoCaseStepDetail();
			}
			if (icdoCaseStepDetail.SelectRow(new object[1] { Aintcasestepdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
