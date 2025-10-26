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
	public partial class busOrgPlanProvider : busExtendBase
    {
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get { return _ibusOrganization; }
            set { _ibusOrganization = value; }
        }

        
        /// <summary>
        /// To load Organization's Information.
        /// </summary>
        public void LoadOrganization(int lintOrgid)
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(lintOrgid);
        }


        private Collection<busOrgPlanProvider> _iclbOtherOrgPlanProvider;
        public Collection<busOrgPlanProvider> iclbOtherOrgPlanProvider
        {
            get { return _iclbOtherOrgPlanProvider; }
            set { _iclbOtherOrgPlanProvider = value; }
        }

        /// <summary>
        /// To Load Other Plans Provider's Information.
        /// </summary>
        public void LoadOtherOrgPlanProviders()
        {
            DataTable ldtbList = Select("cdoOrgPlanProvider.LOAD_OTHER_PLAN_PROVIDERS", new object[2] { _icdoOrgPlanProvider.org_plan_id,_icdoOrgPlanProvider.org_plan_provider_id });
            _iclbOtherOrgPlanProvider = GetCollection<busOrgPlanProvider>(ldtbList, "icdoOrgPlanProvider");
            foreach (busOrgPlanProvider lobjOrgPlanProvider in _iclbOtherOrgPlanProvider)
            {
                lobjOrgPlanProvider.LoadOrganization(_ibusOrganization.icdoOrganization.org_id);
                lobjOrgPlanProvider.LoadOrgPlan(_icdoOrgPlanProvider.org_plan_id);
            }
        }

        private busOrgPlan _ibusOrgPlan;
        public busOrgPlan ibusOrgPlan
        {
            get { return _ibusOrgPlan; }
            set { _ibusOrgPlan = value; }
        }
        public busWssPersonAccountFlexCompConversion ibusWssPersonAccountFlexCompConversion { get; set; } //PIR 10044
        /// <summary> 
        /// To Load Organization's Plan Information.
        /// </summary>
        public void LoadOrgPlan(int lintOrgPlanid)
        {
            if (_ibusOrgPlan == null)
            {
                _ibusOrgPlan = new busOrgPlan();
            }
            _ibusOrgPlan.FindOrgPlan(lintOrgPlanid);
            _ibusOrgPlan.LoadPlanInfo(_ibusOrgPlan.icdoOrgPlan.plan_id);
        }

        private busOrganization _ibusProviderOrg;
        public busOrganization ibusProviderOrg
        {
            get { return _ibusProviderOrg; }
            set { _ibusProviderOrg = value; }
        }

        /// <summary>
        /// To Display the Provider Org Name.
        /// </summary>
        public void LoadProviderOrg()
        {
            if (_ibusProviderOrg == null)
            {
                _ibusProviderOrg = new busOrganization();
            }
            _ibusProviderOrg.FindOrganization(_icdoOrgPlanProvider.provider_org_id);
        }
        
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            _icdoOrgPlanProvider.provider_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(_icdoOrgPlanProvider.istrOrgCodeID); 
            LoadProviderOrg();
            base.BeforeValidate(aenmPageMode);
            
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadProviderOrg();
        }
	}
}
