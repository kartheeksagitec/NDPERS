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
	public class busBenefitMultiplierGen : busExtendBase
    {
		public busBenefitMultiplierGen()
		{

		}

		private cdoBenefitMultiplier _icdoBenefitMultiplier;
		public cdoBenefitMultiplier icdoBenefitMultiplier
		{
			get
			{
				return _icdoBenefitMultiplier;
			}
			set
			{
				_icdoBenefitMultiplier = value;
			}
		}

		public bool FindBenefitMultiplier(int Aintbenefitmultiplierid)
		{
			bool lblnResult = false;
			if (_icdoBenefitMultiplier == null)
			{
				_icdoBenefitMultiplier = new cdoBenefitMultiplier();
			}
			if (_icdoBenefitMultiplier.SelectRow(new object[1] { Aintbenefitmultiplierid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
