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
	public class busBenefitDeductionSummaryGen : busExtendBase
    {
		public busBenefitDeductionSummaryGen()
		{

		}

		private cdoBenefitDeductionSummary _icdoBenefitDeductionSummary;
		public cdoBenefitDeductionSummary icdoBenefitDeductionSummary
		{
			get
			{
				return _icdoBenefitDeductionSummary;
			}
			set
			{
				_icdoBenefitDeductionSummary = value;
			}
		}

		public bool FindBenefitDeductionSummary(int Aintbenefitdeductionsummaryid)
		{
			bool lblnResult = false;
			if (_icdoBenefitDeductionSummary == null)
			{
				_icdoBenefitDeductionSummary = new cdoBenefitDeductionSummary();
			}
			if (_icdoBenefitDeductionSummary.SelectRow(new object[1] { Aintbenefitdeductionsummaryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
