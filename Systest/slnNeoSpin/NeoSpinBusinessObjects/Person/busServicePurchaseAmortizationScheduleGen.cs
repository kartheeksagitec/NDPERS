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
	public class busServicePurchaseAmortizationScheduleGen : busExtendBase
    {
        public busServicePurchaseAmortizationScheduleGen()
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

        private cdoServicePurchaseAmortizationSchedule _icdoServicePurchaseAmortizationSchedule;
        public cdoServicePurchaseAmortizationSchedule icdoServicePurchaseAmortizationSchedule
		{
			get
			{
                return _icdoServicePurchaseAmortizationSchedule;
			}

			set
			{
                _icdoServicePurchaseAmortizationSchedule = value;
			}
		}

        public bool FindServicePurchaseAmortizationSchedule(int Aintservicepurchaseamortizationscheduleid)
		{
			bool lblnResult = false;
			if (_icdoServicePurchaseAmortizationSchedule == null)
			{
			    _icdoServicePurchaseAmortizationSchedule = new cdoServicePurchaseAmortizationSchedule();
			}
            if (_icdoServicePurchaseAmortizationSchedule.SelectRow(new object[1] { Aintservicepurchaseamortizationscheduleid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
