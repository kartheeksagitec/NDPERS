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
	public class busBenefitGhdvDeductionGen : busExtendBase
    {
		public busBenefitGhdvDeductionGen()
		{

		}

		private cdoBenefitGhdvDeduction _icdoBenefitGhdvDeduction;
		public cdoBenefitGhdvDeduction icdoBenefitGhdvDeduction
		{
			get
			{
				return _icdoBenefitGhdvDeduction;
			}
			set
			{
				_icdoBenefitGhdvDeduction = value;
			}
		}

		public bool FindBenefitGhdvDeduction(int Aintbenefithealthdeductionid)
		{
			bool lblnResult = false;
			if (_icdoBenefitGhdvDeduction == null)
			{
				_icdoBenefitGhdvDeduction = new cdoBenefitGhdvDeduction();
			}
			if (_icdoBenefitGhdvDeduction.SelectRow(new object[1] { Aintbenefithealthdeductionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
