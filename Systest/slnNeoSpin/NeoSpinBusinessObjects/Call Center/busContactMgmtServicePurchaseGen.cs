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
	public partial class busContactMgmtServicePurchase : busExtendBase
    {
		public busContactMgmtServicePurchase()
		{

		} 

		private cdoContactMgmtServicePurchase _icdoContactMgmtServicePurchase;
		public cdoContactMgmtServicePurchase icdoContactMgmtServicePurchase
		{
			get
			{
				return _icdoContactMgmtServicePurchase;
			}

			set
			{
				_icdoContactMgmtServicePurchase = value;
			}
		}

		public bool FindContactMgmtServicePurchase(int Aintservicepurchaseid)
		{
			bool lblnResult = false;
			if (_icdoContactMgmtServicePurchase == null)
			{
				_icdoContactMgmtServicePurchase = new cdoContactMgmtServicePurchase();
			}
			if (_icdoContactMgmtServicePurchase.SelectRow(new object[1] { Aintservicepurchaseid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
