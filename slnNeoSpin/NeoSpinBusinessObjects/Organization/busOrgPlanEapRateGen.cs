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
    public class busOrgPlanEapRateGen : busPlanBase
	{
		public busOrgPlanEapRateGen()
		{

		} 

		private cdoOrgPlanEapRate _icdoOrgPlanEapRate;
		public cdoOrgPlanEapRate icdoOrgPlanEapRate
		{
			get
			{
				return _icdoOrgPlanEapRate;
			}

			set
			{
				_icdoOrgPlanEapRate = value;
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

		public bool FindOrgPlanEapRate(int Aintorgplaneaprateid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanEapRate == null)
			{
				_icdoOrgPlanEapRate = new cdoOrgPlanEapRate();
			}
			if (_icdoOrgPlanEapRate.SelectRow(new object[1] { Aintorgplaneaprateid }))
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
			ibusOrgPlan.FindOrgPlan(_icdoOrgPlanEapRate.org_plan_id);
		}

	}
}
