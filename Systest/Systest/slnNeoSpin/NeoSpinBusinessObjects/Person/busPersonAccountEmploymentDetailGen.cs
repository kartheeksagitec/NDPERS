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
	public class busPersonAccountEmploymentDetailGen : busExtendBase
    {
		public busPersonAccountEmploymentDetailGen()
		{

		}
        private busPlan _ibusPlan;

        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }
        private busPersonAccount  _ibusPersonAccount;

        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }

		private cdoPersonAccountEmploymentDetail _icdoPersonAccountEmploymentDetail;
		public cdoPersonAccountEmploymentDetail icdoPersonAccountEmploymentDetail
		{
			get
			{
				return _icdoPersonAccountEmploymentDetail;
			}
			set
			{
				_icdoPersonAccountEmploymentDetail = value;
			}
		}

		public bool FindPersonAccountEmploymentDetail(int Aintpersonaccountemploymentdtlid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountEmploymentDetail == null)
			{
				_icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail();
			}
			if (_icdoPersonAccountEmploymentDetail.SelectRow(new object[1] { Aintpersonaccountemploymentdtlid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        public DataTable idtbPlanCacheData { get; set; }

        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            if (idtbPlanCacheData != null)
                _ibusPlan.idtbPlanCacheData = idtbPlanCacheData;
            _ibusPlan.FindPlan(_icdoPersonAccountEmploymentDetail.plan_id);
        }
        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }
            _ibusPersonAccount.FindPersonAccount(icdoPersonAccountEmploymentDetail.person_account_id);
        }
        public DateTime idtmSuspendEffectiveDate { get; set; }
        public string istrSuspendManually { get; set; }
        public string istrTerminationStatusValue { get; set; }
        public bool iblnIsEmploymentEnded { get; set; }
        public bool VRShowPlanSuspensionStatus()
        {
            if (iblnIsEmploymentEnded || istrTerminationStatusValue == busConstant.EmploymentChangeRequestStatusProcessed)
                return true;
            else
                return false;
        }
    }
}
