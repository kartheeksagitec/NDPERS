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
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitRefundCalculationGen : busBenefitCalculation
    {
        public busBenefitRefundCalculationGen()
        {

        }
        //This properties used for Refund calculation
        public decimal dro_ee_pretax_amount { get; set; }
        public decimal dro_ee_posttax_amount { get; set; }
        public decimal dro_ee_er_pickup_amount { get; set; }
        public decimal dro_er_vested_amount { get; set; }
        public decimal dro_interest_amount { get; set; }
        public decimal dro_capital_gain { get; set; }
        public decimal dro_additional_interest { get; set; }

        //this property used for displaying payee account id in calculation details panel of the application maintenance screen        
        public int iintPayeeAccountID { get; set; }

        public void LoadPayeeAccountID(bool ablnLoad = false)
        {
            if (iclbPayeeAccount == null || ablnLoad)
                LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                if (!lobjPayeeAccount.IsitRefundRHICPayeeAccount())
                {
                    iintPayeeAccountID = lobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                }
            }
        }

        private bool _iblnAdditionalContributionsReportedFlag;
        public bool iblnAdditionalContributionsReportedFlag
        {
            get
            {
                return _iblnAdditionalContributionsReportedFlag;
            }
            set
            {
                _iblnAdditionalContributionsReportedFlag = value;
            }
        }
        public bool IsBenefitOptionRegularRefundOrAutoRefund()
        {
            if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRegularRefund)
            {
                return true;
            }
            else if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionAutoRefund)
            {
                return true;
            }
            return false;
        }
        public bool IsBenefitOptionTFFR()
        {
            if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE)
            {
                return true;
            }
            else if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers)
            {
                return true;
            }
            return false;
        }
        public bool IsBenefitOptionTFFROrTIAA()
        {
            if (IsBenefitOptionTFFR())
            {
                return true;
            }
            else if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer)
            {
                return true;
            }
            return false;
        }
        private cdoBenefitRefundCalculation _icdoBenefitRefundCalculation;
        public cdoBenefitRefundCalculation icdoBenefitRefundCalculation
        {
            get
            {
                return _icdoBenefitRefundCalculation;
            }
            set
            {
                _icdoBenefitRefundCalculation = value;
            }
        }

        public bool FindBenefitRefundCalculation(int Aintbenefitrefundcalculationid)
        {
            bool lblnResult = false;
            if (_icdoBenefitRefundCalculation == null)
            {
                _icdoBenefitRefundCalculation = new cdoBenefitRefundCalculation();
            }
            if (icdoBenefitCalculation == null)
                icdoBenefitCalculation = new cdoBenefitCalculation();
            if (_icdoBenefitRefundCalculation.SelectRow(new object[1] { Aintbenefitrefundcalculationid }))
            {
                if (icdoBenefitCalculation.SelectRow(new object[1] { Aintbenefitrefundcalculationid }))
                {
                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        private busBenefitRefundApplication _ibusRefundBenefitApplication;

        public busBenefitRefundApplication ibusRefundBenefitApplication
        {
            get { return _ibusRefundBenefitApplication; }
            set { _ibusRefundBenefitApplication = value; }
        }
        public void LoadRefundBenefitApplication()
        {
            if (_ibusRefundBenefitApplication == null)
                _ibusRefundBenefitApplication = new busBenefitRefundApplication();
            _ibusRefundBenefitApplication.FindBenefitApplication(icdoBenefitCalculation.benefit_application_id);
        }
        public void LoadBenefitCalculationPersonAccount()
        {
            if (iclbBenefitCalculationPersonAccount == null)
                iclbBenefitCalculationPersonAccount = new Collection<busBenefitCalculationPersonAccount>();
            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            if (ibusBenefitApplication.iclbBenefitApplicationPersonAccounts == null)
                ibusBenefitApplication.LoadBenefitApplicationPersonAccount();
            foreach (busBenefitApplicationPersonAccount lobjBenefitApplicationPersonAccount in ibusBenefitApplication.iclbBenefitApplicationPersonAccounts)
            {
                busBenefitCalculationPersonAccount lobjBenefitCalculationPersonAccount = new busBenefitCalculationPersonAccount();
                lobjBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount();
                lobjBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id = lobjBenefitApplicationPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id;
                iclbBenefitCalculationPersonAccount.Add(lobjBenefitCalculationPersonAccount);
            }
        }

        public void CalculateTotalRefundAmountForRegulaOrAutoRefund()
        {
            if (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
                icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020) //PIR 20232
            {
                icdoBenefitRefundCalculation.total_refund_amount = icdoBenefitRefundCalculation.rhic_ee_amount;
            }
            else
            {
                icdoBenefitRefundCalculation.total_refund_amount = CalculateTotalAmount();
            }
        }
        public void CalculateTotalAmountForTransferOptions()
        {
            if (IsBenefitOptionTFFROrTIAA())
            {
                icdoBenefitRefundCalculation.total_refund_amount = icdoBenefitRefundCalculation.rhic_ee_amount;
            }
            icdoBenefitRefundCalculation.total_transfer_amount = CalculateTotalAmount();
            //PIR 25920 DC 2025 Changes Higher of the two amounts will be the Total Transfer Amount
            if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
                icdoBenefitRefundCalculation.total_transfer_amount = icdoBenefitRefundCalculation.calculated_actuarial_value > icdoBenefitRefundCalculation.db_account_balance ?
                icdoBenefitRefundCalculation.calculated_actuarial_value : icdoBenefitRefundCalculation.db_account_balance;
            
        }

        public decimal CalculateTotalAmount()
        {
            decimal ldecTotalAmount = 0.0M;
            decimal ldecERPreTaxAmount = 0.0M, ldecERInterestAmount = 0.0M;
            if (!IsBenefitOptionRegularRefundOrAutoRefund())
            {
                ldecERPreTaxAmount = icdoBenefitRefundCalculation.overridden_er_pre_tax_amount > 0 ? icdoBenefitRefundCalculation.overridden_er_pre_tax_amount : icdoBenefitRefundCalculation.er_pre_tax_amount;
                ldecERInterestAmount = icdoBenefitRefundCalculation.overridden_er_interest_amount > 0 ? icdoBenefitRefundCalculation.overridden_er_interest_amount : icdoBenefitRefundCalculation.er_interest_amount;
                ldecTotalAmount = icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount + icdoBenefitRefundCalculation.post_tax_ee_contribution_amount +
                    icdoBenefitRefundCalculation.ee_er_pickup_amount + icdoBenefitRefundCalculation.ee_interest_amount + icdoBenefitRefundCalculation.vested_er_contribution_amount +
                     ldecERPreTaxAmount + ldecERInterestAmount +
                     icdoBenefitRefundCalculation.additional_ee_amount + icdoBenefitRefundCalculation.additional_er_amount + icdoBenefitRefundCalculation.additional_er_amount_interest;
            }
            else
            {
                ldecTotalAmount = icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount + icdoBenefitRefundCalculation.post_tax_ee_contribution_amount +
                    icdoBenefitRefundCalculation.ee_er_pickup_amount + icdoBenefitRefundCalculation.ee_interest_amount + icdoBenefitRefundCalculation.vested_er_contribution_amount + icdoBenefitRefundCalculation.rhic_ee_amount;

            }
            return ldecTotalAmount;
        }
    }
}