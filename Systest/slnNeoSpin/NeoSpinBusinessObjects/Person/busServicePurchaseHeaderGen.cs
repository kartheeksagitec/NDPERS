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
    public class busServicePurchaseHeaderGen : busExtendBase
	{
		public busServicePurchaseHeaderGen()
		{

		}
       
        private decimal _idecTotalAllocatedPsc;

        public decimal idecTotalAllocatedPsc
        {
            get { return _idecTotalAllocatedPsc; }
            set { _idecTotalAllocatedPsc = value; }
        }


		private cdoServicePurchaseHeader _icdoServicePurchaseHeader;
		public cdoServicePurchaseHeader icdoServicePurchaseHeader
		{
			get
			{
				return _icdoServicePurchaseHeader;
			}

			set
			{
				_icdoServicePurchaseHeader = value;
			}
		}

		public bool FindServicePurchaseHeader(int Aintservicepurchaseheaderid)
		{
			bool lblnResult = false;
			if (_icdoServicePurchaseHeader == null)
			{
				_icdoServicePurchaseHeader = new cdoServicePurchaseHeader();
			}
			if (_icdoServicePurchaseHeader.SelectRow(new object[1] { Aintservicepurchaseheaderid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
