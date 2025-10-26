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
	public class busBenefitCalculationPersonAccountGen : busExtendBase
    {
		public busBenefitCalculationPersonAccountGen()
		{

		}
        public busBenefitRefundCalculation ibusBenefitRefundCalculation { get; set; }

        public void LoadBenefitRefundCalculation()
        {
            if (ibusBenefitRefundCalculation == null)
                ibusBenefitRefundCalculation = new busBenefitRefundCalculation();
            ibusBenefitRefundCalculation.FindBenefitRefundCalculation(icdoBenefitCalculationPersonAccount.benefit_calculation_id);
        }

        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }

        public void LoadPersonAccount()
        {
            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount();
            ibusPersonAccount.FindPersonAccount(icdoBenefitCalculationPersonAccount.person_account_id);
        }

        public busPayeeAccount ibusPayeeAccount { get; set; }

        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoBenefitCalculationPersonAccount.payee_account_id);
        }

		private cdoBenefitCalculationPersonAccount _icdoBenefitCalculationPersonAccount;
		public cdoBenefitCalculationPersonAccount icdoBenefitCalculationPersonAccount
		{
			get
			{
				return _icdoBenefitCalculationPersonAccount;
			}
			set
			{
				_icdoBenefitCalculationPersonAccount = value;
			}
		}

		public bool FindBenefitCalculationPersonAccount(int Aintbenefitcalculationpersonaccountid)
		{
			bool lblnResult = false;
			if (_icdoBenefitCalculationPersonAccount == null)
			{
				_icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount();
			}
			if (_icdoBenefitCalculationPersonAccount.SelectRow(new object[1] { Aintbenefitcalculationpersonaccountid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
