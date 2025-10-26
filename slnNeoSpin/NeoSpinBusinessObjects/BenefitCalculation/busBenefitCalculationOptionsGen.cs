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
	public class busBenefitCalculationOptionsGen : busExtendBase
    {
		public busBenefitCalculationOptionsGen()
		{

		}

		private cdoBenefitCalculationOptions _icdoBenefitCalculationOptions;
		public cdoBenefitCalculationOptions icdoBenefitCalculationOptions
		{
			get
			{
				return _icdoBenefitCalculationOptions;
			}
			set
			{
				_icdoBenefitCalculationOptions = value;
			}
		}

		public bool FindBenefitCalculationOptions(int Aintbenefitcalculationoptionsid)
		{
			bool lblnResult = false;
			if (_icdoBenefitCalculationOptions == null)
			{
				_icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions();
			}
			if (_icdoBenefitCalculationOptions.SelectRow(new object[1] { Aintbenefitcalculationoptionsid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
