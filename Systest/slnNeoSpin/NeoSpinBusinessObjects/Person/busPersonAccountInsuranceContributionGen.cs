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
    public class busPersonAccountInsuranceContributionGen : busExtendBase
    {
        public busPersonAccountInsuranceContributionGen()
        {

        }

        private cdoPersonAccountInsuranceContribution _icdoPersonAccountInsuranceContribution;
        public cdoPersonAccountInsuranceContribution icdoPersonAccountInsuranceContribution
        {
            get
            {
                return _icdoPersonAccountInsuranceContribution;
            }
            set
            {
                _icdoPersonAccountInsuranceContribution = value;
            }
        }

        public bool FindPersonAccountInsuranceContribution(int Ainthealthinsurancecontributionid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountInsuranceContribution == null)
            {
                _icdoPersonAccountInsuranceContribution = new cdoPersonAccountInsuranceContribution();
            }
            if (_icdoPersonAccountInsuranceContribution.SelectRow(new object[1] { Ainthealthinsurancecontributionid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        private busPersonAccount _ibusPersonAccount;

        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }

        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }
            _ibusPersonAccount.FindPersonAccount(icdoPersonAccountInsuranceContribution.person_account_id);
        }
    }
}
