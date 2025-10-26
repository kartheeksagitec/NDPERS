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
	public class busServicePurchaseDetailUserraGen : busExtendBase
    {
		public busServicePurchaseDetailUserraGen()
		{

		} 

		private cdoServicePurchaseDetailUserra _icdoServicePurchaseDetailUserra;
		public cdoServicePurchaseDetailUserra icdoServicePurchaseDetailUserra
		{
			get
			{
				return _icdoServicePurchaseDetailUserra;
			}

			set
			{
				_icdoServicePurchaseDetailUserra = value;
			}
		}

		public bool FindServicePurchaseDetailUserra(int Aintservicepurchaseuserradetailid)
		{
			bool lblnResult = false;
			if (_icdoServicePurchaseDetailUserra == null)
			{
				_icdoServicePurchaseDetailUserra = new cdoServicePurchaseDetailUserra();
			}
			if (_icdoServicePurchaseDetailUserra.SelectRow(new object[1] { Aintservicepurchaseuserradetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
