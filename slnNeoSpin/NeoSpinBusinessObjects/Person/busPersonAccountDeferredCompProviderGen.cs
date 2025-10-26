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
    public class busPersonAccountDeferredCompProviderGen : busExtendBase
    {
        public busPersonAccountDeferredCompProviderGen()
        {

        }

        private cdoPersonAccountDeferredCompProvider _icdoPersonAccountDeferredCompProvider;
        public cdoPersonAccountDeferredCompProvider icdoPersonAccountDeferredCompProvider
        {
            get
            {
                return _icdoPersonAccountDeferredCompProvider;
            }
            set
            {
                _icdoPersonAccountDeferredCompProvider = value;
            }
        }

        public bool FindPersonAccountDeferredCompProvider(int Aintpersonaccountproviderid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountDeferredCompProvider == null)
            {
                _icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider();
            }
            if (_icdoPersonAccountDeferredCompProvider.SelectRow(new object[1] { Aintpersonaccountproviderid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        private busPersonAccountDeferredComp _ibusPersonAccountDeferredComp;
        public busPersonAccountDeferredComp ibusPersonAccountDeferredComp
        {
            get { return _ibusPersonAccountDeferredComp; }
            set { _ibusPersonAccountDeferredComp = value; }
        }

        public void LoadPersonAccountDeferredComp()
        {
            if (_ibusPersonAccountDeferredComp == null)
            {
                _ibusPersonAccountDeferredComp = new busPersonAccountDeferredComp();
            }
            _ibusPersonAccountDeferredComp.FindPersonAccountDeferredComp(_icdoPersonAccountDeferredCompProvider.person_account_id);
        }

        private busOrgPlan _ibusProviderOrgPlan;
        public busOrgPlan ibusProviderOrgPlan
        {
            get { return _ibusProviderOrgPlan; }
            set { _ibusProviderOrgPlan = value; }
        }

        public void LoadProviderOrgPlan()
        {
            if (_ibusProviderOrgPlan == null)
            {
                _ibusProviderOrgPlan = new busOrgPlan();
            }
            _ibusProviderOrgPlan.FindOrgPlan(_icdoPersonAccountDeferredCompProvider.provider_org_plan_id);
        }

        private busPersonEmployment _ibusPersonEmployment;
        public busPersonEmployment ibusPersonEmployment
        {
            get { return _ibusPersonEmployment; }
            set { _ibusPersonEmployment = value; }
        }

        public void LoadPersonEmployment()
        {
            if (_ibusPersonEmployment == null)
            {
                _ibusPersonEmployment = new busPersonEmployment();
            }
            _ibusPersonEmployment.FindPersonEmployment(icdoPersonAccountDeferredCompProvider.person_employment_id);
        }

        private busOrgContact _ibusProviderAgentOrgContact;
        public busOrgContact ibusProviderAgentOrgContact
        {
            get { return _ibusProviderAgentOrgContact; }
            set { _ibusProviderAgentOrgContact = value; }
        }

        public void LoadProviderAgentOrgContact()
        {
            if (_ibusProviderAgentOrgContact == null)
            {
                _ibusProviderAgentOrgContact = new busOrgContact();
            }
            _ibusProviderAgentOrgContact.FindOrgContact(icdoPersonAccountDeferredCompProvider.provider_agent_contact_id);
        }        
    }
}