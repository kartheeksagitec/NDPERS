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
	public partial class busJournalHeader 
	{
        private decimal _ldclTotalDebits;
        public decimal ldclTotalDebits
        {
            get { return _ldclTotalDebits; }
            set { _ldclTotalDebits = value; }
        }

        private decimal _ldclTotalCredits;
        public decimal ldclTotalCredits
        {
            get { return _ldclTotalCredits; }
            set { _ldclTotalCredits = value; }
        }

        private DateTime _ldtValidDate;
        public DateTime ldtValidDate
        {
            get { return _ldtValidDate; }
            set { _ldtValidDate = value; }
        }

        private int _lintJournalEntry;
        public int lintJournalEntry
        {
            get { return _lintJournalEntry; }
            set { _lintJournalEntry = value; }
        }

        // PIR ID 127 - To Suppress Warning while posting.
        private string _lstrSuppressWarningFlag;
        public string lstrSuppressWarningFlag
        {
            get { return _lstrSuppressWarningFlag; }
            set { _lstrSuppressWarningFlag = value; }
        }

        public ArrayList btnPost_Click()
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;

            if (!IsValidAmount())
            {
                lobjError = AddError(4626, "");
                larrErrors.Add(lobjError);
            }
            if (!IsValidUser())
            {
                lobjError = AddError(4627, "");
                larrErrors.Add(lobjError);
            }
            if (!IsValidPostingDate())
            {
                lobjError = AddError(4628, "");
                larrErrors.Add(lobjError);
            }
            if ((!IsAmountBalancedByFund()) && (_lstrSuppressWarningFlag != busConstant.Flag_Yes))      // PIR ID - 127 
            {
                lobjError = AddError(4709, "");
                larrErrors.Add(lobjError);
            }
            if (larrErrors.Count == 0)
            {
                InsertGLTransaction();
                _icdoJournalHeader.journal_status_value = busConstant.JournalHeaderStatusPosted;
                _icdoJournalHeader.Update();
                //Reload the Object to Refresh the Screen
                FindJournalHeader(_icdoJournalHeader.journal_header_id);
                larrErrors.Add(this);
                //Reload the Initial Load Rules to Hide the Button
                EvaluateInitialLoadRules();
            }
            return larrErrors;
        }

        // PIR ID 125 - User who created the Journal Details also cant post the record.
        public bool IsValidUser()
        {
            if (_icdoJournalHeader.created_by != iobjPassInfo.istrUserID)
            {
                if (_icolJournalDetail == null)
                    LoadJournalDetails();
                foreach (busJournalDetail lobjJournalDetail in _icolJournalDetail)
                {
                    if (lobjJournalDetail.icdoJournalDetail.created_by == iobjPassInfo.istrUserID)
                        return false;
                } 
                return true;
            }
            else
                return false;
        }

        public bool IsValidAmount()
        {
            if (_ldclTotalCredits == 0 || _ldclTotalDebits == 0) //PIR 9745
                LoadTotalDebitandCredit();
            if ((_ldclTotalCredits==_ldclTotalDebits) && (_ldclTotalCredits!=0) &&(_ldclTotalDebits!=0))
                return true;
            else 
                return false;
        }

        public bool IsValidPostingDate()
        {
            busCode lobjCode = new busCode();
            lobjCode.FindCode(busConstant.SystemConstantsAndVariablesCodeID);
            lobjCode.LoadCodeValues();
            foreach (busCodeValue lobjCodeValue in lobjCode.iclbCodeValue)
            {
                if(lobjCodeValue.icdoCodeValue.code_value==busConstant.JournalEntryCutoffDay)
                   lintJournalEntry=Convert.ToInt32( lobjCodeValue.icdoCodeValue.data1);
            }
            if (DateTime.Now.Day > lintJournalEntry )
            {
                ldtValidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
            }
            else
            {
                if (DateTime.Now.Month == 1)
                {
                    ldtValidDate = new DateTime(DateTime.Now.Year - 1, 12, 01);
                }
                else
                {
                    ldtValidDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 01);
                }
            }
            if ((_icdoJournalHeader.posting_date < ldtValidDate) ||(_icdoJournalHeader.posting_date>DateTime.Now))
                return false;
            else
                return true;
        }

        // PIR ID 127 - To check whether the amount is balanced by fund.
        public bool IsAmountBalancedByFund()
        {
            DataTable ldtbAmount = Select("cdoJournalDetail.GetAmountGroupByFundValue", new object[1] { _icdoJournalHeader.journal_header_id });
            foreach (DataRow dr in ldtbAmount.Rows)
            {
                if ( Convert.ToInt32(dr["AMOUNT"]) == 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public void LoadTotalDebitandCredit()
        {
            if (_icolJournalDetail == null)
                LoadJournalDetails();
            foreach (busJournalDetail lobjJournalDetail in _icolJournalDetail)
            {
                _ldclTotalDebits += lobjJournalDetail.icdoJournalDetail.debit_amount;
                _ldclTotalCredits += lobjJournalDetail.icdoJournalDetail.credit_amount;
            }
        }
        
        public void InsertGLTransaction()
        {
            if (_icolJournalDetail == null)
                LoadJournalDetails();
            foreach (busJournalDetail lobjJournalDetail in _icolJournalDetail)
            {
                busGLTransaction lobjGLTransaction = new busGLTransaction();
                lobjGLTransaction.icdoGlTransaction = new cdoGlTransaction();
                lobjGLTransaction.icdoGlTransaction.plan_id = lobjJournalDetail.icdoJournalDetail.plan_id;
                lobjGLTransaction.icdoGlTransaction.fund_value = lobjJournalDetail.icdoJournalDetail.fund_value;
                lobjGLTransaction.icdoGlTransaction.dept_value = lobjJournalDetail.icdoJournalDetail.dept_value;
                lobjGLTransaction.icdoGlTransaction.source_type_value = busConstant.SourceTypeJournalHeader;
                lobjGLTransaction.icdoGlTransaction.source_id = lobjJournalDetail.icdoJournalDetail.journal_header_id;
                lobjGLTransaction.icdoGlTransaction.account_id = lobjJournalDetail.icdoJournalDetail.account_id;
                lobjGLTransaction.icdoGlTransaction.person_id = lobjJournalDetail.icdoJournalDetail.person_id;
                lobjGLTransaction.icdoGlTransaction.org_id = lobjJournalDetail.icdoJournalDetail.org_id;
                lobjGLTransaction.icdoGlTransaction.debit_amount = lobjJournalDetail.icdoJournalDetail.debit_amount;
                lobjGLTransaction.icdoGlTransaction.credit_amount = lobjJournalDetail.icdoJournalDetail.credit_amount;
                lobjGLTransaction.icdoGlTransaction.effective_date = lobjJournalDetail.icdoJournalDetail.effective_date;
                lobjGLTransaction.icdoGlTransaction.posting_date = _icdoJournalHeader.posting_date;
                lobjGLTransaction.icdoGlTransaction.journal_description = lobjJournalDetail.icdoJournalDetail.journal_description;
                lobjGLTransaction.icdoGlTransaction.created_by = iobjPassInfo.istrUserID;
                lobjGLTransaction.icdoGlTransaction.created_date = DateTime.Now;
                lobjGLTransaction.icdoGlTransaction.modified_by = iobjPassInfo.istrUserID;
                lobjGLTransaction.icdoGlTransaction.modified_date = DateTime.Now;
                lobjGLTransaction.icdoGlTransaction.update_seq = 0;
                lobjGLTransaction.icdoGlTransaction.Insert();
            }
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busJournalDetail)
            {
                busJournalDetail lobjJournalDetail = (busJournalDetail)aobjBus;
                lobjJournalDetail.icdoJournalDetail.istrAccountNo = lobjJournalDetail.LoadAccountNo(lobjJournalDetail.icdoJournalDetail.account_id);
                lobjJournalDetail.icdoJournalDetail.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjJournalDetail.icdoJournalDetail.org_id);
                lobjJournalDetail.LoadPlan();
            }
        }

        public override void BeforePersistChanges()
        {
            if (_icdoJournalHeader.ienuObjectState == ObjectState.Insert)
            {
                _icdoJournalHeader.journal_status_value = busConstant.JournalHeaderStatusEntered;
            }
            base.BeforePersistChanges();
        }
	}
}
