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
	public class busServicePurchasePaymentAllocationGen : busExtendBase
    {
        public busServicePurchasePaymentAllocationGen()
		{

		}

        private busServicePurchaseHeader _ibusServicePurchaseHeader;
        public busServicePurchaseHeader ibusServicePurchaseHeader
        {
            get
            {
                return _ibusServicePurchaseHeader;
            }
            set
            {
                _ibusServicePurchaseHeader = value;
            }
        }

        private cdoServicePurchasePaymentAllocation _icdoServicePurchasePaymentAllocation;
        public cdoServicePurchasePaymentAllocation icdoServicePurchasePaymentAllocation
		{
			get
			{
                return _icdoServicePurchasePaymentAllocation;
			}

			set
			{
                _icdoServicePurchasePaymentAllocation = value;
			}
		}

        public bool FindServicePurchasePaymentAllocation(int Aintservicepurchasepaymentallocationid)
		{
			bool lblnResult = false;
			if (_icdoServicePurchasePaymentAllocation == null)
			{
			    _icdoServicePurchasePaymentAllocation = new cdoServicePurchasePaymentAllocation();
			}
            if (_icdoServicePurchasePaymentAllocation.SelectRow(new object[1] { Aintservicepurchasepaymentallocationid}))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
