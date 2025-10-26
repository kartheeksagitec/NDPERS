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
    public class busOrgPlanVisionRateGen : busPlanBase
	{
		public busOrgPlanVisionRateGen()
		{

		} 

		private cdoOrgPlanVisionRate _icdoOrgPlanVisionRate;
		public cdoOrgPlanVisionRate icdoOrgPlanVisionRate
		{
			get
			{
				return _icdoOrgPlanVisionRate;
			}

			set
			{
				_icdoOrgPlanVisionRate = value;
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

		public bool FindOrgPlanVisionRate(int Aintorgplanvisionrateid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanVisionRate == null)
			{
				_icdoOrgPlanVisionRate = new cdoOrgPlanVisionRate();
			}
			if (_icdoOrgPlanVisionRate.SelectRow(new object[1] { Aintorgplanvisionrateid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadOrgPlan()
		{
			if (_ibusOrgPlan == null)
			{
				_ibusOrgPlan = new busOrgPlan();
			}
            _ibusOrgPlan.FindOrgPlan(_icdoOrgPlanVisionRate.org_plan_id);
		}

		public void LoadPlan()
		{
			if (ibusPlan == null)
			{
				ibusPlan = new busPlan();
			}           
            ibusPlan.FindPlan(_ibusOrgPlan.icdoOrgPlan.plan_id);
		}
	}
}
