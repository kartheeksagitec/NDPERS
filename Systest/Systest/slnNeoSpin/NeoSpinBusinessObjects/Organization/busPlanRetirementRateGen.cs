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
	public class busPlanRetirementRateGen : busPlanBase
	{
		public busPlanRetirementRateGen()
		{

		} 

		private cdoPlanRetirementRate _icdoPlanRetirementRate;
		public cdoPlanRetirementRate icdoPlanRetirementRate
		{
			get
			{
				return _icdoPlanRetirementRate;
			}

			set
			{
				_icdoPlanRetirementRate = value;
			}
		}
		

		public bool FindPlanRetirementRate(int Aintplanrateid)
		{
			bool lblnResult = false;
			if (_icdoPlanRetirementRate == null)
			{
				_icdoPlanRetirementRate = new cdoPlanRetirementRate();
			}
			if (_icdoPlanRetirementRate.SelectRow(new object[1] { Aintplanrateid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadPlan()
		{
			if (ibusPlan == null)
			{
				ibusPlan = new busPlan();
			}
           //_ibusPlan.ibusPlanRetirementRate = this;
            ibusPlan.FindPlan(_icdoPlanRetirementRate.plan_id);
        }

	}
}
