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
	public class busBenefitRHICReductionFactorGen : busExtendBase
    {
		public busBenefitRHICReductionFactorGen()
		{

		}

		private cdoBenefitRhicReductionFactor _icdoBenefitRhicReductionFactor;
		public cdoBenefitRhicReductionFactor icdoBenefitRhicReductionFactor
		{
			get
			{
				return _icdoBenefitRhicReductionFactor;
			}
			set
			{
				_icdoBenefitRhicReductionFactor = value;
			}
		}

		public bool FindBenefitRHICReductionFactor(int Aintbenefitrhicreductionfactorid)
		{
			bool lblnResult = false;
			if (_icdoBenefitRhicReductionFactor == null)
			{
				_icdoBenefitRhicReductionFactor = new cdoBenefitRhicReductionFactor();
			}
			if (_icdoBenefitRhicReductionFactor.SelectRow(new object[1] { Aintbenefitrhicreductionfactorid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
