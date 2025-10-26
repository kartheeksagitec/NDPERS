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
	public class busPersonAccountAchDetailGen : busExtendBase
    {
		public busPersonAccountAchDetailGen()
		{

		}

		private cdoPersonAccountAchDetail _icdoPersonAccountAchDetail;
		public cdoPersonAccountAchDetail icdoPersonAccountAchDetail
		{
			get
			{
				return _icdoPersonAccountAchDetail;
			}
			set
			{
				_icdoPersonAccountAchDetail = value;
			}
		}

        public bool FindPersonAccountAchDetail(int Aintpersonaccountachdetailid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountAchDetail == null)
            {
                _icdoPersonAccountAchDetail = new cdoPersonAccountAchDetail();
            }
            if (_icdoPersonAccountAchDetail.SelectRow(new object[1] { Aintpersonaccountachdetailid }))
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
        private busOrganization _ibusBankOrg;

        public busOrganization ibusBankOrg
        {
            get { return _ibusBankOrg; }
            set { _ibusBankOrg = value; }
        }

        public void LoadPersonAccount()
        {
            if (ibusPersonAccount == null)
            {
                ibusPersonAccount = new busPersonAccount();
            }
            ibusPersonAccount.FindPersonAccount(icdoPersonAccountAchDetail.person_account_id);
        }

        public void LoadBankOrgByOrgCode()
        {
            if (_ibusBankOrg == null)
            {
                _ibusBankOrg = new busOrganization();
            }
            _ibusBankOrg.FindOrganizationByOrgCode(icdoPersonAccountAchDetail.org_code);
        }
        public void LoadBankOrgByOrgID()
        {
            if (_ibusBankOrg == null)
            {
                _ibusBankOrg = new busOrganization();
            }
            _ibusBankOrg.FindOrganization(icdoPersonAccountAchDetail.bank_org_id);
        }
       
	}
}
