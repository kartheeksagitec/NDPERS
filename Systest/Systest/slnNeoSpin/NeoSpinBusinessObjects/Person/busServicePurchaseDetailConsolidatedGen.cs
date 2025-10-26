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
	public class busServicePurchaseDetailConsolidatedGen : busExtendBase
	{
		public busServicePurchaseDetailConsolidatedGen()
		{

		} 

		private cdoServicePurchaseDetailConsolidated _icdoServicePurchaseDetailConsolidated;
		public cdoServicePurchaseDetailConsolidated icdoServicePurchaseDetailConsolidated
		{
			get
			{
				return _icdoServicePurchaseDetailConsolidated;
			}

			set
			{
				_icdoServicePurchaseDetailConsolidated = value;
			}
		}

		public bool FindServicePurchaseDetailConsolidated(int Aintservicepurchaseconsolidateddetailid)
		{
			bool lblnResult = false;
			if (_icdoServicePurchaseDetailConsolidated == null)
			{
				_icdoServicePurchaseDetailConsolidated = new cdoServicePurchaseDetailConsolidated();
			}
			if (_icdoServicePurchaseDetailConsolidated.SelectRow(new object[1] { Aintservicepurchaseconsolidateddetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
