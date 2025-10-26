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
    public class busPayeeAccountAchDetailGen : busExtendBase
    {
        public busPayeeAccountAchDetailGen()
        {

        }
        public bool IsPrimaryACHSelected
        {
            get
            {
                if(icdoPayeeAccountAchDetail.primary_account_flag==busConstant.Flag_Yes)
                {
                    return true;
                }
                return false;
            }           
        }

        public bool iblnIsFromMSS { get; set; }

        public string istrPercentage
        {
            get
            {
                if(icdoPayeeAccountAchDetail.percentage_of_net_amount == 0.0m && icdoPayeeAccountAchDetail.primary_account_flag==busConstant.Flag_No)
                {
                    return string.Empty;
                }
                return icdoPayeeAccountAchDetail.percentage_of_net_amount.ToString();
            }
        }
        public string istrAmount
        {
            get
            {
                if (icdoPayeeAccountAchDetail.partial_amount == 0.0m && icdoPayeeAccountAchDetail.primary_account_flag == busConstant.Flag_No)
                {
                    return string.Empty;
                }
                return icdoPayeeAccountAchDetail.partial_amount.ToString();
            }
        }
        private cdoPayeeAccountAchDetail _icdoPayeeAccountAchDetail;
        public cdoPayeeAccountAchDetail icdoPayeeAccountAchDetail
        {
            get
            {
                return _icdoPayeeAccountAchDetail;
            }
            set
            {
                _icdoPayeeAccountAchDetail = value;
            }
        }

        private cdoPayeeAccountAchDetail _icdoPayeeAccountAchDetailSel1;
        public cdoPayeeAccountAchDetail icdoPayeeAccountAchDetailSel1
        {
            get
            {
                return _icdoPayeeAccountAchDetailSel1;
            }
            set
            {
                _icdoPayeeAccountAchDetailSel1 = value;
            }
        }

        private cdoPayeeAccountAchDetail _icdoPayeeAccountAchDetailSel2;
        public cdoPayeeAccountAchDetail icdoPayeeAccountAchDetailSel2
        {
            get
            {
                return _icdoPayeeAccountAchDetailSel2;
            }
            set
            {
                _icdoPayeeAccountAchDetailSel2 = value;
            }
        }


        public bool FindPayeeAccountAchDetail(int Aintpayeeaccountachdetailid)
        {
            bool lblnResult = false;
            if (_icdoPayeeAccountAchDetail == null)
            {
                _icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail();
            }
            if (_icdoPayeeAccountAchDetail.SelectRow(new object[1] { Aintpayeeaccountachdetailid }))
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

		//PIR 18503
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }

        private busOrganization _ibusBankOrg;
        public busOrganization ibusBankOrg
        {
            get { return _ibusBankOrg; }
            set { _ibusBankOrg = value; }
        }


        public void LoadBankOrgByOrgCode()
        {
            if (_ibusBankOrg == null)
            {
                _ibusBankOrg = new busOrganization();
            }
            _ibusBankOrg.FindOrganizationByOrgCode(icdoPayeeAccountAchDetail.org_code);
        }
        public void LoadBankOrgByOrgID()
        {
            if (_ibusBankOrg == null)
            {
                _ibusBankOrg = new busOrganization();
            }
            _ibusBankOrg.FindOrganization(icdoPayeeAccountAchDetail.bank_org_id);
        }
        public void LoadPayeeAccount()
        {
            if (_ibusPayeeAccount == null)
            {
                _ibusPayeeAccount = new busPayeeAccount();
            }
            _ibusPayeeAccount.FindPayeeAccount(icdoPayeeAccountAchDetail.payee_account_id);
        }
        public bool AreFieldsReadOnly()
        {
            if(ibusPayeeAccount==null)
                LoadPayeeAccount();
            if(ibusPayeeAccount.idtNextBenefitPaymentDate==DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (icdoPayeeAccountAchDetail.ach_start_date!=DateTime.MinValue
                &&icdoPayeeAccountAchDetail.ach_start_date < ibusPayeeAccount.idtNextBenefitPaymentDate)
            {
                return true;
            }
            return false;
        }

        ////PIR 18503
        public void LoadBankOrgByRoutingNumber(string AistrRoutingNumber)
        {
            _ibusBankOrg = new busOrganization();
            _ibusBankOrg.FindOrganizationByRoutingNumber(AistrRoutingNumber);
        }

		//PIR 18503
        public void LoadPerson()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id);
        }

    }
}