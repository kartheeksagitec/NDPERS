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
	public class busBenefitSSLIFactorGen : busExtendBase
    {
		public busBenefitSSLIFactorGen()
		{

		}

		private cdoBenefitSsliFactor _icdoBenefitSsliFactor;
		public cdoBenefitSsliFactor icdoBenefitSsliFactor
		{
			get
			{
				return _icdoBenefitSsliFactor;
			}
			set
			{
				_icdoBenefitSsliFactor = value;
			}
		}

		public bool FindBenefitSSLIFactor(int Aintbenefitsslifactorid)
		{
			bool lblnResult = false;
			if (_icdoBenefitSsliFactor == null)
			{
				_icdoBenefitSsliFactor = new cdoBenefitSsliFactor();
			}
			if (_icdoBenefitSsliFactor.SelectRow(new object[1] { Aintbenefitsslifactorid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
