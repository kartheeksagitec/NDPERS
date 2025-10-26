using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.Common;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSTaxWithholding : busPayeeAccountTaxWithholding
    {
        public string istrIsAcknowledged { get; set; }
        public bool iblnExternalUser { get; set; } //PIR-18492
        public override void BeforePersistChanges()
        {
            UpdateAlreadyExistingTaxWithholding();
            base.BeforePersistChanges();
        }

        //PIR 14880 - Check future date taxwithholding is exist or not.
        public bool IsFutureDatedElectionExisting()
        {
            if (ibusPayeeAccount.iclbTaxWithholingHistory.IsNull())
                ibusPayeeAccount.LoadTaxWithHoldingHistory();
            var lenumOpenTaxWithholding = ibusPayeeAccount.iclbTaxWithholingHistory.Where(lobjTax => lobjTax.icdoPayeeAccountTaxWithholding.tax_identifier_value == icdoPayeeAccountTaxWithholding.tax_identifier_value
                && lobjTax.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == icdoPayeeAccountTaxWithholding.benefit_distribution_type_value &&
                lobjTax.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue).FirstOrDefault();
            if (lenumOpenTaxWithholding.IsNotNull())
            {
                if (icdoPayeeAccountTaxWithholding.start_date < lenumOpenTaxWithholding.icdoPayeeAccountTaxWithholding.start_date)
                    return true;
            }
            return false;
        }
        public void UpdateAlreadyExistingTaxWithholding()
        {
            if (ibusPayeeAccount.iclbTaxWithholingHistory.IsNull())
                ibusPayeeAccount.LoadTaxWithHoldingHistory();

            var lenum = ibusPayeeAccount.iclbTaxWithholingHistory.Where(lobjTax => lobjTax.icdoPayeeAccountTaxWithholding.tax_identifier_value == icdoPayeeAccountTaxWithholding.tax_identifier_value
                && lobjTax.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == icdoPayeeAccountTaxWithholding.benefit_distribution_type_value
                && busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountTaxWithholding.start_date, lobjTax.icdoPayeeAccountTaxWithholding.start_date, lobjTax.icdoPayeeAccountTaxWithholding.end_date));

            if (lenum.Count() > 0)
            {
                busPayeeAccountTaxWithholding lobjOldTaxWithholding = new busPayeeAccountTaxWithholding();
                lobjOldTaxWithholding = lenum.FirstOrDefault();

                lobjOldTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = icdoPayeeAccountTaxWithholding.start_date.AddDays(-1);
                lobjOldTaxWithholding.icdoPayeeAccountTaxWithholding.Update();
            }

            // PIR 9516 start
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType.IsNull())
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

            int itemTypeId = 0;
            bool lblnIsStateTax = true;

            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
            {
                lblnIsStateTax = false;
                if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
                {
                    itemTypeId = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount);
                }
                else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
                {
                    itemTypeId = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITRefundFederalTaxAmount);
                }
                else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD)
                {
                    itemTypeId = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITRMDFederalTaxAmount);
                }
            }
            else
            {
                lblnIsStateTax = true;
                if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
                {
                    itemTypeId = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount);
                }
                else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
                {
                    itemTypeId = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITRefundNDStateTaxAmount);
                }
                else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD)
                {
                    itemTypeId = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITRMDStateTaxAmount);
                }
            }

            var lenumFedState = ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(lobj => lobj.icdoPayeeAccountPaymentItemType.payment_item_type_id == itemTypeId
                                && lobj.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue);

            if (lenumFedState.Count() > 0)
            {
                busPayeeAccountPaymentItemType lobjOldPaymentItemType = new busPayeeAccountPaymentItemType();
                lobjOldPaymentItemType = lenumFedState.FirstOrDefault();

                lobjOldPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = icdoPayeeAccountTaxWithholding.start_date.AddDays(-1);
                lobjOldPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
            }
            if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
                InsertPayeeAccountPaymentItemType(ibusPayeeAccount, lblnIsStateTax);
            // PIR 9516 end
            //PIR-18492 : To Post message to message board           
            if (iblnExternalUser)
                busWSSHelper.PublishMSSMessage(0, 0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10319, iobjPassInfo), "Tax Withholding"), busConstant.WSS_MessageBoard_Priority_High,
                ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id);
        }

        // PIR 9516 - function to insert into payment item type
        private void InsertPayeeAccountPaymentItemType(busPayeeAccount AobjPayeeAccount, bool AblnIsStateTax)
        {
            busPayeeAccountPaymentItemType lobjPAPaymentItemType = new busPayeeAccountPaymentItemType();
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id = AobjPayeeAccount.icdoPayeeAccount.payee_account_id;
            if (AblnIsStateTax)
            {
                int lintVendorOrgId = busPayeeAccountHelper.GetStateTaxVendorID();

                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id
                    = AobjPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount);
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.amount = AobjPayeeAccount.icdoPayeeAccount.current_state_tax;
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id = lintVendorOrgId;
            }
            else
            {
                int lintVendorOrgId = busPayeeAccountHelper.GetFedTaxVendorID();

                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id
                    = AobjPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount);
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.amount = AobjPayeeAccount.icdoPayeeAccount.current_federal_tax;
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id = lintVendorOrgId;
            }
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.start_date = icdoPayeeAccountTaxWithholding.start_date;
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.Insert();
        }

        public bool IsStartDatePastDate()
        {
            if (icdoPayeeAccountTaxWithholding.start_date != DateTime.MinValue)
                return (icdoPayeeAccountTaxWithholding.start_date < DateTime.Now);
            return false;
        }


        //is record exists with same start date
        public bool iblnIsRecordExistsWithSameStartDate()
        {
            if (ibusPayeeAccount.iclbTaxWithholingHistory.IsNull())
                ibusPayeeAccount.LoadTaxWithHoldingHistory();

            var lenum = ibusPayeeAccount.iclbTaxWithholingHistory.Where(lobjTax => lobjTax.icdoPayeeAccountTaxWithholding.tax_identifier_value == icdoPayeeAccountTaxWithholding.tax_identifier_value
                && lobjTax.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == icdoPayeeAccountTaxWithholding.benefit_distribution_type_value
                && busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountTaxWithholding.start_date, lobjTax.icdoPayeeAccountTaxWithholding.start_date, lobjTax.icdoPayeeAccountTaxWithholding.end_date));

            if (lenum.Count() > 0)
            {
                if (icdoPayeeAccountTaxWithholding.start_date == lenum.FirstOrDefault().icdoPayeeAccountTaxWithholding.start_date)
                    return true;
            }
            return false;
        }

        public Collection<busPayeeAccountTaxWithholdingItemDetail> iclbMSSTaxWithHoldingTaxItems { get; set; }
        public ArrayList btnMSSCalculateTax()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                iblnIsCalculateButtonClicked = true;
                CalculateTax(true, false);
                iblnIsCalculateButtonClicked = false;
                alReturn.Add(this);
                this.EvaluateInitialLoadRules();
                iclbMSSTaxWithHoldingTaxItems = iclbTaxWithHoldingTaxItems;
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }

        public override void BeforeValidate(Sagitec.Common.utlPageMode aenmPageMode)
        {
            // PIR 9516
            // When choosing 'No ND Tax', the Allowances and Additional Tax Amount should blank out on save.
            if (icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.StateTaxOptionNoMonthlyNDTax)
            {
                icdoPayeeAccountTaxWithholding.marital_status_value = string.Empty;
                icdoPayeeAccountTaxWithholding.no_of_tax_allowance = Convert.ToString(0);
                icdoPayeeAccountTaxWithholding.additional_tax_amount = 0M;
            }
            base.BeforeValidate(aenmPageMode);
        }
        public override int PersistChanges()
        {
            return base.PersistChanges();
        }
        public override void AfterPersistChanges()
        {
            ibusPayeeAccount.iblnIsMSSTaxWithholding = true; //PIR 20387
            base.AfterPersistChanges();
        }
        public override void ValidateHardErrors(Sagitec.Common.utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            //For MSS Layout change
            foreach (utlError lobjErr in iarrErrors)
            {
                if (lobjErr.istrErrorID == "4032")
                {
                    lobjErr.istrErrorMessage = "Start Date is mandatory ";
                }
                //pir 8670
                else if (lobjErr.istrErrorID == "5556")
                {
                    if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                        lobjErr.istrErrorMessage = "If additional tax amount is entered then select tax option of \"Based on IRS Table and Additional Tax\"";
                    else
                        lobjErr.istrErrorMessage = "If additional tax amount is entered then select tax option of \"From State Table and Additional Tax\"";
                }
            }
        }
        public override bool ValidateSoftErrors()
        {
            return base.ValidateSoftErrors();
        }
        //PIR 21227
        public bool IsMSSStartDateNotValid()
        {
            DateTime ldteSystBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
            return (ldteSystBatchDate.Day < 21) ? (icdoPayeeAccountTaxWithholding.start_date != DateTime.MinValue && icdoPayeeAccountTaxWithholding.start_date.Date < busGlobalFunctions.GetFirstDayofNextMonth(ldteSystBatchDate).Date) :
             (icdoPayeeAccountTaxWithholding.start_date != DateTime.MinValue && icdoPayeeAccountTaxWithholding.start_date.Date < busGlobalFunctions.GetFirstDayofNextMonth(ldteSystBatchDate.AddMonths(1)).Date);

        }
        public string istrConfirmationText
        {
            get
            {
                string luserName = ibusPayeeAccount.ibusPayee.istrFullName;
                DateTime Now = DateTime.Now;
                DataTable ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='CONF'");
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
                string lstrConfimation = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, luserName, Now);
                return lstrConfimation;
            }
        }
        private void LoadTaxRef()
        {
            string lstrTaxRef = icdoPayeeAccountTaxWithholding.tax_ref;
            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax && string.IsNullOrEmpty(lstrTaxRef))
                lstrTaxRef = busConstant.PayeeAccountTaxRefFed22Tax;
            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax && string.IsNullOrEmpty(lstrTaxRef))
                lstrTaxRef = busConstant.PayeeAccountTaxRefState22Tax;
            if (ibusTaxRefConfig.IsNull())
                ibusTaxRefConfig = new busTaxRefConfig() { icdoTaxRefConfig = new DataObjects.doTaxRefConfig() };
            ibusTaxRefConfig.FindTaxRefConfig(icdoPayeeAccountTaxWithholding.tax_identifier_value, lstrTaxRef, busGlobalFunctions.GetSysManagementBatchDate());
        }
        public void LoadW4PUIElements()
        {
            if (ibusTaxRefConfig.IsNull())
                LoadTaxRef();
            if (ibusTaxRefConfig.IsNotNull() && ibusTaxRefConfig.icdoTaxRefConfig.IsNotNull())
            {
                icdoPayeeAccountTaxWithholding.three_first_line = string.Format(ibusTaxRefConfig.icdoTaxRefConfig.three_first_line, string.Format("{0:C}",ibusTaxRefConfig.icdoTaxRefConfig.total_amt_single), string.Format("{0:C}", ibusTaxRefConfig.icdoTaxRefConfig.total_amt_married));
                icdoPayeeAccountTaxWithholding.three_second_line = string.Format(ibusTaxRefConfig.icdoTaxRefConfig.three_second_line, ibusTaxRefConfig.icdoTaxRefConfig.child_age, string.Format("{0:C}", ibusTaxRefConfig.icdoTaxRefConfig.child_age_by_amt));
                icdoPayeeAccountTaxWithholding.three_third_line = string.Format(ibusTaxRefConfig.icdoTaxRefConfig.three_third_line, string.Format("{0:C}", ibusTaxRefConfig.icdoTaxRefConfig.other_depd_amt));
                icdoPayeeAccountTaxWithholding.two_tip = ibusTaxRefConfig.icdoTaxRefConfig.two_tip;
            }
        }
        public bool IsFederalPercentLessThanMinimum()
        {
            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum &&
                !string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref))
            {
                if (ibusPayeeAccount.IsNull())
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper.LoadFlatTaxRate(ibusPayeeAccount.idtNextBenefitPaymentDate, icdoPayeeAccountTaxWithholding.tax_identifier_value, icdoPayeeAccountTaxWithholding.tax_ref, busConstant.Flag_No);
                return icdoPayeeAccountTaxWithholding.refund_fed_percent < lbusFedStateFlatTaxRate?.icdoFedStateFlatTaxRate?.min_tax_percentage ? true : false;
            }
            return false;
        }
    }
}
