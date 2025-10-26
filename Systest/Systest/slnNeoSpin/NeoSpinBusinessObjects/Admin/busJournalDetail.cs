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
	public partial class busJournalDetail 
	{
        private busJournalHeader _ibusJournalHeader;
        public busJournalHeader ibusJournalHeader
        {
            get { return _ibusJournalHeader; }
            set { _ibusJournalHeader = value; }
        }

        //PIR - 139
        private string _istrSuppressWarning;
        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }

        public void LoadJournalHeader()
        {
            if (_ibusJournalHeader == null)
            {
                _ibusJournalHeader = new busJournalHeader();
            }
            _ibusJournalHeader.FindJournalHeader(_icdoJournalDetail.journal_header_id);
        }

        // PIR ID 119 - To Display Header Debit and Credit Amount.
        public void LoadHeaderDebitsAndCredits()
        {
            busJournalHeader lobjJournalHeader = new busJournalHeader();
            lobjJournalHeader.icolJournalDetail = new Collection<busJournalDetail>();
            lobjJournalHeader.FindJournalHeader(_icdoJournalDetail.journal_header_id);
            lobjJournalHeader.LoadJournalDetails();
            lobjJournalHeader.LoadTotalDebitandCredit();
            _ibusJournalHeader.ldclTotalCredits = lobjJournalHeader.ldclTotalCredits;
            _ibusJournalHeader.ldclTotalDebits = lobjJournalHeader.ldclTotalDebits;
        }

        public void LoadOtherJournalEntryDetails()
        {
            DataTable ldtbDetail = Select("cdoJournalDetail.LoadOtherDetail", new object[2] {_icdoJournalDetail.journal_header_id,_icdoJournalDetail.journal_detail_id});
            _ibusJournalHeader.icolJournalDetail = new Collection<busJournalDetail>();
            _ibusJournalHeader.icolJournalDetail = GetCollection<busJournalDetail>(ldtbDetail, "icdoJournalDetail");
            foreach (busJournalDetail lobjDetail in _ibusJournalHeader.icolJournalDetail)
            {                
                lobjDetail.icdoJournalDetail.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjDetail.icdoJournalDetail.org_id);
                lobjDetail.icdoJournalDetail.istrAccountNo = lobjDetail.LoadAccountNo(lobjDetail.icdoJournalDetail.account_id);
                lobjDetail.LoadPlan();
            }
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(_icdoJournalDetail.plan_id);
        }

        private string _istrGLType;
        public string istrGLType
        {
            get 
            {
                if (_icdoJournalDetail.debit_amount > 0)
                    _istrGLType = busConstant.JournalDetailGLTypeDebit;
                else if (_icdoJournalDetail.credit_amount > 0)
                    _istrGLType = busConstant.JournalDetailGLTypeCredit;
                return _istrGLType;
            }
        }

        private decimal _idclAmount;
        public decimal idclAmount
        {
            get 
            {
                if (_icdoJournalDetail.debit_amount > 0)
                    _idclAmount = _icdoJournalDetail.debit_amount;
                else if (_icdoJournalDetail.credit_amount > 0)
                    _idclAmount = _icdoJournalDetail.credit_amount;
                return _idclAmount; 
            }
        }

        public string istrPERSLinkID
        {
            get
            {
                string lstrPERSLinkID = string.Empty;
                if (_icdoJournalDetail.person_id != 0)
                    lstrPERSLinkID = Convert.ToString(_icdoJournalDetail.person_id);
                return lstrPERSLinkID;
            }
        }

        public string LoadAccountNo(int AintAccountID)
        {
            String lstrAccountNo = string.Empty;
            busChartOfAccount lobjChartOfAccount = new busChartOfAccount();
            lobjChartOfAccount.FindChartOfAccount(AintAccountID);
            lstrAccountNo = lobjChartOfAccount.icdoChartOfAccount.gl_account_number;
            return lstrAccountNo;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadHeaderDebitsAndCredits();
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            _icdoJournalDetail.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(_icdoJournalDetail.istrOrgCodeID);
            //Load Account ID
            if (_icdoJournalDetail.istrAccountNo != null)
            {
                busChartOfAccount lobjChartOfAccount = new busChartOfAccount();
                _icdoJournalDetail.account_id = lobjChartOfAccount.GetAcctIDByAcctNo(_icdoJournalDetail.istrAccountNo);
            }
            base.BeforeValidate(aenmPageMode);
        }

        public bool IsSourceTypeVaried()
        {
            if (_ibusJournalHeader.icolJournalDetail == null)
                LoadOtherJournalEntryDetails();
            foreach (busJournalDetail lobjDetail in _ibusJournalHeader.icolJournalDetail)
            {
                if (lobjDetail.icdoJournalDetail.source_type_value != icdoJournalDetail.source_type_value)
                    return true;
            }
            return false;
        }

        /// PIR ID 122
        public bool IsValidEntry()
        {
            bool lblnFlag = false;
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            lclbCodeValue = GetCodeValue(1305);
            foreach (cdoCodeValue lcdoCodeValue in lclbCodeValue)
            {
                if ((lcdoCodeValue.data1 == Convert.ToString(icdoJournalDetail.plan_id)) &&
                   (lcdoCodeValue.data2 == icdoJournalDetail.fund_value) &&
                   (lcdoCodeValue.data3 == icdoJournalDetail.dept_value))
                {
                    lblnFlag = true;
                    break;
                }
            }
            return lblnFlag;
        }
	}
}
