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
	public class busBenefitServicePurchase : busBenefitServicePurchaseGen
	{
        private string _istrIncludeRemainder;
        public string istrIncludeRemainder
        {
            get { return _istrIncludeRemainder; }
            set { _istrIncludeRemainder = value; }
        }

        private string _istrActionStatus;
        public string istrActionStatus
        {
            get { return _istrActionStatus; }
            set { _istrActionStatus = value; }
        }

        private string _istrServicePurchaseType;
        public string istrServicePurchaseType
        {
            get { return _istrServicePurchaseType; }
            set { _istrServicePurchaseType = value; }
        }

        private busServicePurchaseHeader _ibusServicePurchaseHeader;
        public busServicePurchaseHeader ibusServicePurchaseHeader
        {
            get { return _ibusServicePurchaseHeader; }
            set { _ibusServicePurchaseHeader = value; }
        }

        public void LoadServicePurchaseHeader()
        {
            if (_ibusServicePurchaseHeader == null)
                _ibusServicePurchaseHeader = new busServicePurchaseHeader();
            _ibusServicePurchaseHeader.FindServicePurchaseHeader(icdoBenefitServicePurchase.service_purchase_header_id);
        }
	}
}
