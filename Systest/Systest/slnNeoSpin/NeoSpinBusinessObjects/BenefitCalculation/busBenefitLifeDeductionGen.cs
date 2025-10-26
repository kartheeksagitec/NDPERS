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
	public class busBenefitLifeDeductionGen : busExtendBase
    {
		public busBenefitLifeDeductionGen()
		{

		}

		private cdoBenefitLifeDeduction _icdoBenefitLifeDeduction;
		public cdoBenefitLifeDeduction icdoBenefitLifeDeduction
		{
			get
			{
				return _icdoBenefitLifeDeduction;
			}
			set
			{
				_icdoBenefitLifeDeduction = value;
			}
		}

		public bool FindBenefitLifeDeduction(int Aintbenefitlifedeductionid)
		{
			bool lblnResult = false;
			if (_icdoBenefitLifeDeduction == null)
			{
				_icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
			}
			if (_icdoBenefitLifeDeduction.SelectRow(new object[1] { Aintbenefitlifedeductionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
