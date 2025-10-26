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
	public class busBenefitOptionFactorGen : busExtendBase
    {
		public busBenefitOptionFactorGen()
		{

		}

		private cdoBenefitOptionFactor _icdoBenefitOptionFactor;
		public cdoBenefitOptionFactor icdoBenefitOptionFactor
		{
			get
			{
				return _icdoBenefitOptionFactor;
			}
			set
			{
				_icdoBenefitOptionFactor = value;
			}
		}

		public bool FindBenefitOptionFactor(int Aintbenefitoptionfactorid)
		{
			bool lblnResult = false;
			if (_icdoBenefitOptionFactor == null)
			{
				_icdoBenefitOptionFactor = new cdoBenefitOptionFactor();
			}
			if (_icdoBenefitOptionFactor.SelectRow(new object[1] { Aintbenefitoptionfactorid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
