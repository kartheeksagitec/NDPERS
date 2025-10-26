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
	public class busServicePurchaseDetailGen : busExtendBase
    {
		public busServicePurchaseDetailGen()
		{

		} 

		private cdoServicePurchaseDetail _icdoServicePurchaseDetail;
		public cdoServicePurchaseDetail icdoServicePurchaseDetail
		{
			get
			{
				return _icdoServicePurchaseDetail;
			}

			set
			{
				_icdoServicePurchaseDetail = value;
			}
		}

		public bool FindServicePurchaseDetail(int Aintservicepurchasedetailid)
		{
			bool lblnResult = false;
			if (_icdoServicePurchaseDetail == null)
			{
				_icdoServicePurchaseDetail = new cdoServicePurchaseDetail();
			}
			if (_icdoServicePurchaseDetail.SelectRow(new object[1] { Aintservicepurchasedetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
