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
	public class busBenefitProvisionBenefitOptionGen : busExtendBase
    {
		public busBenefitProvisionBenefitOptionGen()
		{

		}

		private cdoBenefitProvisionBenefitOption _icdoBenefitProvisionBenefitOption;
		public cdoBenefitProvisionBenefitOption icdoBenefitProvisionBenefitOption
		{
			get
			{
				return _icdoBenefitProvisionBenefitOption;
			}
			set
			{
				_icdoBenefitProvisionBenefitOption = value;
			}
		}

		public bool FindBenefitProvisionBenefitOption(int Aintbenefitprovisionbenefitoptionid)
		{
			bool lblnResult = false;
			if (_icdoBenefitProvisionBenefitOption == null)
			{
				_icdoBenefitProvisionBenefitOption = new cdoBenefitProvisionBenefitOption();
			}
			if (_icdoBenefitProvisionBenefitOption.SelectRow(new object[1] { Aintbenefitprovisionbenefitoptionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
