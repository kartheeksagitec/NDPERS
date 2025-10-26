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
    public class busOrgPlanDentalRateGen : busPlanBase
	{
		public busOrgPlanDentalRateGen()
		{

		} 

		private cdoOrgPlanDentalRate _icdoOrgPlanDentalRate;
		public cdoOrgPlanDentalRate icdoOrgPlanDentalRate
		{
			get
			{
				return _icdoOrgPlanDentalRate;
			}

			set
			{
				_icdoOrgPlanDentalRate = value;
			}
		}

		public bool FindOrgPlanDentalRate(int Aintorgplandentalrateid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanDentalRate == null)
			{
				_icdoOrgPlanDentalRate = new cdoOrgPlanDentalRate();
			}
			if (_icdoOrgPlanDentalRate.SelectRow(new object[1] { Aintorgplandentalrateid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
