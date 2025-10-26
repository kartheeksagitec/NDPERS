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
	public class busBenefitCalculationFasMonthsGen : busExtendBase
    {
		public busBenefitCalculationFasMonthsGen()
		{

		}

		private cdoBenefitCalculationFasMonths _icdoBenefitCalculationFasMonths;
		public cdoBenefitCalculationFasMonths icdoBenefitCalculationFasMonths
		{
			get
			{
				return _icdoBenefitCalculationFasMonths;
			}
			set
			{
				_icdoBenefitCalculationFasMonths = value;
			}
		}

		public bool FindBenefitCalculationFasMonths(int Aintbenefitcalcmonthsid)
		{
			bool lblnResult = false;
			if (_icdoBenefitCalculationFasMonths == null)
			{
				_icdoBenefitCalculationFasMonths = new cdoBenefitCalculationFasMonths();
			}
			if (_icdoBenefitCalculationFasMonths.SelectRow(new object[1] { Aintbenefitcalcmonthsid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
