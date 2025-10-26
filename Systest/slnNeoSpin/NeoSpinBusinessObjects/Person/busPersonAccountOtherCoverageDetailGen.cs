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
	public class busPersonAccountOtherCoverageDetailGen : busPersonAccount
	{
		public busPersonAccountOtherCoverageDetailGen()
		{

		}

		private cdoPersonAccountOtherCoverageDetail _icdoPersonAccountOtherCoverageDetail;
		public cdoPersonAccountOtherCoverageDetail icdoPersonAccountOtherCoverageDetail
		{
			get
			{
				return _icdoPersonAccountOtherCoverageDetail;
			}
			set
			{
				_icdoPersonAccountOtherCoverageDetail = value;
			}
		}

		public bool FindPersonAccountOtherCoverageDetail(int Aintaccountothercoveragedetailid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountOtherCoverageDetail == null)
			{
				_icdoPersonAccountOtherCoverageDetail = new cdoPersonAccountOtherCoverageDetail();
			}
			if (_icdoPersonAccountOtherCoverageDetail.SelectRow(new object[1] { Aintaccountothercoveragedetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        
	}
}
