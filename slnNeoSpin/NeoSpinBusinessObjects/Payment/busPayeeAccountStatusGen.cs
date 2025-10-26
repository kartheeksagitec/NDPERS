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
    public class busPayeeAccountStatusGen : busExtendBase
    {
        public busPayeeAccountStatusGen()
        {

        }
        private DateTime _idtNextPaymentDate;

        public DateTime idtNextPaymentDate
        {
            get { return _idtNextPaymentDate; }
            set { _idtNextPaymentDate = value; }
        }

        private cdoPayeeAccountStatus _icdoPayeeAccountStatus;
        public cdoPayeeAccountStatus icdoPayeeAccountStatus
        {
            get
            {
                return _icdoPayeeAccountStatus;
            }
            set
            {
                _icdoPayeeAccountStatus = value;
            }
        }

        public bool FindPayeeAccountStatus(int Aintpayeeaccountstatusid)
        {
            bool lblnResult = false;
            if (_icdoPayeeAccountStatus == null)
            {
                _icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
            }
            if (_icdoPayeeAccountStatus.SelectRow(new object[1] { Aintpayeeaccountstatusid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        private busPayeeAccount _ibusPayeeAccount;
        public busPayeeAccount ibusPayeeAccount
        {
            get { return _ibusPayeeAccount; }
            set { _ibusPayeeAccount = value; }
        }

        public void LoadPayeeAccount()
        {
            if (_ibusPayeeAccount == null)
            {
                _ibusPayeeAccount = new busPayeeAccount();
            }
            _ibusPayeeAccount.FindPayeeAccount(icdoPayeeAccountStatus.payee_account_id);
        }

        private busPayeeAccountStatus _ibusPreviousStatus;
        public busPayeeAccountStatus ibusPreviousStatus
        {
            get { return _ibusPreviousStatus; }
            set { _ibusPreviousStatus = value; }
        }

        public bool IsApprovedOrReceiving
        {
            get
            {
                if ((icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved) ||
                    (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentRecieving))
                    return true;
                else
                    return false;
            }
        }
    }
}
