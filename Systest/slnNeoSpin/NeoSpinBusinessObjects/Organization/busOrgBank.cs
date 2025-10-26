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
	public partial class busOrgBank : busExtendBase
    {

        private busOrganization _ibusBankOrg;
        public busOrganization ibusBankOrg
        {
            get
            {
                return _ibusBankOrg;
            }

            set
            {
                _ibusBankOrg = value;
            }
        }
        private Collection<busOrgBank> _iclbOtherOrgBank;
        public Collection<busOrgBank> iclbOtherOrgBank
        {
            get { return _iclbOtherOrgBank; }
            set { _iclbOtherOrgBank = value; }
        }
        public void LoadBankOrg()
        {
            if (_ibusBankOrg == null)
            {
                _ibusBankOrg = new busOrganization();
            }
            _ibusBankOrg.FindOrganization(_icdoOrgBank.bank_org_id);
        }
        public void LoadOtherOrgBank()
        {
            if (_iclbOtherOrgBank == null)
            {
                _iclbOtherOrgBank = new Collection<busOrgBank>();
            }
            DataTable ldtbOtherBankOrg = busNeoSpinBase.Select("cdoOrgBank.LoadOtherOrgBank", new object[2] { _icdoOrgBank.org_id, _icdoOrgBank.org_bank_id });
            _iclbOtherOrgBank = GetCollection<busOrgBank>(ldtbOtherBankOrg, "icdoOrgBank");
            foreach (busOrgBank lobjOrgBank in _iclbOtherOrgBank)
            {
                lobjOrgBank.LoadBankOrg();
            }
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadBankOrg();

        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //PIR 195
            _icdoOrgBank.bank_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoOrgBank.istrOrgCodeID);
            LoadBankOrg();           
            base.BeforeValidate(aenmPageMode);
        }        

	}
}
