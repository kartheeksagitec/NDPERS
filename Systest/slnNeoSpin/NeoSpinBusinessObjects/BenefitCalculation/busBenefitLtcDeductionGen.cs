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
	public class busBenefitLtcDeductionGen : busExtendBase
    {
		public busBenefitLtcDeductionGen()
		{

		}

		private cdoBenefitLtcDeduction _icdoBenefitLtcDeduction;
		public cdoBenefitLtcDeduction icdoBenefitLtcDeduction
		{
			get
			{
				return _icdoBenefitLtcDeduction;
			}
			set
			{
				_icdoBenefitLtcDeduction = value;
			}
		}

		public bool FindBenefitLtcDeduction(int Aintbenefitltcdeductionid)
		{
			bool lblnResult = false;
			if (_icdoBenefitLtcDeduction == null)
			{
				_icdoBenefitLtcDeduction = new cdoBenefitLtcDeduction();
			}
			if (_icdoBenefitLtcDeduction.SelectRow(new object[1] { Aintbenefitltcdeductionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
