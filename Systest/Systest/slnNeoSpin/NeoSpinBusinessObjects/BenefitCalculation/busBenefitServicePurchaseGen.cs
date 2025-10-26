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
	public class busBenefitServicePurchaseGen : busExtendBase
    {
		public busBenefitServicePurchaseGen()
		{

		}

		private cdoBenefitServicePurchase _icdoBenefitServicePurchase;
		public cdoBenefitServicePurchase icdoBenefitServicePurchase
		{
			get
			{
				return _icdoBenefitServicePurchase;
			}
			set
			{
				_icdoBenefitServicePurchase = value;
			}
		}

		public bool FindBenefitServicePurchase(int Aintbenefitservicepurchaseid)
		{
			bool lblnResult = false;
			if (_icdoBenefitServicePurchase == null)
			{
				_icdoBenefitServicePurchase = new cdoBenefitServicePurchase();
			}
			if (_icdoBenefitServicePurchase.SelectRow(new object[1] { Aintbenefitservicepurchaseid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
