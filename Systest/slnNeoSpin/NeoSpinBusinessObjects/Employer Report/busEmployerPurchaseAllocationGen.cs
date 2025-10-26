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
	public class busEmployerPurchaseAllocationGen : busExtendBase
    {
		public busEmployerPurchaseAllocationGen()
		{

		} 

		private cdoEmployerPurchaseAllocation _icdoEmployerPurchaseAllocation;
		public cdoEmployerPurchaseAllocation icdoEmployerPurchaseAllocation
		{
			get
			{
				return _icdoEmployerPurchaseAllocation;
			}

			set
			{
				_icdoEmployerPurchaseAllocation = value;
			}
		}

		public bool FindEmployerPurchaseAllocation(int Aintemployerpurchaseallocationid)
		{
			bool lblnResult = false;
			if (_icdoEmployerPurchaseAllocation == null)
			{
				_icdoEmployerPurchaseAllocation = new cdoEmployerPurchaseAllocation();
			}
			if (_icdoEmployerPurchaseAllocation.SelectRow(new object[1] { Aintemployerpurchaseallocationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
