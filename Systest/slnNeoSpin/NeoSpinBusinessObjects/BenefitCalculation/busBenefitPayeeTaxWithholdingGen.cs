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
	public class busBenefitPayeeTaxWithholdingGen : busExtendBase
    {
		public busBenefitPayeeTaxWithholdingGen()
		{

		}

		private cdoBenefitPayeeTaxWithholding _icdoBenefitPayeeTaxWithholding;
		public cdoBenefitPayeeTaxWithholding icdoBenefitPayeeTaxWithholding
		{
			get
			{
				return _icdoBenefitPayeeTaxWithholding;
			}
			set
			{
				_icdoBenefitPayeeTaxWithholding = value;
			}
		}

		public bool FindBenefitPayeeTaxWithholding(int Aintbenefitpayeetaxwithholdingid)
		{
			bool lblnResult = false;
			if (_icdoBenefitPayeeTaxWithholding == null)
			{
				_icdoBenefitPayeeTaxWithholding = new cdoBenefitPayeeTaxWithholding();
			}
			if (_icdoBenefitPayeeTaxWithholding.SelectRow(new object[1] { Aintbenefitpayeetaxwithholdingid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
