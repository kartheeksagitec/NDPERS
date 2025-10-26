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
	public partial class busBenefitEstimate : busExtendBase
    {
		public busBenefitEstimate()
		{

		} 

		private cdoBenefitEstimate _icdoBenefitEstimate;
		public cdoBenefitEstimate icdoBenefitEstimate
		{
			get
			{
				return _icdoBenefitEstimate;
			}

			set
			{
				_icdoBenefitEstimate = value;
			}
		}

		public bool FindBenefitEstimate(int Aintbenefitestimateid)
		{
			bool lblnResult = false;
			if (_icdoBenefitEstimate == null)
			{
				_icdoBenefitEstimate = new cdoBenefitEstimate();
			}
			if (_icdoBenefitEstimate.SelectRow(new object[1] { Aintbenefitestimateid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
