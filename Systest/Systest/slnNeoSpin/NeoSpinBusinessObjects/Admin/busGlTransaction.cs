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
	public partial class busGLTransaction
    {
        #region G/L File For PeopleSoft

        private int _lintFiscalPeriod;
        public int lintFiscalPeriod
        {
            get { return _lintFiscalPeriod; }
            set { _lintFiscalPeriod = value; }
        }

        public string lstrFormattedPeriod
        {
            get { return _lintFiscalPeriod.ToString("00"); }
        }	

        private string _lstrAccountNumber;
        public string lstrAccountNumber
        {
            get { return _lstrAccountNumber; }
            set { _lstrAccountNumber = value; }
        }

        public string Filler50
        {
            get { return "                                                  "; }
        }
        
        private decimal _ldclTotalAmount;
        public decimal ldclTotalAmount
        {
            get { return _ldclTotalAmount; }
            set { _ldclTotalAmount = value; }
        }

        public string Filler5
        {
            get { return "     "; }
        }

        public void LoadFiscalPeriod()
        {
            if (lintPostingMonth <= 6)
                _lintFiscalPeriod = lintPostingMonth + 6;
            else
                _lintFiscalPeriod = lintPostingMonth - 6;
        }

        public void LoadAccountNumber()
        {
            busChartOfAccount lobjChartOfAccount = new busChartOfAccount();
            lobjChartOfAccount.FindChartOfAccount(_icdoGlTransaction.account_id);
            _lstrAccountNumber = lobjChartOfAccount.icdoChartOfAccount.gl_account_number;
        }

        private string _lstrPlanCode;
        public string lstrPlanCode
        {
            get { return _lstrPlanCode; }
            set { _lstrPlanCode = value; }
        }

        public void LoadPlanCode()
        {
            busPlan lobjPlan = new busPlan();
            lobjPlan.FindPlan(_icdoGlTransaction.plan_id);
            _lstrPlanCode= lobjPlan.icdoPlan.plan_code;
        }

        public int lintPostingMonth
        {
            get { return _icdoGlTransaction.posting_date.Month; }
        }

        public string lstrPostingMonth
        {
            get { return _icdoGlTransaction.posting_date.Month.ToString("00"); }
        }

        public int lintPostingYear
        {
            get { return _icdoGlTransaction.posting_date.Year; }
        }

        public DateTime ldteBatchRunDate { get; set; }

        public DateTime ldteBatchRunDateNoNull
        {
            get
            {
                if (ldteBatchRunDate == DateTime.MinValue)
                    return DateTime.Today;
                return ldteBatchRunDate;
            }
        }

        public string lstrJournalDate
        {
            //PROD Pir - 4585
            get 
            {
                if (_icdoGlTransaction.extract_date == DateTime.MinValue)
                    return ldteBatchRunDateNoNull.ToString("MM/dd/yyyy");
                else
                    return _icdoGlTransaction.extract_date.ToString("MM/dd/yyyy");
            }
        }
                
        public string lstrDescription
        {
            //PROD Pir - 4585
            get 
            {
                string lstrDesc = busConstant.GLFileOutDescription;
                if (_icdoGlTransaction.extract_date == DateTime.MinValue)
                    lstrDesc += ldteBatchRunDateNoNull.ToString("MMddyy");
                else
                    lstrDesc += _icdoGlTransaction.extract_date.ToString("MMddyy");
                lstrDesc += " " + _icdoGlTransaction.posting_date.ToString("MM/dd/yyyy");
                return lstrDesc;
            }
        }

        #endregion

        private string _lstrOrgCodeID;
        public string lstrOrgCodeID
        {
            get { return _lstrOrgCodeID; }
            set { _lstrOrgCodeID = value; }
        }

        private string _lstrPlanName;
        public string lstrPlanName
        {
            get { return _lstrPlanName; }
            set { _lstrPlanName = value; }
        }

        public void LoadSourceIDDerived()
        {
            if (icdoGlTransaction.source_type_value == busConstant.GLSourceTypeValueBenefitPayment ||
                   icdoGlTransaction.source_type_value == busConstant.GLSourceTypeValueInsuranceTransfer ||
                   icdoGlTransaction.source_type_value == busConstant.GLSourceTypeValueVendorPayment)
            {
                busPaymentHistoryDetail lobjDetail = new busPaymentHistoryDetail();
                lobjDetail.FindPaymentHistoryDetail(icdoGlTransaction.source_id);
                icdoGlTransaction.source_id_derived = lobjDetail.icdoPaymentHistoryDetail.payment_history_header_id;
            }
            else if (icdoGlTransaction.source_type_value == busConstant.GLSourceTypeValueChkMaintenance)
            {
                busAccountReference lobjAccountReference = new busAccountReference();
                lobjAccountReference.FindAccountReference(icdoGlTransaction.account_reference_id);
                if (lobjAccountReference.icdoAccountReference.status_transition_value == busConstant.GLStatusTransitionPaymentHistory)
                {
                    busPaymentHistoryDetail lobjDetail = new busPaymentHistoryDetail();
                    lobjDetail.FindPaymentHistoryDetail(icdoGlTransaction.source_id);
                    icdoGlTransaction.source_id_derived = lobjDetail.icdoPaymentHistoryDetail.payment_history_header_id;
                }
                else
                    icdoGlTransaction.source_id_derived = icdoGlTransaction.source_id;
            }
            else
                icdoGlTransaction.source_id_derived = icdoGlTransaction.source_id;
        }
    }
}
