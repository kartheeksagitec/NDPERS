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
	public class busOrgPlanLtcRateGen : busPlanBase
	{
		public busOrgPlanLtcRateGen()
		{

		} 

		private cdoOrgPlanLtcRate _icdoOrgPlanLtcRate;
		public cdoOrgPlanLtcRate icdoOrgPlanLtcRate
		{
			get
			{
				return _icdoOrgPlanLtcRate;
			}

			set
			{
				_icdoOrgPlanLtcRate = value;
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

		public bool FindOrgPlanLtcRate(int Aintorgplanltcrateid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanLtcRate == null)
			{
				_icdoOrgPlanLtcRate = new cdoOrgPlanLtcRate();
			}
			if (_icdoOrgPlanLtcRate.SelectRow(new object[1] { Aintorgplanltcrateid }))
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
			_ibusOrgPlan.FindOrgPlan(_icdoOrgPlanLtcRate.org_plan_id);
		}

	}
}
