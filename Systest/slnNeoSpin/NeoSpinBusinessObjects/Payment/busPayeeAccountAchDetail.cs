#region Using directives

using System;
using Sagitec.Common;
using NeoSpin.CustomDataObjects;
using System.Data;
using System.Collections;
using System.Text;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Sagitec.ExceptionPub;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPayeeAccountAchDetail : busPayeeAccountAchDetailGen
    {
        public const decimal HUNDREDPERCENTAGE = 100.00M;
        //Load Active ACH Detail based on next benefit payment date
        //If Primary ACH already exists ,Then throw an error while creating the next one. BR - 18

        //PIR 18503
        public Collection<busWssAcknowledgement> iclbACHDetailsAcknowledgementPart1 { get; set; }
        public busOrganization ibusOrganization { get; set; }
       
        public string istrAcknowledgementFlag { get; set; }
        public bool iblnPartialAmount1 { get; set; }
        public bool iblnIsOTPGeneratedSuccessfully { get; set; }
        public bool iblnIsOTPEnteredVerified { get; set; }
        public string istrActivationCode { get; set; }
        public int iintOTPActivationID { get; set; }
        public bool iblnIsOTPExpired { get; set; }
        public bool iblnIsResendClicked { get; set; }
        public bool iblnRN1Exists { get; set; }
        public bool iblnRN2Exists { get; set; }


        private busPayeeAccountAchDetail _ibusPayeeAccountACHDetailSel1;
        public busPayeeAccountAchDetail ibusPayeeAccountACHDeatilSel1
        {
            get { return _ibusPayeeAccountACHDetailSel1; }
            set { _ibusPayeeAccountACHDetailSel1 = value; }

        }

        private busPayeeAccountAchDetail _ibusPayeeAccountACHDetailSel2;
        private int iintNewOrgIdFirst { get; set; }
        private int iintNewOrgIdSecond { get; set; }

        public busPayeeAccountAchDetail ibusPayeeAccountACHDeatilSel2
        {
            get { return _ibusPayeeAccountACHDetailSel2; }
            set { _ibusPayeeAccountACHDetailSel2 = value; }

        }

        public bool IsPrimaryAchAlreadyExist()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbAchDetail == null)
                ibusPayeeAccount.LoadACHDetail();
            foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in ibusPayeeAccount.iclbAchDetail)
            {
                if (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id
                    != icdoPayeeAccountAchDetail.payee_account_ach_detail_id)
                {
                    if ((busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountAchDetail.ach_start_date,
                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date,
                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date)) && (lobjPayeeAccountAchDetail.IsPrimaryACHSelected))
                    {
                        if (icdoPayeeAccountAchDetail.primary_account_flag == busConstant.Flag_Yes)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //If there is no Primary ACH exists ,then mandate the primary ach
        public bool IsPrimaryAchNotExist()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                            icdoPayeeAccountAchDetail.ach_start_date, icdoPayeeAccountAchDetail.ach_end_date))
            {
                if (ibusPayeeAccount.iclbActiveACHDetails == null)
                    ibusPayeeAccount.LoadActiveACHDetail();
                if (ibusPayeeAccount.iclbActiveACHDetails.Count == 0)
                {
                    if (icdoPayeeAccountAchDetail.primary_account_flag == busConstant.Flag_No)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsPercentageAndStartDateModifiedForSecondary()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in ibusPayeeAccount.iclbAchDetail)
            {
                if ((lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id
                    != icdoPayeeAccountAchDetail.payee_account_ach_detail_id) && (lobjPayeeAccountAchDetail.IsPrimaryACHSelected))
                {
                    if ((busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountAchDetail.ach_start_date,
                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date,
                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date))
                        && (icdoPayeeAccountAchDetail.primary_account_flag == busConstant.Flag_No))
                    {
                        if ((icdoPayeeAccountAchDetail.percentage_of_net_amount > 0.0M) || (icdoPayeeAccountAchDetail.partial_amount > 0.0M)
                           || (icdoPayeeAccountAchDetail.ach_start_date != lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //Load Active ACH Detail based on next benefit payment date and Get The Total ACH Amoun and Percentage
        //If the persentage is not equal to 100 or the amount is not equal to benefit amount,Then throw an error while creating the next one. BR - 13,14

        public bool IsTotalAchPercentageOrAmountInvalid()
        {
            decimal ldecTotalAchPercentage = 0.00M;
            decimal ldecTotalAchAmount = 0.00M;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idecBenefitAmount == 0.00M)
                ibusPayeeAccount.LoadBenefitAmount();
            decimal ldecBenefitAmount = ibusPayeeAccount.idecBenefitAmount;
            ibusPayeeAccount.LoadPrimaryACHDetail(icdoPayeeAccountAchDetail.ach_start_date);
            if (ibusPayeeAccount.ibusPrimaryAchDetail != null)
            {
                if (ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id
                    != icdoPayeeAccountAchDetail.payee_account_ach_detail_id)
                {
                    if (ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.partial_amount > 0.00M)
                    {
                        ldecTotalAchAmount = ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.partial_amount
                            + icdoPayeeAccountAchDetail.partial_amount;
                    }
                    else
                    {
                        ldecTotalAchPercentage = ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount +
                            icdoPayeeAccountAchDetail.percentage_of_net_amount;
                    }
                    ldecTotalAchAmount = Math.Round(ldecTotalAchAmount, 2, MidpointRounding.AwayFromZero);
                    ldecTotalAchPercentage = Math.Round(ldecTotalAchPercentage, 2, MidpointRounding.AwayFromZero);
                    ldecBenefitAmount = Math.Round(ldecBenefitAmount, 2, MidpointRounding.AwayFromZero);
                }
                if (((ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount > 0.00M) && (ldecTotalAchPercentage != HUNDREDPERCENTAGE))
                || ((ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.partial_amount > 0.00M) && (ldecTotalAchAmount != ldecBenefitAmount)))
                {
                    return true;
                }
            }
            return false;
        }
        // If Primary ACH is already exist for the Same Payee Account ,Set The Flag as"NO" and Calculate the Percentage
        // If Primary ACH is not exist ,Set it as Primary and Set the Percentage as 100%. BR - 10,11
        public void SetPrimaryOrSecordaryAch()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idecBenefitAmount == 0.00M)
                ibusPayeeAccount.LoadBenefitAmount();
            if (ibusPayeeAccount.iclbActiveACHDetails == null)
                ibusPayeeAccount.LoadActiveACHDetail();
            if (ibusPayeeAccount.iclbActiveACHDetails.Count == 0)
            {
                icdoPayeeAccountAchDetail.primary_account_flag = busConstant.Flag_Yes;
                icdoPayeeAccountAchDetail.ach_start_date = ibusPayeeAccount.idtNextBenefitPaymentDate;
                icdoPayeeAccountAchDetail.percentage_of_net_amount = 100.0M;
            }
            foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in ibusPayeeAccount.iclbAchDetail)
            {
                if (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id
                    != icdoPayeeAccountAchDetail.payee_account_ach_detail_id)
                {
                    if ((lobjPayeeAccountAchDetail.IsPrimaryACHSelected) && (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue))
                    {
                        icdoPayeeAccountAchDetail.primary_account_flag = busConstant.Flag_No;
                        icdoPayeeAccountAchDetail.ach_start_date = lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date;
                        icdoPayeeAccountAchDetail.percentage_of_net_amount = 0.0M;
                    }
                }
            }
        }
        // While creating secondary Ach ,The amount or percentage should be equal to Total benefit amount minus primary ach amount or percentage
        // else throw an error
        public bool IsPercentageOrAmountInValidForSecondary()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            ibusPayeeAccount.LoadPrimaryACHDetail(icdoPayeeAccountAchDetail.ach_start_date);
            if (ibusPayeeAccount.ibusPrimaryAchDetail != null)
            {
                if (ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id
                    != icdoPayeeAccountAchDetail.payee_account_ach_detail_id)
                {
                    if (ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount > 0.00M)
                    {
                        if (icdoPayeeAccountAchDetail.partial_amount > 0.00M)
                            return true;
                    }
                    else if (ibusPayeeAccount.ibusPrimaryAchDetail.icdoPayeeAccountAchDetail.partial_amount > 0.00M)
                    {
                        if (icdoPayeeAccountAchDetail.percentage_of_net_amount > 0.00M)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //If Ach Start Date is less than  next benefit payment date or not first of month,then throw an error BR-23
        public bool IsStartdateNotValid()
        {
            if (!AreFieldsReadOnly())
            {
                if (icdoPayeeAccountAchDetail.ach_start_date != DateTime.MinValue)
                {
                    if (ibusPayeeAccount == null)
                        LoadPayeeAccount();
                    if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        ibusPayeeAccount.LoadNexBenefitPaymentDate();
                    DateTime ldtAchStartDate = new DateTime(ibusPayeeAccount.idtNextBenefitPaymentDate.Year, ibusPayeeAccount.idtNextBenefitPaymentDate.Month, 1);
                    if (icdoPayeeAccountAchDetail.ach_start_date < ldtAchStartDate)
                    {
                        return true;
                    }
                    else if (icdoPayeeAccountAchDetail.ach_start_date >= ldtAchStartDate)
                    {
                        if (icdoPayeeAccountAchDetail.ach_start_date.Day != 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //Validate delete
        public bool IsStartDateLessthanNexBenefitPaymentDate()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (icdoPayeeAccountAchDetail.ach_start_date < ibusPayeeAccount.idtNextBenefitPaymentDate)
            {
                return true;
            }
            return false;
        }
        // ACH Start date should not be prior to Benefit Begin date or Retirement Date
        public bool IsStartDatePriorToBenefitBeginDate()
        {
            if (icdoPayeeAccountAchDetail.ach_start_date != DateTime.MinValue)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusApplication == null)
                    ibusPayeeAccount.LoadApplication();
                if ((icdoPayeeAccountAchDetail.ach_start_date < ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date) ||
                    (icdoPayeeAccountAchDetail.ach_start_date < ibusPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date))
                    return true;
            }
            return false;
        }

        //If Ach End Date is less than or equal to next benefit payment date or one day before next benefit payment date ,then throw an error BR-24
        public bool IsEnddateNotValid()
        {
            if ((icdoPayeeAccountAchDetail.ach_start_date != DateTime.MinValue) && (icdoPayeeAccountAchDetail.ach_end_date != DateTime.MinValue))
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtBatchDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
                if (icdoPayeeAccountAchDetail.ach_end_date < ldtBatchDate.AddDays(-1))
                {
                    return true;
                }
                else if (icdoPayeeAccountAchDetail.ach_end_date >= ldtBatchDate.AddDays(-1))
                {
                    DateTime ldtLastDateOfMonth = busGlobalFunctions.GetLastDayOfMonth(icdoPayeeAccountAchDetail.ach_end_date);
                    if (ldtLastDateOfMonth.Day != icdoPayeeAccountAchDetail.ach_end_date.Day)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsSecondaryAchAlreadyExist()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbAchDetail == null)
                ibusPayeeAccount.LoadACHDetail();
            foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in ibusPayeeAccount.iclbAchDetail)
            {
                if (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id
                    != icdoPayeeAccountAchDetail.payee_account_ach_detail_id)
                {
                    if ((busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountAchDetail.ach_start_date,
                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date,
                        lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date)) && (!lobjPayeeAccountAchDetail.IsPrimaryACHSelected))
                    {
                        if (icdoPayeeAccountAchDetail.primary_account_flag == busConstant.Flag_No)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (iblnIsFromMSS)
            {
                if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1 != null)
                {
                    //Load Bank Org by Routing number entered through screen in Selection 1.
                    LoadBankOrgByRoutingNumber(ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1);

                    //If Bank Exists for the Routing Number entered, then populate the Bank Name.
                    if (ibusBankOrg.icdoOrganization.org_id > 0)
                        ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.bank_org_id = ibusBankOrg.icdoOrganization.org_id;
                    else
                    {
                        ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.bank_org_id = 0;
                        //If Routing Number do not match then initiate WFL to create organization with Routing Number and Bank Name entered in pending status.
                        ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.iblnIsOrgWorkflowRequired1 = true;
                        ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.iblnOrgIDDoesNotExist1 = true;
                    }
                }
                if (ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2 != null)
                {
                    //Load Bank Org by Routing number entered through screen in Selection 2.
                    LoadBankOrgByRoutingNumber(ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2);

                    if (ibusBankOrg.icdoOrganization.org_id > 0)
                        ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.bank_org_id = ibusBankOrg.icdoOrganization.org_id;
                    else
                    {
                        ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.bank_org_id = 0;
                        ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.iblnIsOrgWorkflowRequired2 = true;
                        ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.iblnOrgIDDoesNotExist2 = true;
                    }
                }

                //Display the Remainder of Benefit in Selection 2 if Flat amount was entered in Selection 1.
                if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.partial_amount > 0.00M)
                {
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.idecRemainderOfBenefit = ibusPayeeAccount.idecBenefitAmount - ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.partial_amount;
                }
            }
            else
            {
                if (icdoPayeeAccountAchDetail.org_code != null)
                {
                    LoadBankOrgByOrgCode();
                    icdoPayeeAccountAchDetail.bank_org_id = ibusBankOrg.icdoOrganization.org_id;
                }
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            if (iblnIsFromMSS)
            {
                DateTime ldteSysBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                //End all existing ACH Details
                if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1 != null)
                {
                    ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.payee_account_id = ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.pre_note_flag = busConstant.Flag_Yes;
                    ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.primary_account_flag = busConstant.Flag_Yes;
                    ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.ach_start_date = ldteSysBatchDate.Day < 16 ? busGlobalFunctions.GetFirstDayofNextMonth(ldteSysBatchDate) : busGlobalFunctions.GetFirstDayofNextMonth(ldteSysBatchDate.AddMonths(1)); //PIR 21227
                   
                    //Ending any existing ACH Details.
                    if (ibusPayeeAccount.iclbACHDetailsWithEndDateNull.IsNullOrEmpty())
                        ibusPayeeAccount.LoadAllACHDetailsWithEndDateNullMSS();
                    
                    foreach (busPayeeAccountAchDetail lobjACHDetail in ibusPayeeAccount.iclbACHDetailsWithEndDateNull)
                    {
                        //lobjACHDetail.icdoPayeeAccountAchDetail.ach_end_date = ibusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1);
                        lobjACHDetail.icdoPayeeAccountAchDetail.ach_end_date = ldteSysBatchDate.Day < 16 ? busGlobalFunctions.GetLastDayofMonth(ldteSysBatchDate) : busGlobalFunctions.GetLastDayofMonth(ldteSysBatchDate.AddMonths(1)); //PIR 25589
                        lobjACHDetail.icdoPayeeAccountAchDetail.Update();
                    }

                }
                if (ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2 != null)
                {
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.payee_account_id = ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.primary_account_flag = busConstant.Flag_No;
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.pre_note_flag = busConstant.Flag_Yes;
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.ach_start_date = ldteSysBatchDate.Day < 16 ? busGlobalFunctions.GetFirstDayofNextMonth(ldteSysBatchDate) : busGlobalFunctions.GetFirstDayofNextMonth(ldteSysBatchDate.AddMonths(1)); //PIR 21227
                }
            }
            base.BeforePersistChanges();
        }


        //PIR 18271
        public override int PersistChanges()
        {
            if (icdoPayeeAccountAchDetail.old_ach_form == busConstant.Flag_Yes)
                icdoPayeeAccountAchDetail.asi_sent_flag = busConstant.Flag_Yes;

            if (iblnIsFromMSS)
            {
                if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.iblnIsOrgWorkflowRequired1)
                {
                    iintNewOrgIdFirst = InsertOrgBankForRoutingNumber(ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrBankName1, ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1);

                    LoadBankOrgByRoutingNumber(ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1);
                    ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.bank_org_id = ibusBankOrg.icdoOrganization.org_id;
                }
                if (ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.iblnIsOrgWorkflowRequired2)
                {
                    iintNewOrgIdSecond = InsertOrgBankForRoutingNumber(ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrBankName2, ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2);
                    LoadBankOrgByRoutingNumber(ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2);
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.bank_org_id = ibusBankOrg.icdoOrganization.org_id;
                }
            }

            return base.PersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            if (iblnIsFromMSS)
            {
                if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.iblnIsOrgWorkflowRequired1)
                {
                    LoadBankOrgByRoutingNumber(ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1);
                    if (iintNewOrgIdFirst > 0)
                        InitializeWorkflow(iintNewOrgIdFirst, busConstant.Map_Process_Create_And_Maintain_Organization_Information);
                }

                if (ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.iblnIsOrgWorkflowRequired2)
                {
                    LoadBankOrgByRoutingNumber(ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2);
                    if (iintNewOrgIdSecond > 0)
                        InitializeWorkflow(iintNewOrgIdSecond, busConstant.Map_Process_Create_And_Maintain_Organization_Information);
                }

                //Code to update Reference id in OTP Activation table.

                if (iblnIsOTPEnteredVerified)
                {
                    DataTable ldtCorrectOTP = Select("cdoWssOtpActivation.LoadGeneratedOTP", new object[1] { iintOTPActivationID });

                    if (ibusPayeeAccount == null)
                        LoadPayeeAccount();

                    //Load the latest ACH Detail
                    if (ibusPayeeAccount.iclbActiveACHDetails == null)
                        ibusPayeeAccount.LoadActiveACHDetail();

                    string lstrReferenceID = string.Empty;
                    foreach (busPayeeAccountAchDetail lobjACHDetail in ibusPayeeAccount.iclbActiveACHDetails)
                    {
                        if (lobjACHDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id > 0)
                        {
                            lstrReferenceID += lobjACHDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id.ToString() + ", ";
                        }
                    }
                    if (!string.IsNullOrEmpty(lstrReferenceID))
                    {
                        lstrReferenceID = lstrReferenceID.TrimEnd(new char[2] { ',', ' ' }); ;

                        if (ldtCorrectOTP.Rows.Count > 0)
                        {
                            foreach (DataRow dr in ldtCorrectOTP.Rows)
                            {
                                busWssOtpActivation lobjOTP = new busWssOtpActivation { icdoWssOtpActivation = new cdoWssOtpActivation() };
                                lobjOTP.icdoWssOtpActivation.LoadData(dr);
                                lobjOTP.icdoWssOtpActivation.reference_id = lstrReferenceID;
                                lobjOTP.icdoWssOtpActivation.activation_code_validate = busConstant.Flag_Yes;
                                lobjOTP.icdoWssOtpActivation.Update();

                            }
                        }
                    }
                }

                //Trigger a message to the message board on data persist. 
                busWSSHelper.PublishMSSMessage(0, 0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10319, iobjPassInfo), "Direct Deposit Information"), busConstant.WSS_MessageBoard_Priority_High,
                ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id);

                //PIR 18503 - Generate Correspondence
                if (ibusPerson == null)
                    LoadPerson();
                ibusPerson.istrEffectiveDate = ibusPayeeAccount.idtNextBenefitPaymentDate.ToString(busConstant.DateFormatMMddyyyy);
                 
                GenerateCorrespondence(ibusPerson);
            }
            else
            {
                //On Addition or modification of Ach Details change the payee account status to review
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusSoftErrors == null)
                    ibusPayeeAccount.LoadErrors();
                ibusPayeeAccount.CreateReviewPayeeAccountStatus();
                ibusPayeeAccount.iblnClearSoftErrors = false;
                ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;
                ibusPayeeAccount.iblnACHInfoChangeIndicator = true;
                ibusPayeeAccount.ValidateSoftErrors();
                ibusPayeeAccount.UpdateValidateStatus();

                //Load History to refresh the grid
                ibusPayeeAccount.LoadACHDetail();
            }
        }

        public override bool SeBpmActivityInstanceReferenceID()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                
                ibusPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                ibusPayeeAccount.SeBpmActivityInstanceReferenceID();
                //  ibusPayeeAccount.SetProcessInstanceParameters();
                ibusPayeeAccount.SetCaseInstanceParameters();
            }
            return true;
        }

        public override int Delete()
        {
            //On deletion of Ach Details change the payee account status to review
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusSoftErrors == null)
                ibusPayeeAccount.LoadErrors();
            ibusPayeeAccount.iblnClearSoftErrors = false;
            ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;
            ibusPayeeAccount.CreateReviewPayeeAccountStatus();
            ibusPayeeAccount.iblnACHInfoDeleteIndicator = true;
            ibusPayeeAccount.ValidateSoftErrors();
            ibusPayeeAccount.UpdateValidateStatus();
            return base.Delete();
        }

        //uat pir 1420
        public bool IsPayeeAccountStatusCancelPendingOrCancelled()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelPending() || ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                lblnResult = true;

            return lblnResult;
        }

        //PIR 18503
        public string istrConfirmationText
        {
            get
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusPayee == null)
                    ibusPayeeAccount.LoadPayeePerson();

                string luserName = ibusPayeeAccount.ibusPayee.icdoPerson.FullName ;
                DateTime Now = DateTime.Now;
                DataTable ldtbListWSSAck = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + busConstant.ACHDetailsAcknowledgement + "'");
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                if (ldtbListWSSAck.Rows.Count > 0)
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListWSSAck.Rows[0]["acknowledgement_text"].ToString();
                string lstrConfimation = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, luserName, Now);
                return lstrConfimation;
            } 
        }

        public string istrAcknowledgementText
        {
            get
            {
                DateTime ldteSysBatchDate;
                int lintDisplaySequence = 1;
                string lstrConfimation = string.Empty;
                ldteSysBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
                DateTime lACHEffectiveDate = ldteSysBatchDate.Day < 15 ? busGlobalFunctions.GetFirstDayofNextMonth(ldteSysBatchDate) : busGlobalFunctions.GetFirstDayofNextMonth(ldteSysBatchDate.AddMonths(1)); //PIR 21227 
                if(ldteSysBatchDate.Day >= 15)
                     lintDisplaySequence = 2;
                DataTable ldtbListWSSAck = Select("cdoWssAcknowledgement.SelectACHAck", new object[3] { busConstant.ACHDetailsAuthorizationPart1, DateTime.Now, lintDisplaySequence });
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                if (ldtbListWSSAck?.Rows.Count > 0)
                {
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListWSSAck.Rows[0]["acknowledgement_text"].ToString();
                    lstrConfimation = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, lACHEffectiveDate.ToString(busConstant.DateFormatMMddyyyy));
                }
                return lstrConfimation;
            }
        }

        public void LoadACHDetailsAcknowledgement()
        {
            busBase lbusbase = new busBase();
            iclbACHDetailsAcknowledgementPart1 = new Collection<busWssAcknowledgement>();
            DataTable ldtbList = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.ACHDetailsAuthorizationPart2 });
            iclbACHDetailsAcknowledgementPart1 = lbusbase.GetCollection<busWssAcknowledgement>(ldtbList);
        }
        
        public busOrganization GetBankNameByRoutingNumber1(string AintRoutingNumber1)
        {
            busOrganization lobjOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            if (AintRoutingNumber1 != null)
            {
                lobjOrg.FindOrganizationByRoutingNumber(AintRoutingNumber1);
                if (lobjOrg.icdoOrganization.org_id > 0)
                {
                    lobjOrg.icdoOrganization.iblnIsRouitngNumberExists1 = true;
                }
            }
            return lobjOrg;
        }

        public busOrganization GetBankNameByRoutingNumber2(string AintRoutingNumber2)
        {
            busOrganization lobjOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            if (AintRoutingNumber2 != null)
            {
                lobjOrg.FindOrganizationByRoutingNumber(AintRoutingNumber2);
                if (lobjOrg.icdoOrganization.org_id > 0)
                {
                    lobjOrg.icdoOrganization.iblnIsRouitngNumberExists2 = true;
                }
            }
            return lobjOrg;
        }

        public busPayeeAccountAchDetail GetPartialAmountSel1(string AidecPartialAmount1)
        {
            busPayeeAccountAchDetail lobjACH = new busPayeeAccountAchDetail { icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail() };
            if (!string.IsNullOrEmpty(AidecPartialAmount1) && !AidecPartialAmount1.Equals("$0.00") )
            {
                lobjACH.iblnPartialAmount1 = true;
            }
            else
                lobjACH.iblnPartialAmount1 = false;
            return lobjACH;
        }

        public void InitializeObjects()
        {
            ibusPayeeAccountACHDeatilSel1 = new busPayeeAccountAchDetail();
            ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1 = new cdoPayeeAccountAchDetail();

            ibusPayeeAccountACHDeatilSel2 = new busPayeeAccountAchDetail();
            ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2 = new cdoPayeeAccountAchDetail();

        }

        public bool IsAcknowledgementNotSelected() => istrAcknowledgementFlag == busConstant.Flag_Yes;

        //Initialize workflow if the entered routing number does not exists.
        public void InitializeWorkflow(int aintOrgID, int aintProcessID)
        {
            if (!busWorkflowHelper.IsActiveInstanceAvailableForOrg(busConstant.Map_Process_Create_And_Maintain_Organization_Information, aintOrgID))
            {
                busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_Create_And_Maintain_Organization_Information, 0, aintOrgID, 0, iobjPassInfo);
            }
        }

        //First Selection should be a primary account with Net Percentage defaulted to 100%.
        public void SetPrimaryACHDetail()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idecBenefitAmount == 0.00M)
                ibusPayeeAccount.LoadBenefitAmount();

            ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount = 100;
            ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.primary_account_flag = busConstant.Flag_Yes;
            ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.ach_start_date = ibusPayeeAccount.idtNextBenefitPaymentDate;
            ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.pre_note_flag = busConstant.Flag_Yes;
        }

        //Rule to check if Percentage is valid.(Total ACH percentage should be 100% )
        public bool IsTotalAchPercentageOrAmountInvalidMSS()
        {
            decimal ldecTotalAchPercentage = 0.00M;

            if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount > 0)
            {
                if (ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2 != null &&
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.account_number != null &&
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.bank_account_type_value != null)
                {
                    ldecTotalAchPercentage = ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount
                                    + ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.percentage_of_net_amount;
                }
                else
                {
                    ldecTotalAchPercentage = ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount;
                }
            }
            ldecTotalAchPercentage = Math.Round(ldecTotalAchPercentage, 2, MidpointRounding.AwayFromZero);

            if ((ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount > 0.00M) && ldecTotalAchPercentage != HUNDREDPERCENTAGE)
                return true;
                
            return false;
        }

        //Rule to check if Sel1 Account Number is null or Sel2 Account Number is null.
        public bool IsAccountNumberNullMSS()
        {
            if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.account_number == null)
                return true;
            else if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.account_number != null)
            {
                if ((ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2 != null ||
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.bank_account_type_value != null) &&
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.account_number == null)
                    return true;
            }
            return false;
        }

        //Rule to check if Account Type is selected or not.
        public bool IsAccountTypeNotSelectedMSS()
        {
            if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.bank_account_type_value == null)
                return true;
            else if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.bank_account_type_value != null)
            {
                if ((ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2 != null ||
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.account_number != null) &&
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.bank_account_type_value == null)
                    return true;
            }
            return false;
        }

        //Rule to check if Routing number is null
        public bool IsRoutingNumberNullMSS()
        {
            if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1 == null)
                return true;
            else if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1 != null)
            {
                if ((ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.account_number != null ||
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.bank_account_type_value != null) &&
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2 == null)
                    return true;
            }
            return false;
        }

        //Rule to check if Percentage of Primary ACH Detail is blank and Percentage of Secondary is entered.
        public bool IsPercentageOfPrimaryBlank()
        {
            if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.partial_amount == 0)
            {
                if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount == 0 &&
                    ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.percentage_of_net_amount > 0)
                    return true;
            }

            return false;
        }
        
        //Check if partial amount and percentage both are null
        public bool IsPartialAmountOrPercentageNullMSS()
        {
            if ((ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.partial_amount == 0 && ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount == 0) ||
                ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.partial_amount != 0 && ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount != 0)
                return true;
            return false;
        }

        //Rule to check if Percentage of net amount is greater than 100%. (Percentage cannot be greater than 100.)
        public bool IsPercentageOfNetAmountGreaterThan100()
        {
            if (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount > HUNDREDPERCENTAGE ||
                (ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount + ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.percentage_of_net_amount) > HUNDREDPERCENTAGE)
                return true;
            return false;
        }
        
        //Insert a pending Bank record with routing number and bank name if the routing number entered by the user does not exists.
        public int InsertOrgBankForRoutingNumber(string astrBankName, string astrRoutingNumber)
        {
            busOrganization lobjOrganization = new busOrganization();
            lobjOrganization.icdoOrganization = new cdoOrganization();

            lobjOrganization.icdoOrganization.org_name = astrBankName;
            lobjOrganization.icdoOrganization.routing_no = astrRoutingNumber;
            lobjOrganization.icdoOrganization.org_type_value = busConstant.OrganizationTypeBank;
            lobjOrganization.icdoOrganization.status_value = busConstant.OrganizationStatusPending;

            lobjOrganization.icdoOrganization.org_code = lobjOrganization.GetNewOrgCodeRangeID();
            DBFunction.DBNonQuery("cdoOrgCodeByType.UpdateLastEnteredOrgCodeID", new object[2] {
                                            (Convert.ToInt32(lobjOrganization.icdoOrganization.org_code)- 1), Convert.ToInt32(lobjOrganization.icdoOrganization.org_code) },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (lobjOrganization.icdoOrganization.org_code.Length < 6)
                lobjOrganization.icdoOrganization.org_code = lobjOrganization.icdoOrganization.org_code.PadLeft(6, '0');

            lobjOrganization.icdoOrganization.Insert();
            return lobjOrganization.icdoOrganization.org_id;
        }

        public ArrayList btnVertifyOTP_Click(string AstrActivationCode)
        {
            ArrayList larrList = new ArrayList();
            BeforeValidate(utlPageMode.All);
            ValidateHardErrors(utlPageMode.All);
            istrActivationCode = AstrActivationCode;
            if (IsActivationCodeEnteredWithin30Mintues())
            {
                iblnIsOTPExpired = true;
                utlError lobjError = new utlError();
                lobjError = AddError(10328, "");
                larrList.Add(lobjError);
                return larrList;
            }

            if (!IsActivationCodeEnteredValid())
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10318, "");
                larrList.Add(lobjError);
                return larrList;
            }
            
            if (iarrErrors.Count > 0)
            {
                return iarrErrors;
            }
            else
            {
                iblnIsOTPEnteredVerified = true;
                EvaluateInitialLoadRules(utlPageMode.All);
                BeforeValidate(utlPageMode.All);
                BeforePersistChanges();
                PersistChanges();
                AfterPersistChanges();
            }
            larrList.Add(this);
            return larrList;
        }

        //Rule to check if OTP entered is correct.
        public bool IsActivationCodeEnteredValid()
        {
            bool lblnResult = false;
            DataTable ldtCorrectOTP = Select("cdoWssOtpActivation.IsOTPEnteredCorrect", new object[2] { ibusPayeeAccount.ibusPayee.icdoPerson.person_id, busConstant.PayeeAccountACHDetailOTPSource });
            if (ldtCorrectOTP.Rows.Count > 0)
            {
                if (ldtCorrectOTP.Rows[0]["ACTIVATION_CODE"].Equals(istrActivationCode))
                {
                    iintOTPActivationID = Convert.ToInt32(ldtCorrectOTP.Rows[0]["OTP_ACTIVATION_ID"]);                    

                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        //Check if OTP Entered has expired.
        public bool IsActivationCodeEnteredWithin30Mintues()
        {
            bool iblnResult = false;
            DataTable ldtCorrectOTP = Select("cdoWssOtpActivation.IsGeneratedOTPExpired", new object[2] { ibusPayeeAccount.ibusPayee.icdoPerson.person_id, busConstant.PayeeAccountACHDetailOTPSource });
            if (ldtCorrectOTP.Rows.Count == 0)
            {
                iblnResult = true;
            }
            return iblnResult;
        }

        public void GenerateOTPForPayeeAccountACHDetail()
        {
            try
            {
                //Generate an OTP
                busWssOtpActivation lobjWssOTPActivation = new busWssOtpActivation { icdoWssOtpActivation = new cdoWssOtpActivation() };

                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                //if (ibusPayeeAccount.ibusPayee == null) //F/W Upgrade PIR 11721 - Loaded email everytime the OTP need to be sent.
                ibusPayeeAccount.LoadPayeePerson();

                lobjWssOTPActivation.icdoWssOtpActivation.person_id = ibusPayeeAccount.ibusPayee.icdoPerson.person_id;
                lobjWssOTPActivation.icdoWssOtpActivation.source_value = busConstant.PayeeAccountACHDetailOTPSource;
                lobjWssOTPActivation.icdoWssOtpActivation.activation_code = busGlobalFunctions.GenerateAnOTP();
                lobjWssOTPActivation.icdoWssOtpActivation.activation_code_date = DateTime.Now;

                lobjWssOTPActivation.icdoWssOtpActivation.activation_code_validate = busConstant.Flag_No;
                lobjWssOTPActivation.icdoWssOtpActivation.Insert();

                //Send an email with OTP
                string lstrEmailFrom = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSRetrieveMailFrom);
                string lstrActivationCodeEmailSub = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSActivationCodeEmailSubject);
                string lstrActivationCodeEmailMsg = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSActivationCodeEmailMsg);
                string lstrEmailMessage = string.Empty;

                //PIR 20049
                string lstrEmailChangeEmailMsgSignature = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSMailBodySignature);
                string lstrMemberName = ibusPayeeAccount.ibusPayee.icdoPerson.first_name + " " + ibusPayeeAccount.ibusPayee.icdoPerson.last_name;

                lstrEmailMessage = string.Format(lstrActivationCodeEmailMsg + lstrEmailChangeEmailMsgSignature, lstrMemberName, Convert.ToString(lobjWssOTPActivation.icdoWssOtpActivation.activation_code));

                lstrActivationCodeEmailMsg = lstrActivationCodeEmailMsg + " : " + (lobjWssOTPActivation.icdoWssOtpActivation.activation_code).ToString();
                busGlobalFunctions.SendMailRetryOnFail(lstrEmailFrom, ibusPayeeAccount.ibusPayee.icdoPerson.email_address, lstrActivationCodeEmailSub, lstrEmailMessage, true, true);
            }
            catch(Exception e)
            {
                ExceptionManager.Publish(e);
                utlError lutlError = new utlError();
                lutlError.istrErrorMessage = NeoSpin.Common.ApplicationSettings.Instance.MSSEmailServerNotReachableMsg;
                this.iarrErrors.Add(lutlError);
            }
        }

        

        //Function is called on Save button click.
        public ArrayList btnGenerateOTP_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            BeforeValidate(utlPageMode.All);
            ValidateHardErrors(utlPageMode.All);

            if (iarrErrors.Count == 0)
            {
                GenerateOTPForPayeeAccountACHDetail();
                iblnIsOTPGeneratedSuccessfully = true;
                EvaluateInitialLoadRules(utlPageMode.All);

                if (!string.IsNullOrEmpty(ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1))
                {
                    ibusOrganization = new busOrganization();
                    ibusOrganization = GetBankNameByRoutingNumber1(ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrRoutingNumber1);
                    if (ibusOrganization.icdoOrganization.org_id > 0)//PIR 20049 - Point 5
                        ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrBankName1 = ibusOrganization.icdoOrganization.org_name;
                    iblnRN1Exists = ibusOrganization.icdoOrganization.iblnIsRouitngNumberExists1;

                }
                if (!string.IsNullOrEmpty(ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2))
                {
                    ibusOrganization = new busOrganization();
                    ibusOrganization = GetBankNameByRoutingNumber2(ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2);
                    if (ibusOrganization.icdoOrganization.org_id > 0)//PIR 20049 - Point 5
                        ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrBankName2 = ibusOrganization.icdoOrganization.org_name;
                    iblnRN2Exists = ibusOrganization.icdoOrganization.iblnIsRouitngNumberExists2;
                }

                alReturn.Add(this);
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

        public ArrayList btnResendOTP_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            BeforeValidate(utlPageMode.All);
            ValidateHardErrors(utlPageMode.All);

            if (iarrErrors.Count == 0)
            {
                if (iblnIsOTPExpired)
                    istrActivationCode = null;

                iblnIsResendClicked = true;
                iblnIsOTPExpired = false;

                GenerateOTPForPayeeAccountACHDetail();
                iblnIsOTPGeneratedSuccessfully = true;

                EvaluateInitialLoadRules(utlPageMode.All);
                alReturn.Add(this);
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

        //Rule to check if email address is waived.
        public bool IsEmailAddressWaived()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayeePerson();

            if (ibusPayeeAccount.ibusPayee.icdoPerson.email_waiver_flag == busConstant.Flag_Yes)
                return true;
            else if ((ibusPayeeAccount.ibusPayee.icdoPerson.email_waiver_flag == busConstant.Flag_No || ibusPayeeAccount.ibusPayee.icdoPerson.email_waiver_flag.IsNullOrEmpty()) 
                && ibusPayeeAccount.ibusPayee.icdoPerson.email_address.IsNotNullOrEmpty())
                return false;

            return false;
        }

        //Rule to check if routing number does not exists and bank name is null.
        public bool IsBankNameNull()
        {
            if ((ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.iblnOrgIDDoesNotExist1 &&
                ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrBankName1 == null) ||
                (ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.iblnOrgIDDoesNotExist2 &&
                ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrBankName2 == null))
                return true;

            return false;

        }

        //Rule to check bank length.
        public bool IsBankNameLengthGreaterThan50()
        {
            if ((ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.iblnOrgIDDoesNotExist1 &&
                ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrBankName1 != null &&
                ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.istrBankName1.Length > 50) ||
                (ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.iblnOrgIDDoesNotExist2 &&
                ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrBankName2 != null &&
                ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrBankName2.Length > 50))
                return true;
            return false;
        }

        public void GenerateCorrespondence(busPerson aobjPerson)
        {
            ArrayList larrlist = new ArrayList();
            larrlist.Add(aobjPerson);

            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence("PAY-4056", aobjPerson, lhstDummyTable);
            Sagitec.CorBuilder.CorBuilderXML lobjCorBuilder = new Sagitec.CorBuilder.CorBuilderXML();
            lobjCorBuilder.InstantiateWord();
            lobjCorBuilder.CreateCorrespondenceFromTemplate("PAY-4056", lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
            lobjCorBuilder.CloseWord();

        }
        //PIR 24923- is record exists with same start date
        public bool IsACHDetailRecordExistsWithSameStartDate()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbAchDetail.IsNull())
                ibusPayeeAccount.LoadACHDetail();
            DateTime ldteSysBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
            DateTime lACHEffectiveDate = ldteSysBatchDate.Day < 16 ? busGlobalFunctions.GetFirstDayofNextMonth(ldteSysBatchDate) : busGlobalFunctions.GetFirstDayofNextMonth(ldteSysBatchDate.AddMonths(1));

            if(ibusPayeeAccount.iclbAchDetail.IsNotNull() && ibusPayeeAccount.iclbAchDetail.Count > 0 && 
               ibusPayeeAccount.iclbAchDetail.Where(i => i.icdoPayeeAccountAchDetail.ach_start_date.Date == lACHEffectiveDate.Date).Count() > 0)
                    return true;
            return false;
        }

        public bool IsMultipleDirectDepositAccountSubmitted()
        {

            if (iblnIsFromMSS == true)
            {
                if ((ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.percentage_of_net_amount < 100 ||
                    ibusPayeeAccountACHDeatilSel1.icdoPayeeAccountAchDetailSel1.partial_amount > 0) && ibusPayeeAccountACHDeatilSel2.icdoPayeeAccountAchDetailSel2.istrRoutingNumber2.IsNullOrEmpty())
                {
                    return true;
                }
            }
                return false;
            
        }
    }
}
