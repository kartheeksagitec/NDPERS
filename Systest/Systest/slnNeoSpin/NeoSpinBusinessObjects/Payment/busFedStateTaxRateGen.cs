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
	public class busFedStateTaxRateGen : busExtendBase
    {
        public busFedStateTaxRateGen()
		{

		}

		private cdoFedStateTaxRate _icdoFedStateTaxRate;
        public cdoFedStateTaxRate icdoFedStateTaxRate
		{
			get
			{
                return _icdoFedStateTaxRate;
			}
			set
			{
                _icdoFedStateTaxRate = value;
			}
		}

		public bool FindFedStateTaxRate(int Aintfedstatetaxid)
		{
			bool lblnResult = false;
			if (_icdoFedStateTaxRate == null)
			{
				_icdoFedStateTaxRate = new cdoFedStateTaxRate();
			}
            if (_icdoFedStateTaxRate.SelectRow(new object[1] { Aintfedstatetaxid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
