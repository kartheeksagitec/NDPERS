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
	public class busPersonAccountBeneficiary : busPersonBeneficiary
	{
        private cdoPersonAccountBeneficiary _icdoPersonAccountBeneficiary;
        public cdoPersonAccountBeneficiary icdoPersonAccountBeneficiary
        {
            get
            {
                return _icdoPersonAccountBeneficiary;
            }
            set
            {
                _icdoPersonAccountBeneficiary = value;
            }
        }
            #region Marital Status Correspondence PER-0055
        public string istrPlanNameCaps
        {
            get
            {
                return string.IsNullOrEmpty(icdoPersonAccountBeneficiary.istrPlanName) ? string.Empty : icdoPersonAccountBeneficiary.istrPlanName.ToUpper();
            }
        }

        public string istrBeneficiaryTypeCaps
        {
            get 
            {
                return icdoPersonAccountBeneficiary.beneficiary_type_description.ToUpper();
            }
        }


        #endregion
        public bool FindPersonAccountBeneficiary(int Aintpersonaccountbeneficiaryid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountBeneficiary == null)
            {
                _icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
            }
            if (_icdoPersonAccountBeneficiary.SelectRow(new object[1] { Aintpersonaccountbeneficiaryid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindPersonAccountBeneficiary(int AintBeneficiaryID, int AintPersonAccountID)
        {
            bool lblnFlag = false;
            if (icdoPersonAccountBeneficiary == null)
                icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
            DataTable ldtbList = Select<cdoPersonAccountBeneficiary>(new string[2] { "beneficiary_id", "person_account_id" },
                                                                    new object[2] { AintBeneficiaryID, AintPersonAccountID }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoPersonAccountBeneficiary.LoadData(ldtbList.Rows[0]);
                lblnFlag = true;
            }
            return lblnFlag;
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
                _ibusPersonAccount = new busPersonAccount();
            _ibusPersonAccount.FindPersonAccount(_icdoPersonAccountBeneficiary.person_account_id);
        }
	}
}
