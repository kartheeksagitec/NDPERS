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
	public class busOrgPlanHealthMedicarePartDRateGen : busPlanBase
	{
		public busOrgPlanHealthMedicarePartDRateGen()
		{

		} 

		private cdoOrgPlanHealthMedicarePartDRate _icdoOrgPlanHealthMedicarePartDRate;
		public cdoOrgPlanHealthMedicarePartDRate icdoOrgPlanHealthMedicarePartDRate
		{
			get
			{
				return _icdoOrgPlanHealthMedicarePartDRate;
			}

			set
			{
				_icdoOrgPlanHealthMedicarePartDRate = value;
			}
		}

		public bool FindOrgPlanHealthMedicarePartDRate(int Aintorgplanhealthmedicarepartdrateid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanHealthMedicarePartDRate == null)
			{
				_icdoOrgPlanHealthMedicarePartDRate = new cdoOrgPlanHealthMedicarePartDRate();
			}
			if (_icdoOrgPlanHealthMedicarePartDRate.SelectRow(new object[1] { Aintorgplanhealthmedicarepartdrateid }))
			{
				lblnResult = true;
			}
			return lblnResult;
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
            _ibusOrgPlan.FindOrgPlan(_icdoOrgPlanHealthMedicarePartDRate.org_plan_id);
        }
	}
}
