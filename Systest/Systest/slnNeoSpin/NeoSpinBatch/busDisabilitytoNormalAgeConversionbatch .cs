using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpinBatch
{
    class busDisabilitytoNormalAgeConversionbatch : busNeoSpinBatch
    {
        public busDBCacheData ibusDBCacheData { get; set; }

        public void ConvertDisabilityToNormalBasedOnAgeEligibility()
        {
            //iobjPassInfo.istrUserID = busConstant.PERSLinkBatchUser;
            istrProcessName = "Disability to Normal Conversion - Age Based Batch ";

            DateTime ldtBatchStartDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            DateTime ldtBatchEndDate = new DateTime(ldtBatchStartDate.AddMonths(2).Year, ldtBatchStartDate.AddMonths(2).Month, 1);
            ldtBatchEndDate = ldtBatchEndDate.AddDays(-1);

            //Load benefit provision eligibility data
            if (ibusDBCacheData == null)
                ibusDBCacheData = new busDBCacheData();
            if (ibusDBCacheData.idtbCachedBenefitProvisionEligibility == null)
                ibusDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);

            idlgUpdateProcessLog("Load all Person applied for Disability and having Payee Account in receiving status", "INFO", istrProcessName);
            //get all disability record with receiving status
            DataTable ldtGetDisablePersonRecords = busBase.Select("cdoBenefitCalculation.LoadPayeeAccountForAgeConversion", new object[0] { });

            foreach (DataRow ldtr in ldtGetDisablePersonRecords.Rows)
            {
                // Load all objects related to Payee account 
                busPayeeAccount lobjPayeeAccount = LoadObjectsReturnPayeeAccount(ldtr);

                int lintBenefitProvisionId = lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id;

                DateTime ldtDateOfBirth = lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth;

                idlgUpdateProcessLog("For Person Id :" + lobjPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id +
                                    " For Plan " + lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPlan.icdoPlan.plan_name, "INFO", istrProcessName);

                if ((lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
                    && (lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC &&
                    lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                    lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC2025)) //PIR 25920
                {
                    //get eligibility Age for this person based on the plan and benefit account type
                    var lobjResult = (from a in ibusDBCacheData.idtbCachedBenefitProvisionEligibility.AsEnumerable()
                                where a.Field<int>("benefit_provision_id") == lintBenefitProvisionId
                                 && a.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement
                                 && a.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal
                                 && a.Field<DateTime>("Effective_date") <= ldtBatchEndDate
                                select a.Field<int>("age"));

                    int lintAgetobeAdded = Convert.ToInt32(lobjResult.FirstOrDefault());
                    // add no of years obtained to get Normal Eligibility
                    DateTime ldtNormalEligibilityDate = ldtDateOfBirth.AddYears(lintAgetobeAdded);

                    if ((busGlobalFunctions.CheckDateOverlapping(ldtNormalEligibilityDate, ldtBatchStartDate, ldtBatchEndDate)) ||
                        (ldtNormalEligibilityDate < ldtBatchStartDate))
                    {
                        // initiate workflow
                        InitializeWorkFlow(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, lobjPayeeAccount.icdoPayeeAccount.payee_account_id);

                        ////set workflow age conversion flag as yes
                        lobjPayeeAccount.icdoPayeeAccount.workflow_age_conversion_flag = busConstant.Flag_Yes;
                        lobjPayeeAccount.icdoPayeeAccount.Update();
                    }
                    else
                    {
                        idlgUpdateProcessLog("No Workflow Initialized for Person Id :" 
                            + lobjPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id, "INFO", istrProcessName);
                    }
                }
                else if (lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdJobService)
                {
                    int lintNumbersOfServiceMonths = busGlobalFunctions.DateDiffByMonth(lobjPayeeAccount.icdoPayeeAccount.benefit_begin_date, ldtBatchEndDate); ;
                    int lintNumberOfMonths = busGlobalFunctions.DateDiffByMonth(ldtDateOfBirth, ldtBatchEndDate);
                    /// FOR JOB-SERVICE ATTAINED AGE HAS TO BE CALCULATED, HENCE 65 REF:BR-080-07 && SYS PIR ID 2223
                    DateTime ldteCurrentYearDOB = ldtDateOfBirth.AddMonths(65 * 12);
                    if ((lintNumbersOfServiceMonths > 5 * 12) && (lintNumberOfMonths >= (65 * 12)))
                    {
                        idlgUpdateProcessLog("Creating New Payee Account Status For Old Payee Account Person Id :" +
                            lobjPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id,
                            "INFO", istrProcessName);
                        lobjPayeeAccount.icdoPayeeAccount.benefit_end_date = ldteCurrentYearDOB.GetLastDayofMonth();

                        // create a new Payment status and complete the Old Payee Account
                        CreateNewPayeeAccountStatus(busConstant.PayeeAccountStatusDisabilityPaymentCompleted,
                                                    lobjPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                    lobjPayeeAccount.icdoPayeeAccount.disa_normal_effective_date);

                        lobjPayeeAccount.icdoPayeeAccount.workflow_age_conversion_flag = busConstant.Flag_Yes;
                        // close old Payee Account and update                       
                        lobjPayeeAccount.icdoPayeeAccount.Update();

                        busPayeeAccount lobjNewPayeeAccount = new busPayeeAccount { icdoPayeeAccount = lobjPayeeAccount.icdoPayeeAccount };

                        //Copy other details related to Old Payee Account to new Payee Account
                        LoadOtherDetails(lobjPayeeAccount, lobjNewPayeeAccount);

                        lobjPayeeAccount.icdoPayeeAccount.workflow_age_conversion_flag = busConstant.Flag_No;

                        lobjNewPayeeAccount.icdoPayeeAccount.benefit_begin_date = ldteCurrentYearDOB.GetFirstDayofNextMonth();
                        lobjNewPayeeAccount.icdoPayeeAccount.benefit_end_date = DateTime.MinValue;

                        lobjNewPayeeAccount.icdoPayeeAccount.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                        lobjNewPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeNormal;

                        lobjNewPayeeAccount.icdoPayeeAccount.benefit_option_value = busConstant.BenefitOptionStraightLife;

                        // get new benefit account id
                        lobjNewPayeeAccount.icdoPayeeAccount.benefit_account_id = GetBenefitAccountId(lobjPayeeAccount);

                        idlgUpdateProcessLog("Creating New Payee Account For Person Id :" 
                            + lobjPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id, "INFO", istrProcessName);

                        //insert the New Payee Account
                        lobjNewPayeeAccount.icdoPayeeAccount.modified_by = null;
                        lobjNewPayeeAccount.icdoPayeeAccount.modified_date = DateTime.MinValue;
                        lobjNewPayeeAccount.icdoPayeeAccount.created_by = null;
                        lobjNewPayeeAccount.icdoPayeeAccount.created_date = DateTime.MinValue;
                        lobjNewPayeeAccount.icdoPayeeAccount.Insert();

                        //Insert other details for the new Payee account
                        InsertNewPayeeAccountOtherDetails(lobjNewPayeeAccount);
                    }
                }
            }
            if (ldtGetDisablePersonRecords.Rows.Count.Equals(0))
            {
                idlgUpdateProcessLog("No records found for conversion", "INFO", istrProcessName);
            }
        }

        /// <summary>
        /// this method returns a payee account
        /// after loading all objects required for this batch
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private busPayeeAccount LoadObjectsReturnPayeeAccount(DataRow dr)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lobjPayeeAccount.icdoPayeeAccount.LoadData(dr);

            lobjPayeeAccount.ibusBenefitCalculaton = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            lobjPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.LoadData(dr);

            lobjPayeeAccount.ibusApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lobjPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(dr);           

            lobjPayeeAccount.ibusApplication.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.LoadData(dr);

            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson.icdoPerson.LoadData(dr);

            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lobjPayeeAccount.ibusApplication.ibusPersonAccount.ibusPlan.icdoPlan.LoadData(dr);

            return lobjPayeeAccount;
        }

        private void InitializeWorkFlow(int aintPersonID, int aintPayeeAccountID)
        {
            idlgUpdateProcessLog("Initializing Workflow For Person Id :" + aintPersonID, "INFO", istrProcessName);

            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_Disability_to_Normal_Age_Conversion, aintPersonID, 0, aintPayeeAccountID, iobjPassInfo);
        }

        //Load collections of other details related to Old Payee account 
        // and map to the new Payee account
        private void LoadOtherDetails(busPayeeAccount aobjOldPayeeAccount, busPayeeAccount aobjNewPayeeAccount)
        {
            //Load Active ACH details
            if (aobjOldPayeeAccount.iclbActiveACHDetails == null)
                aobjOldPayeeAccount.LoadActiveACHDetail();
            if (aobjNewPayeeAccount.iclbActiveACHDetails == null)
            {
                aobjNewPayeeAccount.iclbActiveACHDetails = new Collection<busPayeeAccountAchDetail>();
                aobjNewPayeeAccount.iclbActiveACHDetails = aobjOldPayeeAccount.iclbActiveACHDetails;
            }

            // Load Fed tax Info
            if (aobjOldPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
                aobjOldPayeeAccount.LoadFedTaxWithHoldingInfo();
            //load item details
            foreach (busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding in aobjOldPayeeAccount.iclbPayeeAccountFedTaxWithHolding)
            {
                if (lobjPayeeAccountTaxWithholding.iclbTaxWithHoldingTaxItems == null)
                    lobjPayeeAccountTaxWithholding.LoadTaxWithHoldingTaxItems();
            }
            if (aobjNewPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
            {
                aobjNewPayeeAccount.iclbPayeeAccountFedTaxWithHolding = new Collection<busPayeeAccountTaxWithholding>();
                aobjNewPayeeAccount.iclbPayeeAccountFedTaxWithHolding = aobjOldPayeeAccount.iclbPayeeAccountFedTaxWithHolding;
            }

            //Load State Tax info
            if (aobjOldPayeeAccount.iclbPayeeAccountStateTaxWithHolding == null)
                aobjOldPayeeAccount.LoadStateTaxWithHoldingInfo();
            //load item details
            foreach (busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding in aobjOldPayeeAccount.iclbPayeeAccountStateTaxWithHolding)
            {
                if (lobjPayeeAccountTaxWithholding.iclbTaxWithHoldingTaxItems == null)
                    lobjPayeeAccountTaxWithholding.LoadTaxWithHoldingTaxItems();
            }
            if (aobjNewPayeeAccount.iclbPayeeAccountStateTaxWithHolding == null)
            {
                aobjNewPayeeAccount.iclbPayeeAccountStateTaxWithHolding = new Collection<busPayeeAccountTaxWithholding>();
                aobjNewPayeeAccount.iclbPayeeAccountStateTaxWithHolding = aobjOldPayeeAccount.iclbPayeeAccountStateTaxWithHolding;
            }

            //Load Active Rollover Detail
            if (aobjOldPayeeAccount.iclbActiveRolloverDetails == null)
                aobjOldPayeeAccount.LoadActiveRolloverDetail();
            //load item details
            foreach (busPayeeAccountRolloverDetail lobjPayeeAccountRolloverDetail in aobjOldPayeeAccount.iclbActiveRolloverDetails)
            {
                if (lobjPayeeAccountRolloverDetail.iclbRolloverItemDetail == null)
                    lobjPayeeAccountRolloverDetail.LoadRolloverItemDetail();
            }
            if (aobjNewPayeeAccount.iclbActiveRolloverDetails == null)
            {
                aobjNewPayeeAccount.iclbActiveRolloverDetails = new Collection<busPayeeAccountRolloverDetail>();
                aobjNewPayeeAccount.iclbActiveRolloverDetails = aobjOldPayeeAccount.iclbActiveRolloverDetails;
            }

            // Load Deductions
            if (aobjOldPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                aobjOldPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (aobjNewPayeeAccount.iclbPayeeAccountPaymentItemType == null)
            {
                aobjNewPayeeAccount.iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
                aobjNewPayeeAccount.iclbPayeeAccountPaymentItemType = aobjOldPayeeAccount.iclbPayeeAccountPaymentItemType;
            }
        }

        // Get Benefit Account id 
        //if will check if any benefit account exists for the person account 
        //and benefit account type value. if yes then return that benefit account id. 
        //else will return the new id after creating a new benefit account
        private int GetBenefitAccountId(busPayeeAccount aobjOldPayeeAccount)
        {
            int lintBenefitAccountID = aobjOldPayeeAccount.icdoPayeeAccount.benefit_account_id;
                //aobjOldPayeeAccount.ibusBenefitCalculaton.IsBenefitAccountExists(aobjOldPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.person_account_id,
                //busConstant.ApplicationBenefitTypeRetirement);

            if (aobjOldPayeeAccount.ibusBenefitAccount == null)
                aobjOldPayeeAccount.LoadBenfitAccount();

            aobjOldPayeeAccount.ibusApplication.GetRoundedTVSC();

            aobjOldPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson.GetTotalPSC(aobjOldPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date);

            lintBenefitAccountID = aobjOldPayeeAccount.ibusApplication.ManageBenefitAccount(
                                   aobjOldPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.starting_taxable_amount,
                                   aobjOldPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.starting_nontaxable_amount,
                                   busConstant.StatusValid, aobjOldPayeeAccount.ibusApplication.icdoBenefitApplication.rhic_option_value, 0,
                                   aobjOldPayeeAccount.ibusApplication.ibusPersonAccount.ibusPerson.idecRoundedTPSC,
                                   aobjOldPayeeAccount.ibusApplication.icdoBenefitApplication.idecTVSC,
                                   aobjOldPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_org_id, lintBenefitAccountID,
                                   DateTime.MinValue, 0.0M, string.Empty, 0M, 0M);

            return lintBenefitAccountID;
        }

        // create a new Payee account payment status 
        private void CreateNewPayeeAccountStatus(string astrPaymentStatus, int aintPayeeAccountId, DateTime adteStatusEffectiveDate)
        {
            busPayeeAccountStatus lobjPayeeAccountStatus = new busPayeeAccountStatus() { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.payee_account_id = aintPayeeAccountId;
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = astrPaymentStatus;
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = (adteStatusEffectiveDate == DateTime.MinValue) ? DateTime.Today : adteStatusEffectiveDate; // PROD PIR ID 5482
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.created_by = null;
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.modified_by = null;
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert();
        }

        /// <summary>
        /// insert into the table the details of the new payee account
        /// </summary>
        /// <param name="aobjPayeeAccount"></param>
        private void InsertNewPayeeAccountOtherDetails(busPayeeAccount aobjPayeeAccount)
        {
            idlgUpdateProcessLog("Creating New Payee Account Status For Person Id :" 
                + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);

            //insert Payee account status
            CreateNewPayeeAccountStatus(busConstant.PayeeAccountStatusReceiving,
                                        aobjPayeeAccount.icdoPayeeAccount.payee_account_id,
                                        aobjPayeeAccount.icdoPayeeAccount.disa_normal_effective_date);

            //insert ACH related details into DB          

            foreach (busPayeeAccountAchDetail lobjPayeeAccountACHDetail in aobjPayeeAccount.iclbActiveACHDetails)
            {
                idlgUpdateProcessLog("Creating New ACH details Status For Person Id :" 
                    + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);
                lobjPayeeAccountACHDetail.icdoPayeeAccountAchDetail.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPayeeAccountACHDetail.icdoPayeeAccountAchDetail.created_by = null;
                lobjPayeeAccountACHDetail.icdoPayeeAccountAchDetail.modified_by = null;
                lobjPayeeAccountACHDetail.icdoPayeeAccountAchDetail.created_date = DateTime.MinValue;
                lobjPayeeAccountACHDetail.icdoPayeeAccountAchDetail.modified_date = DateTime.MinValue;
                lobjPayeeAccountACHDetail.icdoPayeeAccountAchDetail.Insert();
            }

            //insert Rollover related details into DB            
            foreach (busPayeeAccountRolloverDetail lobjPayeeAccountRolloverDetail in aobjPayeeAccount.iclbActiveRolloverDetails)
            {
                idlgUpdateProcessLog("Creating New Rollover details Status For Person Id :" 
                    + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);
                lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.created_by = null;
                lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.modified_by = null;
                lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.created_date = DateTime.MinValue;
                lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.modified_date = DateTime.MinValue;
                lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.Insert();

                // insert item details also               
                foreach (busPayeeAccountRolloverItemDetail lobjRolloverItemDetails in lobjPayeeAccountRolloverDetail.iclbRolloverItemDetail)
                {
                    idlgUpdateProcessLog("Creating New Rollover Item details Status For Person Id :" 
                        + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);
                    lobjRolloverItemDetails.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id = 
                        lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id;
                    lobjRolloverItemDetails.icdoPayeeAccountRolloverItemDetail.created_by = null;
                    lobjRolloverItemDetails.icdoPayeeAccountRolloverItemDetail.modified_by = null;
                    lobjRolloverItemDetails.icdoPayeeAccountRolloverItemDetail.created_date = DateTime.MinValue;
                    lobjRolloverItemDetails.icdoPayeeAccountRolloverItemDetail.modified_date = DateTime.MinValue;
                    lobjRolloverItemDetails.icdoPayeeAccountRolloverItemDetail.Insert();
                }
            }

            //insert Fed Tax related details into DB            
            foreach (busPayeeAccountTaxWithholding lobjPayeeAccountFedTax in aobjPayeeAccount.iclbPayeeAccountFedTaxWithHolding)
            {
                idlgUpdateProcessLog("Creating New Fed Tax details Status For Person Id :" 
                    + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);
                lobjPayeeAccountFedTax.icdoPayeeAccountTaxWithholding.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPayeeAccountFedTax.icdoPayeeAccountTaxWithholding.created_by = null;
                lobjPayeeAccountFedTax.icdoPayeeAccountTaxWithholding.modified_by = null;
                lobjPayeeAccountFedTax.icdoPayeeAccountTaxWithholding.created_date = DateTime.MinValue;
                lobjPayeeAccountFedTax.icdoPayeeAccountTaxWithholding.modified_date = DateTime.MinValue;
                lobjPayeeAccountFedTax.icdoPayeeAccountTaxWithholding.Insert();

                // insert item details also                
                foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingItemDetails in lobjPayeeAccountFedTax.iclbTaxWithHoldingTaxItems)
                {
                    idlgUpdateProcessLog("Creating New Fed Tax Item details Status For Person Id :" 
                        + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_id = 
                        lobjPayeeAccountFedTax.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.created_by = null;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.modified_by = null;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.created_date = DateTime.MinValue;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.modified_date = DateTime.MinValue;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.Insert();
                }
            }

            //insert State related details into DB            
            foreach (busPayeeAccountTaxWithholding lobjPayeeAccountStateTax in aobjPayeeAccount.iclbPayeeAccountStateTaxWithHolding)
            {
                idlgUpdateProcessLog("Creating New State Tax details Status For Person Id :" 
                        + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);
                lobjPayeeAccountStateTax.icdoPayeeAccountTaxWithholding.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPayeeAccountStateTax.icdoPayeeAccountTaxWithholding.created_by = null;
                lobjPayeeAccountStateTax.icdoPayeeAccountTaxWithholding.modified_by = null;
                lobjPayeeAccountStateTax.icdoPayeeAccountTaxWithholding.created_date = DateTime.MinValue;
                lobjPayeeAccountStateTax.icdoPayeeAccountTaxWithholding.modified_date = DateTime.MinValue;
                lobjPayeeAccountStateTax.icdoPayeeAccountTaxWithholding.Insert();

                // insert item details also                
                foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingItemDetails in lobjPayeeAccountStateTax.iclbTaxWithHoldingTaxItems)
                {
                    idlgUpdateProcessLog("Creating New State Tax Item details Status For Person Id :" 
                        + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_id = 
                        lobjPayeeAccountStateTax.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.created_by = null;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.modified_by = null;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.created_date = DateTime.MinValue;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.modified_date = DateTime.MinValue;
                    lobjTaxWithholdingItemDetails.icdoPayeeAccountTaxWithholdingItemDetail.Insert();
                }
            }

            //insert Deductions related details into DB            
            foreach (busPayeeAccountPaymentItemType lobjPaymenItemType in aobjPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                idlgUpdateProcessLog("Creating New Deductions details Status For Person Id :" 
                    + aobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, "INFO", istrProcessName);
                lobjPaymenItemType.icdoPayeeAccountPaymentItemType.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPaymenItemType.icdoPayeeAccountPaymentItemType.created_by = null;
                lobjPaymenItemType.icdoPayeeAccountPaymentItemType.modified_by = null;
                lobjPaymenItemType.icdoPayeeAccountPaymentItemType.created_date = DateTime.MinValue;
                lobjPaymenItemType.icdoPayeeAccountPaymentItemType.modified_date = DateTime.MinValue;
                lobjPaymenItemType.icdoPayeeAccountPaymentItemType.Insert();
            }
        }
    }
}
