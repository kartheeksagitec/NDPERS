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
	public class busDeductionCalculationGen : busExtendBase
	{
		public busDeductionCalculationGen()
		{

		}

		private cdoBenefitCalculation _icdoBenefitCalculation;
		public cdoBenefitCalculation icdoBenefitCalculation
		{
			get
			{
				return _icdoBenefitCalculation;
			}
			set
			{
				_icdoBenefitCalculation = value;
			}
		}

		public bool FindDeductionCalculation(int Aintbenefitcalculationid)
		{
			bool lblnResult = false;
			if (_icdoBenefitCalculation == null)
			{
				_icdoBenefitCalculation = new cdoBenefitCalculation();
			}
			if (_icdoBenefitCalculation.SelectRow(new object[1] { Aintbenefitcalculationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
