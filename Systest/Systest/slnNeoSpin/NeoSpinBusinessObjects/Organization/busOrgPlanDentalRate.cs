#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busOrgPlanDentalRate : busOrgPlanDentalRateGen
	{
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
        private busOrganization _ibusProvider;
        public busOrganization ibusProvider
        {
            get { return _ibusProvider; }
            set { _ibusProvider = value; }
        }
        public void LoadOrgPlan()
        {
            if (_ibusOrgPlan == null)
            {
                _ibusOrgPlan = new busOrgPlan();
            }
            _ibusOrgPlan.FindOrgPlan(icdoOrgPlanDentalRate.org_plan_id);
        }
        private int _iintProviderID;
        public int iintProviderID
        {
            get { return _iintProviderID; }
            set { _iintProviderID = value; }
        } 
        public void LoadPlan()
        {
            if (ibusPlan == null)
            {
                ibusPlan = new busPlan();
            }
            ibusPlan.FindPlan(_ibusOrgPlan.icdoOrgPlan.plan_id);
        }
        public void LoadProvider(int AintProviderID)
        {
            if (_ibusProvider == null)
            {
                _ibusProvider = new busOrganization();
            }           
            _ibusProvider.FindOrganization(AintProviderID);
        }
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
            {
                ibusOrganization = new busOrganization();
            }
            ibusOrganization.FindOrganization(ibusOrgPlan.icdoOrgPlan.org_id);
        }
	}
}
