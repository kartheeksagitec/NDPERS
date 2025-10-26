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
	public class busOrgPlanLifeRateGen : busPlanBase
	{
		public busOrgPlanLifeRateGen()
		{

		} 

		private cdoOrgPlanLifeRate _icdoOrgPlanLifeRate;
		public cdoOrgPlanLifeRate icdoOrgPlanLifeRate
		{
			get
			{
				return _icdoOrgPlanLifeRate;
			}

			set
			{
				_icdoOrgPlanLifeRate = value;
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

		public bool FindOrgPlanLifeRate(int Aintorgplanliferateid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanLifeRate == null)
			{
				_icdoOrgPlanLifeRate = new cdoOrgPlanLifeRate();
			}
			if (_icdoOrgPlanLifeRate.SelectRow(new object[1] { Aintorgplanliferateid }))
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
			//_ibusPlan.ibusOrgPlanLifeRate = this;
            ibusPlan.FindPlan(_ibusOrgPlan.icdoOrgPlan.plan_id);
		}


		public void LoadOrgPlan()
		{
			if (_ibusOrgPlan == null)
			{
				_ibusOrgPlan = new busOrgPlan();
			}	
			_ibusOrgPlan.FindOrgPlan(_icdoOrgPlanLifeRate.org_plan_id);
		}

	}
}
