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
	public class busBenefitRHICOptionGen : busExtendBase
    {
		public busBenefitRHICOptionGen()
		{

		}

		private cdoBenefitRhicOption _icdoBenefitRhicOption;
		public cdoBenefitRhicOption icdoBenefitRhicOption
		{
			get
			{
				return _icdoBenefitRhicOption;
			}
			set
			{
				_icdoBenefitRhicOption = value;
			}
		}

		public bool FindBenefitRHICOption(int Aintbenefitrhicoptionid)
		{
			bool lblnResult = false;
			if (_icdoBenefitRhicOption == null)
			{
				_icdoBenefitRhicOption = new cdoBenefitRhicOption();
			}
			if (_icdoBenefitRhicOption.SelectRow(new object[1] { Aintbenefitrhicoptionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
