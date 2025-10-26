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
	public class busFedStateFlatTaxRateGen : busExtendBase
    {
		public busFedStateFlatTaxRateGen()
		{

		}

		private cdoFedStateFlatTaxRate _icdoFedStateFlatTaxRate;
		public cdoFedStateFlatTaxRate icdoFedStateFlatTaxRate
		{
			get
			{
				return _icdoFedStateFlatTaxRate;
			}
			set
			{
				_icdoFedStateFlatTaxRate = value;
			}
		}

		public bool FindFedStateFlatTaxRate(int Aintfedstateflattaxid)
		{
			bool lblnResult = false;
			if (_icdoFedStateFlatTaxRate == null)
			{
				_icdoFedStateFlatTaxRate = new cdoFedStateFlatTaxRate();
			}
			if (_icdoFedStateFlatTaxRate.SelectRow(new object[1] { Aintfedstateflattaxid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
