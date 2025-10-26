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
	public class busBenefitApplicationPersonAccountGen : busExtendBase
    {
		public busBenefitApplicationPersonAccountGen()
		{

		}

		private cdoBenefitApplicationPersonAccount _icdoBenefitApplicationPersonAccount;
		public cdoBenefitApplicationPersonAccount icdoBenefitApplicationPersonAccount
		{
			get
			{
				return _icdoBenefitApplicationPersonAccount;
			}
			set
			{
				_icdoBenefitApplicationPersonAccount = value;
			}
		}

		private busPersonAccount _ibusPersonAccount;
		public busPersonAccount ibusPersonAccount
		{
			get
			{
				return _ibusPersonAccount;
			}
			set
			{
				_ibusPersonAccount = value;
			}
		}

        public busBenefitApplication ibusBenefitApplication { get; set; }
        public busBenefitRefundApplication ibusBenefitRefundApplication { get; set; }
		public bool FindBenefitApplicationPersonAccount(int Aintbenefitapplicationpersonaccountid)
		{
			bool lblnResult = false;
			if (_icdoBenefitApplicationPersonAccount == null)
			{
				_icdoBenefitApplicationPersonAccount = new cdoBenefitApplicationPersonAccount();
			}
			if (_icdoBenefitApplicationPersonAccount.SelectRow(new object[1] { Aintbenefitapplicationpersonaccountid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadPersonAccount()
		{
			if (_ibusPersonAccount == null)
			{
				_ibusPersonAccount = new busPersonAccount();
			}
			_ibusPersonAccount.FindPersonAccount(_icdoBenefitApplicationPersonAccount.person_account_id);
		}

        public void LoadBenefitApplication()
        {
            if (ibusBenefitApplication.IsNull())
                ibusBenefitApplication = new busBenefitApplication();
            ibusBenefitApplication.FindBenefitApplication(icdoBenefitApplicationPersonAccount.benefit_application_id);
            if (ibusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (ibusBenefitRefundApplication == null)
                    ibusBenefitRefundApplication = new busBenefitRefundApplication();
                ibusBenefitRefundApplication.FindBenefitApplication(icdoBenefitApplicationPersonAccount.benefit_application_id);
            }
        }

        public busPayeeAccount ibusPayeeAccount { get; set; }

        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoBenefitApplicationPersonAccount.payee_account_id);
        }
	}
}
