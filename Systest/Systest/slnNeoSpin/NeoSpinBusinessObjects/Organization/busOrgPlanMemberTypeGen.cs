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
	public class busOrgPlanMemberTypeGen : busPlanBase
	{
		public busOrgPlanMemberTypeGen()
		{

		}

        private cdoOrgPlanMemberType _icdoOrgPlanMemberType;
        public cdoOrgPlanMemberType icdoOrgPlanMemberType
        {
            get
            {
                return _icdoOrgPlanMemberType;
            }

            set
            {
                _icdoOrgPlanMemberType = value;
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

        public bool FindOrgPlanMemberType(int AintOrgPlanMemberTypeID)
        {
            bool lblnResult = false;
            if (_icdoOrgPlanMemberType == null)
            {
                _icdoOrgPlanMemberType = new cdoOrgPlanMemberType();
            }
            if (_icdoOrgPlanMemberType.SelectRow(new object[1] { AintOrgPlanMemberTypeID }))
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
            _ibusOrgPlan.FindOrgPlan(_icdoOrgPlanMemberType.org_plan_id);
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
