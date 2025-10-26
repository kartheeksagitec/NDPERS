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
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	public partial class busAccountReference : busExtendBase
    {
        private string _lstrPlanName;
        public string lstrPlanName
        {
            get { return _lstrPlanName; }
            set { _lstrPlanName = value; }
        }

        //PIR - 137
        private string _istrSuppressWarning;
        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }

        /*** PIR - 350 ***/

        private busChartOfAccount _ibusDebitAccount;
        public busChartOfAccount ibusDebitAccount
        {
            get { return _ibusDebitAccount; }
            set { _ibusDebitAccount = value; }
        }

        private busChartOfAccount _ibusCreditAccount;
        public busChartOfAccount ibusCreditAccount
        {
            get { return _ibusCreditAccount; }
            set { _ibusCreditAccount = value; }
        }

        public override void BeforePersistChanges()
        {
            LoadAccountID();
            base.BeforePersistChanges();
        }

        public void LoadAccountID()
        {
            _ibusCreditAccount.LoadByAccountNo(_ibusCreditAccount.icdoChartOfAccount.gl_account_number);
            _ibusDebitAccount.LoadByAccountNo(_ibusDebitAccount.icdoChartOfAccount.gl_account_number);
            _icdoAccountReference.debit_account_id = _ibusDebitAccount.icdoChartOfAccount.chart_of_account_id;
            _icdoAccountReference.credit_account_id = _ibusCreditAccount.icdoChartOfAccount.chart_of_account_id;
        }

        public void LoadAccountNo()
        {
            if (_ibusDebitAccount == null)
            {
                _ibusDebitAccount = new busChartOfAccount();
            }
            _ibusDebitAccount.FindChartOfAccount(_icdoAccountReference.debit_account_id);
            if (_ibusCreditAccount == null)
            {
                _ibusCreditAccount = new busChartOfAccount();
            }
            _ibusCreditAccount.FindChartOfAccount(_icdoAccountReference.credit_account_id);
        }

        /// PIR ID 122
        public bool IsValidEntry()
        {
            bool lblnFlag = false;
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            lclbCodeValue = GetCodeValue(1305);
            foreach (cdoCodeValue lcdoCodeValue in lclbCodeValue)
            {
                if ((lcdoCodeValue.data1 == Convert.ToString(icdoAccountReference.plan_id)) &&
                   (lcdoCodeValue.data2 == icdoAccountReference.fund_value) &&
                   (lcdoCodeValue.data3 == icdoAccountReference.dept_value))
                {
                    lblnFlag = true;
                    break;
                }
            }
            return lblnFlag;
        }
	}
}
