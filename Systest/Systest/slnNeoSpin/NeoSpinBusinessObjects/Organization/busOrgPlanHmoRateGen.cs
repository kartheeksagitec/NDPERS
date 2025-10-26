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
    public class busOrgPlanHmoRateGen : busPlanBase
	{
		public busOrgPlanHmoRateGen()
		{

		} 

		private cdoOrgPlanHmoRate _icdoOrgPlanHmoRate;
		public cdoOrgPlanHmoRate icdoOrgPlanHmoRate
		{
			get
			{
				return _icdoOrgPlanHmoRate;
			}

			set
			{
				_icdoOrgPlanHmoRate = value;
			}
		}
	

		private busOrgPlan _ibusOrgPlan;
		public busOrgPlan ibusOrgPlan
		{
			get
			{
				return _ibusOrgPlan;
			}

			set
			{
				_ibusOrgPlan = value;
			}
		}

		public bool FindOrgPlanHmoRate(int Aintorgplanhmorateid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanHmoRate == null)
			{
				_icdoOrgPlanHmoRate = new cdoOrgPlanHmoRate();
			}
			if (_icdoOrgPlanHmoRate.SelectRow(new object[1] { Aintorgplanhmorateid }))
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
            ibusPlan.FindPlan(_ibusOrgPlan.icdoOrgPlan.plan_id);
		}


		public void LoadOrgPlan()
		{
			if (_ibusOrgPlan == null)
			{
				_ibusOrgPlan = new busOrgPlan();
			}			
			_ibusOrgPlan.FindOrgPlan(_icdoOrgPlanHmoRate.org_plan_id);
		}
	}
}
