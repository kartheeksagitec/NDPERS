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
	public class busBenefitProvisionBenefitTypeGen : busExtendBase
    {
		public busBenefitProvisionBenefitTypeGen()
		{

		}

		private cdoBenefitProvisionBenefitType _icdoBenefitProvisionBenefitType;
		public cdoBenefitProvisionBenefitType icdoBenefitProvisionBenefitType
		{
			get
			{
				return _icdoBenefitProvisionBenefitType;
			}
			set
			{
				_icdoBenefitProvisionBenefitType = value;
			}
		}

		public bool FindBenefitProvisionBenefitType(int Aintbenefitprovisionbenefittypeid)
		{
			bool lblnResult = false;
			if (_icdoBenefitProvisionBenefitType == null)
			{
				_icdoBenefitProvisionBenefitType = new cdoBenefitProvisionBenefitType();
			}
			if (_icdoBenefitProvisionBenefitType.SelectRow(new object[1] { Aintbenefitprovisionbenefittypeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
