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
	public class busBenefitFasIndexFactorGen : busExtendBase
    {
		public busBenefitFasIndexFactorGen()
		{

		}

		private cdoBenefitFasIndexFactor _icdoBenefitFasIndexFactor;
		public cdoBenefitFasIndexFactor icdoBenefitFasIndexFactor
		{
			get
			{
				return _icdoBenefitFasIndexFactor;
			}
			set
			{
				_icdoBenefitFasIndexFactor = value;
			}
		}

		public bool FindBenefitFasIndexFactor(int Aintbenefitfasindexfactorid)
		{
			bool lblnResult = false;
			if (_icdoBenefitFasIndexFactor == null)
			{
				_icdoBenefitFasIndexFactor = new cdoBenefitFasIndexFactor();
			}
			if (_icdoBenefitFasIndexFactor.SelectRow(new object[1] { Aintbenefitfasindexfactorid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
