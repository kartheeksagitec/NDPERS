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
	public class busCaseFileDetailGen : busExtendBase
    {
		public busCaseFileDetailGen()
		{

		}

		public cdoCaseFileDetail icdoCaseFileDetail { get; set; }




		public virtual bool FindCaseFileDetail(int Aintcasefiledetailid)
		{
			bool lblnResult = false;
			if (icdoCaseFileDetail == null)
			{
				icdoCaseFileDetail = new cdoCaseFileDetail();
			}
			if (icdoCaseFileDetail.SelectRow(new object[1] { Aintcasefiledetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
