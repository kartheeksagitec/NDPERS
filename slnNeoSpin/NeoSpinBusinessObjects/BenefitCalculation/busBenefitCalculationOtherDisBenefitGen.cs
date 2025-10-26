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
	public class busBenefitCalculationOtherDisBenefitGen : busExtendBase
    {
		public busBenefitCalculationOtherDisBenefitGen()
		{

		}

		private cdoBenefitCalculationOtherDisBenefit _icdoBenefitCalculationOtherDisBenefit;
		public cdoBenefitCalculationOtherDisBenefit icdoBenefitCalculationOtherDisBenefit
		{
			get
			{
				return _icdoBenefitCalculationOtherDisBenefit;
			}
			set
			{
				_icdoBenefitCalculationOtherDisBenefit = value;
			}
		}

		public bool FindBenefitCalculationOtherDisBenefit(int Aintbenefitestimateotherdisbenefitid)
		{
			bool lblnResult = false;
			if (_icdoBenefitCalculationOtherDisBenefit == null)
			{
				_icdoBenefitCalculationOtherDisBenefit = new cdoBenefitCalculationOtherDisBenefit();
			}
			if (_icdoBenefitCalculationOtherDisBenefit.SelectRow(new object[1] { Aintbenefitestimateotherdisbenefitid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
